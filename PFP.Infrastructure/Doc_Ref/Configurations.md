# EF Core Configurations — Design Reference

**One section per entity, covering every `IEntityTypeConfiguration<T>` under `Persistence/Configurations/`.** All 14 are implemented and the `InitialCreate` migration is applied against the real `PFP_Procurement` database (SQL Server LocalDB) — this document now describes finished, verified code, not a plan to implement against. Every column, index, and relationship below is cross-checked against the actual generated migration SQL, not just read off the `Configuration` source.

This document assumes the conventions already established in `Infrastructure.md` (Section 7) — enum-to-string conversion, filtered unique indexes for nullable-unique columns, `.IsRowVersion()` for optimistic concurrency, `DeleteBehavior.Restrict` for relationships into audit history, and always naming the inverse navigation explicitly in `.WithMany(x => x.Collection)`. Those rules are not repeated per entity below; only what is specific to each entity is called out.

---

## Table of Contents

1. [A cross-cutting risk to read first: multiple cascade paths](#1-a-cross-cutting-risk-to-read-first-multiple-cascade-paths)
2. [UserConfiguration](#2-userconfiguration) — implemented
3. [SupplierConfiguration](#3-supplierconfiguration) — implemented
4. [ItemConfiguration](#4-itemconfiguration) — implemented
5. [PurchaseRequestConfiguration](#5-purchaserequestconfiguration) — implemented
6. [PurchaseRequestDetailConfiguration](#6-purchaserequestdetailconfiguration) — implemented
7. [SupplierQuoteCopyConfiguration](#7-supplierquotecopyconfiguration) — implemented
8. [SupplierQuoteDetailConfiguration](#8-supplierquotedetailconfiguration) — implemented
9. [RequestQuotationConfiguration](#9-requestquotationconfiguration) — implemented
10. [RequestQuotationDetailConfiguration](#10-requestquotationdetailconfiguration) — implemented
11. [RQApprovalConfiguration](#11-rqapprovalconfiguration) — implemented
12. [PurchaseOrderConfiguration](#12-purchaseorderconfiguration) — implemented
13. [PurchaseOrderDetailConfiguration](#13-purchaseorderdetailconfiguration) — implemented
14. [ApprovalSettingConfiguration](#14-approvalsettingconfiguration) — implemented
15. [CounterConfiguration](#15-counterconfiguration) — implemented

---

## 1. A cross-cutting risk to read first: multiple cascade paths

`PurchaseRequest` and `SupplierQuoteCopy` reference each other in both directions:

- `SupplierQuoteCopy.PurchaseRequestId` → `PurchaseRequest` (a copy belongs to a request)
- `PurchaseRequest.SelectedSupplierCopyId` → `SupplierQuoteCopy` (the request points back at whichever copy was selected)

If both relationships were configured with `DeleteBehavior.Cascade`, SQL Server refuses to create the second foreign key at migration time (`"may cause cycles or multiple cascade paths"`). The rule applied throughout this document: **the "child owns lifecycle" direction (`SupplierQuoteCopy.PurchaseRequestId`) cascades; the "reference to a sibling" direction (`PurchaseRequest.SelectedSupplierCopyId`) does not** — it uses `DeleteBehavior.NoAction`. Confirmed against the applied migration: `FK_supplierquotecopies_purchaserequests_PurchaseRequestId` is `ON DELETE CASCADE`; `FK_purchaserequests_supplierquotecopies_SelectedSupplierCopyId` has no `onDelete` at all, which EF/SQL Server both default to `NO ACTION`.

**A second, related gotcha found while generating the first migration**: for a many-to-one relationship where the "one" side has a real collection navigation (e.g. `Supplier.RequestQuotations`), the many side's `.HasOne(x => x.Supplier)` must pair with `.WithMany(x => x.RequestQuotations)` — a named inverse — not an unnamed `.WithMany()`. An unnamed `.WithMany()` does not bind to the real collection; EF Core instead auto-discovers a *second*, unconfigured relationship for that collection and creates a phantom shadow FK column (`SupplierId1`) alongside the real `SupplierId`. This was caught by EF's own model-validation warning during `dotnet ef migrations add` and fixed in `PurchaseOrderConfiguration.cs` and `RequestQuotationConfiguration.cs` (Sections 9 and 12 below). Every relationship in this document now names both navigations explicitly wherever a real one exists on both sides.

---

## 2. UserConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `users` |
| Primary key | `Id` |
| Notable columns | `Name` (150), `Email` (255), `Role` → string (30), `Department` (100), `IsActive` default `true`, `PasswordHash` (500) |
| Indexes | Unique on `Email` |
| Relationships | `PurchaseRequests` (1:many via `PurchaseRequest.RequesterId`, `Restrict`); `RQApprovals` (1:many via `RQApproval.ApproverId`, `Restrict`) |

`Name`/`Email`/`Department`/`PasswordHash` do not call `.IsRequired()` explicitly — not a gap. `User`'s corresponding C# properties are `required string` (non-nullable reference types with `<Nullable>enable</Nullable>` project-wide), which EF Core's own convention already maps to `NOT NULL` without an explicit call. Confirmed via the migration: all four columns are `nullable: false`.

---

## 3. SupplierConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `suppliers` |
| Primary key | `Id` |
| Notable columns | `Name` (150), `Email` (255), `Contact` (100, nullable), `CreditorCode` (50, nullable), `AccountStatus` → string (20), `RegistrationToken` (100, nullable), `PasswordHash` (500, nullable) |
| Indexes | Unique on `Email`; unique + filtered (`IS NOT NULL`) on `CreditorCode`; unique + filtered on `RegistrationToken` |
| Relationships | `RequestQuotations` (1:many via `RequestQuotation.SupplierId`, `Restrict`); `PurchaseOrders` (1:many via `PurchaseOrder.SupplierId`, `Restrict`) |

Both of `Supplier`'s relationships are also (re-)declared from the child side, in `RequestQuotationConfiguration.cs`/`PurchaseOrderConfiguration.cs` — see the Section 1 gotcha. Since both sides now name the same pair of navigations, EF Core merges them into one relationship rather than creating a duplicate; this is not a conflict.

---

## 4. ItemConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `items` |
| Primary key | `Id` |
| Notable columns | `Code` (50) — the AutoCount sync-matching key; `Name` (150); `Uom` (20); `RefPrice` — `decimal(18,2)` |
| Indexes | Unique on `Code` |
| Relationships | None. `PurchaseRequestDetail.ItemCode` is a plain string snapshot, not a foreign key to `Item`. |

**Bug found and fixed**: the file originally configured `x.Code` twice (a copy-paste error under the "// Item Name" comment) and never configured `x.Name` at all, so `Name` fell back to EF Core's default convention — `nvarchar(max)`, unbounded — instead of a real length. Caught while refreshing this document against the applied migration, fixed to `builder.Property(x => x.Name).HasMaxLength(150).IsRequired();`, and the database was regenerated (dropped and reapplied — no data existed yet, so no rename migration was needed).

---

## 5. PurchaseRequestConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `purchaserequests` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `Department` (100); `Status` → string (20), default `"Quoting"`; `PmRemarks` (1000, nullable); `CreditorCode` / `CreditorName` (50 / 150, nullable — snapshots, not live references); `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo` |
| Relationships | `Requester` → `User` via `RequesterId`, `Restrict`; `DecidedByUser` → `User?` via `DecidedByUserId`, `Restrict`; `SelectedSupplierCopy` → `SupplierQuoteCopy?` via `SelectedSupplierCopyId`, **`NoAction`** (Section 1); `Items` → `PurchaseRequestDetail` (1:many, `Cascade`); `SupplierQuoteCopies` (1:many, `Cascade`) |

**The cardinality question flagged in earlier versions of this document is resolved.** `PurchaseRequest.RequestQuotations` is a single nullable `RequestQuotation?` navigation, not a collection — it does **not** get a `.HasMany(...)` block here at all. The relationship is configured entirely from the other side, in `RequestQuotationConfiguration.cs` (Section 9), which owns the `.HasForeignKey<RequestQuotation>(...)` call a one-to-one relationship requires. An earlier, stale `.HasMany(x => x.RequestQuotations)` block in this file (left over from before the entity was fixed to a single nav) caused a real compile error — `HasMany` cannot bind to a non-enumerable property — and was removed rather than reconfigured, since the relationship was already complete on the `RequestQuotation` side.

---

## 6. PurchaseRequestDetailConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `purchaserequestdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500, nullable); `Location` (200, nullable); `Uom` (20); `Qty` — `decimal(18,3)` |
| Indexes | None beyond the FK |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, `Cascade` (a detail line is owned by its request) |

The inverse side (`SupplierQuoteDetail.PurchaseRequestItemId` referencing this line) is configured from `SupplierQuoteDetailConfiguration.cs` — see Section 8.

---

## 7. SupplierQuoteCopyConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `supplierquotecopies` |
| Primary key | `Id` |
| Notable columns | `Token` (100); `Status` → string (20), default `"Pending"`; `Remarks` (1000, nullable); `TotalAmount` — `decimal(18,2)`; `SentAt` / `SubmittedAt` — `datetime2` (`SubmittedAt` nullable) |
| Indexes | Unique on `Token` (not nullable, so no filter needed) |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, **`Cascade`**, named inverse `.WithMany(x => x.SupplierQuoteCopies)`; `Supplier` via `SupplierId`, `Restrict`, unnamed `.WithMany()` |

This is the "owned" side of the cycle described in Section 1 — its cascade toward `PurchaseRequest` is what forces `PurchaseRequest.SelectedSupplierCopyId` to be `NoAction` rather than `Cascade`. Unlike the `Supplier` relationships on `RequestQuotation`/`PurchaseOrder`, the `Supplier` relationship here legitimately uses an unnamed `.WithMany()` — `Supplier` has no `SupplierQuoteCopies` collection navigation to bind to, so there is nothing to name.

---

## 8. SupplierQuoteDetailConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `supplierquotedetails` |
| Primary key | `Id` |
| Notable columns | `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond FKs |
| Relationships | `SupplierQuoteCopy` via `SupplierQuoteCopyId`, `Cascade`, named inverse `.WithMany(x => x.QuotedDetail)`; `PurchaseRequestDetail` via `PurchaseRequestItemId`, `Restrict`, named inverse `.WithMany(x => x.QuotedBy)` |

The second relationship targets `PurchaseRequestDetail` (the correct entity, matching the `PurchaseRequestItemId` foreign key name) — an earlier version of this document flagged this as depending on a Domain-layer type fix; that fix has landed and is confirmed by the applied migration's `FK_supplierquotedetails_purchaserequestdetails_PurchaseRequestItemId` referencing `purchaserequestdetails`.

---

## 9. RequestQuotationConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `requestquotations` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `TotalAmount` — `decimal(18,2)`; `Status` → string (20); `RequiresL2` — `bool`; `CreatedAt` — `datetime2`; `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo`; **unique on `PurchaseRequestId`** |
| Relationships | `PurchaseRequest` via `PurchaseRequestId`, `Restrict`, `.HasForeignKey<RequestQuotation>(...)`; `Supplier` via `SupplierId`, `Restrict`, named inverse `.WithMany(x => x.RequestQuotations)`; `Items` → `RequestQuotationDetail` (1:many, `Cascade`); `Approvals` → `RQApproval` (1:many, `Cascade`); `PurchaseOrder` |

**Both cardinality questions flagged in earlier versions of this document are resolved.** `PurchaseRequest.RequestQuotations` and `RequestQuotation.PurchaseOrder` are both single nullable navigations now, and both relationships are database-enforced 1:0-or-1 via real unique indexes (`IX_requestquotations_PurchaseRequestId` here; `IX_purchaseorders_RequestQuotationId`, Section 12). The `PurchaseRequest` relationship is configured with `.HasOne(x => x.PurchaseRequest).WithOne(x => x.RequestQuotations).HasForeignKey<RequestQuotation>(x => x.PurchaseRequestId)` — the explicit `HasForeignKey<TDependent>` generic argument is required for one-to-one relationships, since EF Core cannot infer which side owns the FK from a symmetric `HasOne().WithOne()` chain alone.

The `Supplier` relationship here originally used an unnamed `.WithMany()` — the Section 1 shadow-FK bug (`SupplierId1`). Fixed to `.WithMany(x => x.RequestQuotations)`.

---

## 10. RequestQuotationDetailConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `requestquotationdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500); `Uom` (20); `Qty` — `decimal(18,3)`; `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond the FK |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Cascade`, named inverse `.WithMany(x => x.Items)` |

---

## 11. RQApprovalConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `rqapprovals` |
| Primary key | `Id` |
| Notable columns | `Level` → string (10); `Action` → string (20); `Remark` (1000, nullable); `Timestamp` — `datetime2` |
| Indexes | None required by any current use case |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Cascade`, named inverse `.WithMany(x => x.Approvals)`; `Approver` → `User` via `ApproverId`, `Restrict`, named inverse `.WithMany(x => x.RQApprovals)` |

---

## 12. PurchaseOrderConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `purchaseorders` |
| Primary key | `Id` |
| Notable columns | `DocNo` (50); `TotalAmount` — `decimal(18,2)`; `Status` → string (20); `IsNewSupplier` — `bool`; `CreatedAt` / `SyncedAt` — `datetime2` (`SyncedAt` nullable); `AutoCountPORef` / `AutoCountCreditorRef` (100, nullable); `SyncError` (2000, nullable); `SyncAttempts` — `int`, default `0`; `RowVersion` via `.IsRowVersion()` |
| Indexes | Unique on `DocNo`; **unique on `RequestQuotationId`** — this is what actually enforces "one RQ converts to at most one PO" at the database level |
| Relationships | `RequestQuotation` via `RequestQuotationId`, `Restrict`, `.HasForeignKey<PurchaseOrder>(...)`, named inverse `.WithOne(x => x.PurchaseOrder)`; `Supplier` via `SupplierId`, `Restrict`, named inverse `.WithMany(x => x.PurchaseOrders)`; `Items` → `PurchaseOrderDetail` (1:many, `Cascade`) |

Same shadow-FK bug as `RequestQuotationConfiguration.cs` (Section 1), same fix: the `Supplier` relationship originally used an unnamed `.WithMany()` and was changed to `.WithMany(x => x.PurchaseOrders)`.

---

## 13. PurchaseOrderDetailConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `purchaseorderdetails` |
| Primary key | `Id` |
| Notable columns | `ItemCode` (50); `Description` (500); `Uom` (20); `Qty` — `decimal(18,3)`; `UnitPrice` — `decimal(18,2)` |
| Indexes | None beyond the FK |
| Relationships | `PurchaseOrder` via `PurchaseOrderId`, `Cascade`, named inverse `.WithMany(x => x.Items)` |

---

## 14. ApprovalSettingConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `approvalsettings` |
| Primary key | `Level` — **not `Id`**. `ApprovalSetting` does not implement `IEntity`; its natural key is the `ApprovalLevel` value itself (only two rows ever exist: `L1`, `L2`). |
| Notable columns | `Level` → string (10, matches the primary key); `ApproverRole` → string (30); `MinAmount` — `decimal(18,2)`; `MaxAmount` — `decimal(18,2)`, nullable (`null` = no upper bound) |
| Indexes | None beyond the primary key |
| Relationships | None. `ApproverRole` is a `Role` enum value, not a foreign key to any specific `User` row — the actual approver at runtime is resolved by matching a logged-in `User.Role` against this value, not by a database relationship. |

This table's two rows are meant to be populated by `ApplicationDbContextSeed.cs`, which is still an unimplemented scaffold — see `Infrastructure.md` Known Issues, item 6. The table itself exists and is correctly shaped; it simply has no rows in it yet on a fresh database.

---

## 15. CounterConfiguration

**Status: implemented.**

| Aspect | Design |
|---|---|
| Table | `counters` |
| Primary key | `Name` — a string (`"PR"`, `"RQ"`, `"PO"`), not `Id`. `Counter` implements no marker interface at all. |
| Notable columns | `Name` (10); `Seq` — `int`, default `0` |
| Indexes | None beyond the primary key |
| Relationships | None |

No `RowVersion` and no optimistic concurrency here. The actual implementation (`Persistence/DocumentNumbers/SequentialDocumentNumberGenerator.cs`, documented in `Infrastructure.md` Section 1 and `Repositories.md`) ended up using a raw `UPDATE counters SET Seq = Seq + 1 OUTPUT INSERTED.Seq WHERE Name = @prefix` via `Database.SqlQuery<int>(...)`, rather than the `ExecuteUpdateAsync` originally anticipated here — both approaches are atomic under concurrent access without a concurrency token, but `SqlQuery` was chosen so the increment bypasses the change tracker entirely and can never prematurely flush unrelated pending changes on a `DbContext` shared with the calling Handler for the rest of its request.
