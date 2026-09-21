# EF Core Configurations — Design Reference

**One section per entity, covering the intended schema design for every `IEntityTypeConfiguration<T>` under `Persistence/Configurations/`.** This is a design specification, not a description of finished code: as of this writing only `UserConfiguration` and `SupplierConfiguration` have real content; the other twelve are documented here as the plan to implement against.

This document assumes the conventions already established in `Infrastructure.md` (Section 5) — enum-to-string conversion, filtered unique indexes for nullable-unique columns, `.IsRowVersion()` for optimistic concurrency, `DeleteBehavior.Restrict` for relationships into audit history. Those rules are not repeated per entity below; only what is specific to each entity is called out.

---

## Table of Contents

1. [A cross-cutting risk to read first: multiple cascade paths](#1-a-cross-cutting-risk-to-read-first-multiple-cascade-paths)
2. [UserConfiguration](#2-userconfiguration) — implemented
3. [SupplierConfiguration](#3-supplierconfiguration) — implemented
4. [ItemConfiguration](#4-itemconfiguration) — planned
5. [PurchaseRequestConfiguration](#5-purchaserequestconfiguration) — planned
6. [PurchaseRequestDetailConfiguration](#6-purchaserequestdetailconfiguration) — planned
7. [SupplierQuoteCopyConfiguration](#7-supplierquotecopyconfiguration) — planned
8. [SupplierQuoteDetailConfiguration](#8-supplierquotedetailconfiguration) — planned
9. [RequestQuotationConfiguration](#9-requestquotationconfiguration) — planned
10. [RequestQuotationDetailConfiguration](#10-requestquotationdetailconfiguration) — planned
11. [RQApprovalConfiguration](#11-rqapprovalconfiguration) — planned
12. [PurchaseOrderConfiguration](#12-purchaseorderconfiguration) — planned
13. [PurchaseOrderDetailConfiguration](#13-purchaseorderdetailconfiguration) — planned
14. [ApprovalSettingConfiguration](#14-approvalsettingconfiguration) — planned
15. [CounterConfiguration](#15-counterconfiguration) — planned

---

## 1. A cross-cutting risk to read first: multiple cascade paths

`PurchaseRequest` and `SupplierQuoteCopy` reference each other in both directions:

- `SupplierQuoteCopy.PurchaseRequestId` → `PurchaseRequest` (a copy belongs to a request)
- `PurchaseRequest.SelectedSupplierCopyId` → `SupplierQuoteCopy` (the request points back at whichever copy was selected)

If both relationships were configured with `DeleteBehavior.Cascade`, SQL Server refuses to create the second foreign key at migration time (`"may cause cycles or multiple cascade paths"`). The rule applied throughout this document: **the "child owns lifecycle" direction (`SupplierQuoteCopy.PurchaseRequestId`) cascades; the "reference to a sibling" direction (`PurchaseRequest.SelectedSupplierCopyId`) does not** — it uses `DeleteBehavior.NoAction`. This is called out again at the two relevant entities below, but the underlying reason is this cycle.

---

## 2. UserConfiguration

**Status: implemented.** Reproduced here for completeness; see the file itself for the exact current code.

| Aspect | Design |
|---|---|
| Table | `users` |
| Primary key | `Id` |
| Notable columns | `Name` (150), `Email` (255), `Role` → string (30), `Department` (100), `IsActive` default `true`, `PasswordHash` (500) |
| Indexes | Unique on `Email` |
| Relationships | `PurchaseRequests` (1:many via `PurchaseRequest.RequesterId`, `Restrict`); `RQApprovals` (1:many via `RQApproval.ApproverId`, `Restrict`) |

---

## 3. SupplierConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `suppliers` |
| Primary key | `Id` |
| Notable columns | `Name` (150), `Email` (255), `Contact` (100, nullable), `CreditorCode` (50, nullable), `AccountStatus` → string (20), `RegistrationToken` (100, nullable), `PasswordHash` (500, nullable) |
| Indexes | Unique on `Email`; unique + filtered (`IS NOT NULL`) on `CreditorCode`; unique + filtered on `RegistrationToken` |
| Relationships | `RequestQuotations` (1:many via `SupplierId`, `Restrict`); `PurchaseOrders` (1:many via `SupplierId`, `Restrict`) |

---

## 4. ItemConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `items` |
| Primary key | `Id` |
| Notable columns | `Code` (50) — the AutoCount sync-matching key; `Name` (150); `Uom` (20); `RefPrice` — `decimal`, `.HasPrecision(18, 2)` |
| Indexes | Unique on `Code` |
| Relationships | None. `PurchaseRequestDetail.ItemCode` is a plain string snapshot, not a foreign key to `Item` — see the corresponding section. |

`RefPrice` needs an explicit `.HasPrecision(18, 2)` (or whatever precision the business actually needs) — EF Core will otherwise emit a "decimal properties not configured for precision" warning at migration time and silently pick a default that may not match what the application assumes.

---

## 5. PurchaseRequestConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `purchaserequests` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `Department` (100); `Status` → string (20), default `"Quoting"`; `PmRemarks` (1000, nullable); `CreditorCode` / `CreditorName` (50 / 150, nullable — snapshots, not live references); `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo` |
| Relationships | `Requester` → `User` via `RequesterId`, `Restrict` (see `Domain.md` Known Issues item 1 for the current `RequestedId` naming — this design assumes it has been renamed to `RequesterId`); `DecidedByUser` → `User?` via `DecidedByUserId`, `Restrict`; `SelectedSupplierCopy` → `SupplierQuoteCopy?` via `SelectedSupplierCopyId`, **`NoAction`** (see the corresponding section); `Items` → `PurchaseRequestDetail` (1:many, `Cascade` — detail lines have no life apart from their request); `SupplierQuoteCopies` (1:many, `Cascade` — same reasoning); `RequestQuotations` (1:many, `Restrict`) |

**A design question worth resolving before writing this file**: `PurchaseRequest.RequestQuotations` is typed `ICollection<RequestQuotation>`, but the Full Flow in the Scope Document describes a Purchase Request converting into *a* Request Quotation (singular), and `CreateFromApprovedPRCommandHandler` is designed as a one-time conversion. If the real cardinality is 1:0..1, this is the same class of issue already tracked as `Domain.md` Known Issues item 3 (`RequestQuotation.PurchaseOrder`) — worth confirming and, if so, tracking as an additional known issue rather than configuring it as an open 1:many relationship.

---

## 6. PurchaseRequestDetailConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `purchaserequestdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500, nullable); `Location` (200, nullable — see `Domain.md`, entity table note: the business meaning of this field is still unconfirmed); `Uom` (20); `Qty` — `decimal`, `.HasPrecision(18, 3)` |
| Indexes | None beyond the FK |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, `Cascade` (a detail line is owned by its request) |

`QuotedBy` (the collection of `SupplierQuoteDetail` referencing this line) is configured from the other side — see the corresponding section.

---

## 7. SupplierQuoteCopyConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `supplierquotecopies` |
| Primary key | `Id` |
| Notable columns | `Token` (100); `Status` → string (20), default `"Pending"`; `Remarks` (1000, nullable); `TotalAmount` — `decimal(18,2)`; `SentAt` / `SubmittedAt` — `datetime2` |
| Indexes | Unique on `Token` (not nullable, so no filter needed) |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, **`Cascade`**; `Supplier` via `SupplierId`, `Restrict` |

This is the "owned" side of the cycle described in the corresponding section — its cascade toward `PurchaseRequest` is what forces `PurchaseRequest.SelectedSupplierCopyId` to be `NoAction` rather than `Cascade`.

---

## 8. SupplierQuoteDetailConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `supplierquotedetails` |
| Primary key | `Id` |
| Notable columns | `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond FKs |
| Relationships | `SupplierQuoteCopy` via `SupplierQuoteCopyId`, `Cascade`; the second relationship via `PurchaseRequestItemId` should point at `PurchaseRequestDetail`, `Restrict` |

The second relationship depends on `Domain.md` Known Issues item 2 (`SupplierQuoteDetail.purchaseRequest` is currently typed `PurchaseRequest`, not `PurchaseRequestDetail`, despite the paired key being `PurchaseRequestItemId`). This `Configuration` cannot be written correctly until that type is fixed — configuring a relationship against the wrong entity type would not match the foreign key's actual target.

---

## 9. RequestQuotationConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `requestquotations` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `TotalAmount` — `decimal(18,2)`; `Status` → string (20); `RequiresL2` — `bool`; `CreatedAt` — `datetime2`; `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo` |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, `Restrict`; `Supplier` via `SupplierId`, `Restrict`; `Items` → `RequestQuotationDetail` (1:many, `Cascade`); `Approvals` → `RQApproval` (1:many, `Cascade`); `PurchaseOrder` |

`PurchaseOrder` is `Domain.md` Known Issues item 3 — typed `ICollection<PurchaseOrder>` for what the business rule describes as a 1:1 relationship (one RQ converts into at most one PO). Configuring this correctly means either resolving that type first, or — if the type stays a collection for now — adding a unique index on `PurchaseOrder.RequestQuotationId` (see the corresponding section) so the database enforces the 1:1 constraint regardless of what the C# type says.

---

## 10. RequestQuotationDetailConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `requestquotationdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500); `Uom` (20); `Qty` — `decimal(18,3)`; `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond the FK |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Cascade` |

---

## 11. RQApprovalConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `rqapprovals` |
| Primary key | `Id` |
| Notable columns | `Level` → string (10); `Action` → string (20); `Remark` (1000, nullable); `Timestamp` — `datetime2` |
| Indexes | None required by any current use case |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Cascade`; `Approver` → `User` via `ApproverId`, `Restrict` |

---

## 12. PurchaseOrderConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `purchaseorders` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `TotalAmount` — `decimal(18,2)`; `Status` → string (20); `IsNewSupplier` — `bool`; `CreatedAt` / `SyncedAt` — `datetime2` (`SyncedAt` nullable); `AutoCountPORef` / `AutoCountCreditorRef` (100, nullable); `SyncError` (2000, nullable); `SyncAttempts` — `int`, default `0`; `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo`; **unique on `RequestQuotationId`** — this is what actually enforces "one RQ converts to at most one PO" at the database level, independent of whether the C# navigation property on `RequestQuotation` is ever corrected (see the corresponding section) |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Restrict`; `Supplier` via `SupplierId`, `Restrict`; `Items` → `PurchaseOrderDetail` (1:many, `Cascade`) |

---

## 13. PurchaseOrderDetailConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `purchaseorderdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500); `Uom` (20); `Qty` — `decimal(18,3)`; `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond the FK |
| Relationships | `PurchaseOrder` via `PurchaseOrderId`, `Cascade` |

---

## 14. ApprovalSettingConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `approvalsettings` |
| Primary key | `Level` — **not `Id`**. `ApprovalSetting` does not implement `IEntity`; its natural key is the `ApprovalLevel` value itself (only two rows ever exist: `L1`, `L2`). |
| Notable columns | `Level` → string (10, matches the primary key); `ApproverRole` → string (30); `MinAmount` — `decimal(18,2)`; `MaxAmount` — `decimal(18,2)`, nullable (`null` = no upper bound) |
| Indexes | None beyond the primary key |
| Relationships | None. `ApproverRole` is a `Role` enum value, not a foreign key to any specific `User` row — the actual approver at runtime is resolved by matching a logged-in `User.Role` against this value, not by a database relationship. |

Recall from `Infrastructure.md` Section 3: this table's two rows are populated by `ApplicationDbContextSeed.cs`, not created through a Command — there is deliberately no "create" use case for this entity.

---

## 15. CounterConfiguration

**Status: planned.**

| Aspect | Design |
|---|---|
| Table | `counters` |
| Primary key | `Name` — a string (`"PR"`, `"RQ"`, `"PO"`), not `Id`. `Counter` implements no marker interface at all. |
| Notable columns | `Name` (10); `Seq` — `int`, default `0` |
| Indexes | None beyond the primary key |
| Relationships | None |

No `RowVersion` and no optimistic concurrency here — the atomic increment for document numbering is expected to be implemented with `ExecuteUpdateAsync` (a single `UPDATE ... SET Seq = Seq + 1` statement), which is inherently safe under concurrent access without needing a concurrency token at the EF Core level.
