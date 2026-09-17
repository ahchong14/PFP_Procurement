# PFP.Domain

**Domain layer of the PFP.Procurement system.** Pure data structures — no framework references, no NuGet packages, no knowledge of `PFP.Application`, `PFP.Infrastructure`, or `PFP.WebApi`.

| | |
|---|---|
| Project | `PFP.Domain` |
| Depends on | Nothing |
| Depended on by | `PFP.Application` (and, transitively, everything above it) |
| Status | 14 entities, 9 enums, 6 marker interfaces — all present and building |
| Audience | Backend maintainers, frontend integrators, incoming contributors |

Application-layer architecture (Behaviours, Repositories, request pipeline) is documented separately in `PFP.Application/Application.md` and is not repeated here.

---

## Table of Contents

1. [Design Principles](#1-design-principles)
2. [Directory Structure](#2-directory-structure)
3. [Enum Reference](#3-enum-reference)
4. [Entity Reference](#4-entity-reference)
5. [Marker Interfaces](#5-marker-interfaces)
6. [Known Issues](#6-known-issues)

---

## 1. Design Principles

- **Anemic model.** Every entity is data only — no methods, no embedded business rules. A state-machine rule such as "can a `PurchaseRequest` move from `Quoting` to `PmReview`" is not implemented on the entity; it lives in the corresponding Handler under `PFP.Application/Features/`.
- **Zero framework dependency.** No EF Core, no ORM, no web-framework reference. This keeps the layer reusable across any consumer and unit-testable without a database.
- **Marker interfaces.** Capabilities such as "has an integer id" or "is exposed via a Dto" are expressed through empty-bodied interfaces (`IEntity`, `IExposableEntity`) rather than `is`-checks scattered through calling code.

---

## 2. Directory Structure

```
PFP.Domain/
├── PFP.Domain.csproj
├── Entities/
│   ├── Commons/
│   │   ├── Users/User.cs
│   │   └── Suppliers/Supplier.cs
│   ├── Items/Item.cs
│   ├── PurchaseRequests/
│   │   ├── PurchaseRequest.cs
│   │   └── PurchaseRequestDetail.cs
│   ├── SupplierQuoteCopys/
│   │   ├── SupplierQuoteCopy.cs
│   │   └── SupplierQuoteDetail.cs
│   ├── RequestQuotations/
│   │   ├── RequestQuotation.cs
│   │   ├── RequestQuotationDetail.cs
│   │   └── RQApproval.cs
│   ├── PurchaseOrders/
│   │   ├── PurchaseOrder.cs
│   │   └── PurchaseOrderDetail.cs
│   ├── ApprovalSetting.cs
│   └── Counter.cs
├── Enums/
│   ├── Role.cs
│   ├── PRStatus.cs
│   ├── CopyStatus.cs
│   ├── RQStatus.cs
│   ├── ApprovalLevel.cs
│   ├── ApprovalAction.cs
│   ├── POStatus.cs
│   ├── SupplierAccountStatus.cs
│   └── DocumentType.cs
└── Interface/
    ├── IEntity.cs
    ├── IExposableEntity.cs
    ├── IBaseEntity.cs
    ├── IBaseExposableEntity.cs
    ├── Auditables/ICreationAuditable.cs
    └── Concurrency/IConcurrencyAware.cs
```

> There is no `Department.cs` in `Enums/`. `Department` is not currently modeled as an enum anywhere in this project — see "Known Issues" (below), item 6.

---

## 3. Enum Reference

Enum values serialize to JSON as strings (e.g. `"role": "HeadOfPurchase"`), not integers.

| Enum | Values | Used on |
|---|---|---|
| `Role` | `Requester`, `PurchaseManager`, `DirectorL1`, `DirectorL2`, `HeadOfPurchase` | `User.Role`, `ApprovalSetting.ApproverRole` |
| `PRStatus` | `Quoting`, `PmReview`, `Approved`, `Rejected`, `Converted` | `PurchaseRequest.Status` |
| `Copystatus` (note: spelled with lowercase "s", not `CopyStatus` -- see Known Issues item 4) | `Pending`, `Submitted` | `SupplierQuoteCopy.Status` -- one-way, non-reversible |
| `RQStatus` | `PendingL1`, `PendingL2`, `Approved`, `Rejected`, `Converted` | `RequestQuotation.Status` |
| `ApprovalLevel` | `L1`, `L2` | `RQApproval.Level`, `ApprovalSetting.level` |
| `ApprovalAction` | `Approved`, `Rejected` | `RQApproval.Action` |
| `POStatus` | `Created`, `Synced`, `SyncFailed` | `PurchaseOrder.Status` |
| `SupplierAccountStatus` | `Invited`, `Registered`, `Suspended` | `Supplier.AccountStatus` |
| `DocumentType` | `PurchaseRequest`, `RequestQuotation`, `PurchaseOrder` | `IDocumentNumberGenerator` (Application layer); not typically consumed by clients |

---

## 4. Entity Reference

### `User` — internal account

| Field | Type |
|---|---|
| Id | int |
| Name | string |
| Email | string |
| Role | Role |
| Department | string (required) |
| IsActive | bool, default `true` |
| PasswordHash | string |
| PurchaseRequests | ICollection\<PurchaseRequest\> (as Requester) |
| RQApprovals | ICollection\<RQApproval\> (as Approver) |

### `Supplier` — supplier account

| Field | Type |
|---|---|
| Id | int (required — see the corresponding section below, item 7) |
| Name | string |
| Email | string |
| Contact | string? |
| InAutoCount | bool |
| CreditorCode | string? — AutoCount creditor code, used as the sync-matching key |
| AccountStatus | SupplierAccountStatus, default `Invited` |
| RegistrationToken | string? |
| RegisteredAt | DateTime? |
| PasswordHash | string? |
| RequestQuotations | ICollection\<RequestQuotation\> |
| PurchaseOrders | ICollection\<PurchaseOrder\> |

### `Item` — material master data

| Field | Type |
|---|---|
| Id | int |
| Code | string — unique, AutoCount sync-matching key |
| Name | string |
| Uom | string |
| RefPrice | decimal |

### `PurchaseRequest`

| Field | Type |
|---|---|
| Id | int |
| DocNo | string |
| RequestedId | int — should read `RequesterId`, see the corresponding section below, item 1 |
| Requester | User |
| Department | string |
| Status | PRStatus, default `Quoting` |
| PmRemarks | string? |
| SelectedSupplierCopyId | int? |
| SelectedSupplierCopy | SupplierQuoteCopy? |
| CreditorCode / CreditorName | string? |
| CreatedAt / DecidedAt | DateTime |
| DecidedByUserId | int? |
| DecidedByUser | User? |
| RowVersion | byte[] |
| Items | ICollection\<PurchaseRequestDetail\> |
| SupplierQuoteCopies | ICollection\<SupplierQuoteCopy\> |
| RequestQuotations | ICollection\<RequestQuotation\> |

### `PurchaseRequestDetail`

| Field | Type |
|---|---|
| Id | int |
| PurchaseRequestId | int |
| PurchaseRequest | PurchaseRequest |
| ItemCode | string |
| Description | string? |
| Location | string? |
| Uom | string |
| Qty | decimal |
| QuotedBy | ICollection\<SupplierQuoteDetail\> |

### `SupplierQuoteCopy` — per-supplier RFQ distribution

| Field | Type |
|---|---|
| Id | int |
| PurchaseRequestId / PurchaseRequest | int / PurchaseRequest |
| SupplierId / Supplier | int / Supplier |
| Token | string — unguessable access credential |
| Status | Copystatus, default `Pending` |
| Remarks | string? |
| TotalAmount | decimal |
| SentAt | DateTime |
| SubmittedAt | DateTime? |
| QuotedDetail | ICollection\<SupplierQuoteDetail\> |

### `SupplierQuoteDetail` — submitted quote line

| Field | Type |
|---|---|
| Id | int |
| SupplierQuoteCopyId / supplierQuoteCopy | int / SupplierQuoteCopy |
| PurchaseRequestItemId | int |
| purchaseRequest | PurchaseRequest — type does not match the foreign key, see the corresponding section below, item 2 |
| UnitPrice | decimal |

### `RequestQuotation`

| Field | Type |
|---|---|
| Id | int |
| DocNo | string |
| PurchaseRequestId / PurchaseRequest | int / PurchaseRequest |
| SupplierId / Supplier | int / Supplier |
| TotalAmount | decimal |
| Status | RQStatus |
| RequiresL2 | bool |
| CreatedAt | DateTime |
| RowVersion | byte[] |
| Items | ICollection\<RequestQuotationDetail\> |
| Approvals | ICollection\<RQApproval\> |
| PurchaseOrder | ICollection\<PurchaseOrder\> — relationship is 1:1, collection type is incorrect; see the corresponding section below, item 3 |

### `RequestQuotationDetail` — snapshot

| Field | Type |
|---|---|
| Id | int |
| RequestQuotationId / RequestQuotation | int / RequestQuotation |
| ItemCode / Description / Uom | string |
| Qty / UnitPrice | decimal |

### `RQApproval` — approval/rejection audit record

| Field | Type |
|---|---|
| Id | int |
| RequestQuotationId / RequestQuotation | int / RequestQuotation |
| Level | ApprovalLevel |
| ApproverId / Approver | int / User |
| Action | ApprovalAction |
| Remark | string? |
| Timestamp | DateTime |

### `PurchaseOrder`

| Field | Type |
|---|---|
| Id | int |
| DocNo | string |
| RequestQuotationId / RequestQuotation | int / RequestQuotation (1:1) |
| SupplierId / Supplier | int / Supplier |
| IsNewSupplier | bool |
| TotalAmount | decimal |
| Status | POStatus |
| CreatedAt | DateTime |
| SyncedAt | DateTime? |
| AutoCountPORef / AutoCountCreditorRef | string? |
| SyncError | string? |
| SyncAttempts | int |
| RowVersion | byte[] |
| Items | ICollection\<PurchaseOrderDetail\> |

### `PurchaseOrderDetail` — snapshot

| Field | Type |
|---|---|
| Id | int |
| PurchaseOrderId | int |
| PurchaseOrder | PurchaseOrder |
| ItemCode / Description / Uom | string |
| Qty / UnitPrice | decimal |

### `ApprovalSetting` — exactly two records (L1, L2)

| Field | Type |
|---|---|
| level | ApprovalLevel — lowercase, see the corresponding section below, item 5 |
| ApproverRole | Role |
| MinAmount | decimal |
| MaxAmount | decimal? |

### `Counter` — document-number sequence generator

| Field | Type |
|---|---|
| Name | string (primary key — `"PR"` / `"RQ"` / `"PO"`) |
| Seq | int |

---

## 5. Marker Interfaces

| Interface | Definition | Implemented by |
|---|---|---|
| `IEntity` | `int Id { get; }` | Most entities. `ApprovalSetting` (keyed by `Level`) and `Counter` (keyed by `Name`) do not implement it. |
| `IExposableEntity` | Empty — marks "mapped to a Dto for output" | All entities except `Counter`. |
| `IBaseEntity` | `IEntity` + `ICreationAuditable` | Entities that also track a creation timestamp. |
| `IBaseExposableEntity` | `IBaseEntity` + `IExposableEntity` | Same set as above. |
| `ICreationAuditable` (`Auditables/`) | `DateTime CreatedAt { get; }` | Entities with a creation timestamp. |
| `IConcurrencyAware` (`Concurrency/`) | `byte[] RowVersion { get; set; }` | `PurchaseRequest`, `RequestQuotation`, `PurchaseOrder` — the three core document entities requiring optimistic concurrency control. |

These interfaces let `PFP.Application` write generic infrastructure code — e.g. a `GetOrThrowAsync<T>` helper constrained to `T : IEntity`, or an optimistic-concurrency retry helper constrained to `T : IConcurrencyAware` — without writing that logic per entity type.

---

## 6. Known Issues

Discrepancies found while cross-checking this document against the real source files. None currently prevent the solution from building; they are listed here so they are not rediscovered independently and so they can be prioritized deliberately.

| # | Location | Issue | Impact |
|---|---|---|---|
| 1 | `PurchaseRequest.RequestedId` | Should read `RequesterId` — the current name does not match the paired navigation property `Requester` | Cosmetic; no functional impact |
| 2 | `SupplierQuoteDetail.purchaseRequest` | Typed as `PurchaseRequest`, but the paired foreign key is `PurchaseRequestItemId`; the navigation property should be typed `PurchaseRequestDetail` | Type/FK mismatch — a source of confusion when this relationship is configured in EF Core |
| 3 | `RequestQuotation.PurchaseOrder` | Typed as `ICollection<PurchaseOrder>`, but `PurchaseOrder.RequestQuotationId` establishes a 1:1 relationship (one RQ converts to at most one PO) | Should be `PurchaseOrder?` |
| 4 | `Enums/CopyStatus.cs` | The type is declared as `Copystatus` (lowercase "s"), inconsistent with the PascalCase convention used by `PRStatus`/`RQStatus`/`POStatus` | Naming convention only |
| 5 | `ApprovalSetting.level` | Property name starts lowercase, the only property in the codebase that does not follow PascalCase | Naming convention only |
| 6 | `User.Department`, `PurchaseRequest.Department` | Both are plain `string`; there is no `Department` enum in this project despite `Department` appearing as a documented business concept in the original scope reconciliation | If a frontend expects a fixed set of department values, the backend currently performs no such validation |
| 7 | `Supplier.Id` | Declared `required int Id` | Forces every `new Supplier { ... }` construction to explicitly assign `Id`, even though it is a database-generated identity value; inconsistent with `User.Id`/`PurchaseRequest.Id`, which are plain `int` |

Items 1–3 involve changing a field type or name and may have downstream impact once the corresponding `Features/` modules are implemented. Items 4–7 are lower-risk, isolated changes.
