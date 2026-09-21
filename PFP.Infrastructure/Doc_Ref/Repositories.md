# Repositories — Design Reference

**One section per repository, covering every implementation under `Persistence/Repositories/`.** As of this writing only `UserRepository` and `SupplierRepository` have real content; the other six are documented here as the plan to implement against — see `Infrastructure.md` for overall project status.

Each repository implements the interface of the same name declared in `PFP.Application/Abstractions/Persistence/`. This document assumes the conventions below and does not repeat them per repository; only what is specific to each one is called out.

---

## Table of Contents

1. [Conventions Shared By Every Repository](#1-conventions-shared-by-every-repository)
2. [UserRepository](#2-userrepository) — implemented
3. [SupplierRepository](#3-supplierrepository) — implemented
4. [ItemRepository](#4-itemrepository) — planned
5. [ApprovalSettingRepository](#5-approvalsettingrepository) — planned
6. [PurchaseRequestRepository](#6-purchaserequestrepository) — planned
7. [SupplierQuoteRepository](#7-supplierquoterepository) — planned
8. [RequestQuotationRepository](#8-requestquotationrepository) — planned
9. [PurchaseOrderRepository](#9-purchaseorderrepository) — planned

---

## 1. Conventions Shared By Every Repository

| Rule | Why |
|---|---|
| `internal sealed class`, primary constructor over `ApplicationDbContext` | Nothing outside `PFP.Infrastructure` should ever hold a concrete repository type — only the interface, resolved through DI. |
| `CancellationToken` is a required parameter, never `= default` | Every caller is a `Features/` Handler, and `IRequestHandler.Handle` already hands the Handler a real token with no default of its own. Making the repository parameter optional buys no real convenience here and makes it easier to silently drop cancellation on one call. |
| List-returning methods return `Task<IReadOnlyList<T>>`, not `Task<List<T>>` | Signals a read-only view to the caller and does not leak the concrete collection type. |
| No `Update(T entity)` method, on any repository | A `Handler` loads a tracked entity via a `GetByIdAsync`-style method, mutates its properties, and calls `IUnitOfWork.SaveChangesAsync()`. EF Core's change tracker generates the correct `UPDATE` from that alone. An explicit `Update()` is easy to implement as a full-entity overwrite (`DbSet<T>.Update()` marks every column modified, not just the changed ones), which risks clobbering data with a partially-populated object. |
| No `Remove(T entity)` method unless a real use case calls for one | None of the nine `Features/` modules currently has a hard-delete use case — see the per-repository notes below for why, entity by entity. |
| A single-entity lookup used by a mutating Handler stays tracked (no `.AsNoTracking()`) | The Handler needs the tracked instance to mutate and save it without a second round trip. |
| A list-returning method used only by Query handlers gets `.AsNoTracking()` and an explicit `.OrderBy(...)` | No tracking overhead for read-only paths; an explicit order keeps the result stable across calls (SQL Server does not guarantee row order without one). |
| `GetByIdAsync` on an aggregate root with child collections adds `.Include(...)` for those collections | The Handler expects the full aggregate; without it, the child collections would silently come back empty. |

---

## 2. UserRepository

**Status: implemented.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked | Loading a `User` a Handler is about to mutate (`UpdateUserRole`, `ActivateUser`, `DeactivateUser`), or a plain lookup (`GetUserById`) |
| `GetByEmailAsync(string email, ct)` | Tracked | `Login` (credential check) and `CreateUser` (uniqueness check before insert) |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `Name` | `GetUsers` |
| `Add(User user)` | — (synchronous, no I/O until `SaveChangesAsync`) | `CreateUser` |

No `Remove`: user accounts are disabled via `DeactivateUser` (`IsActive = false`), never deleted — `UserConfiguration`'s `Restrict` delete behavior on `PurchaseRequests`/`RQApprovals` would reject a hard delete for any user with history anyway, so the method would be unusable for exactly the users someone would want to remove.

---

## 3. SupplierRepository

**Status: implemented.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked | Plain lookup (`GetSupplierById`) |
| `GetByEmailAsync(string email, ct)` | Tracked | `Login` — suppliers authenticate through the same endpoint as internal users |
| `GetByRegistrationTokenAsync(string token, ct)` | Tracked | `CompleteSupplierRegistration` — the Handler loads the supplier by token, sets the password hash, flips `AccountStatus` to `Registered`, and clears the token, all on the same tracked instance |
| `GetByCreditorCodeAsync(string creditorCode, ct)` | Tracked | `SyncSuppliersFromAutoCount`'s upsert — this is the sync-matching key described in `Domain.md` |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `Name` | `GetSuppliers` |
| `Add(Supplier supplier)` | — | `CreateSupplier` |

No `Remove`: same reasoning as `User` — a supplier account is suspended (`AccountStatus = Suspended`), never deleted, and would be rejected by the `Restrict` delete behavior on `RequestQuotations`/`PurchaseOrders` for any supplier with history.

---

## 4. ItemRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked | Plain lookup (`GetItemById`) |
| `GetByCodeAsync(string code, ct)` | Tracked | `SyncItemsFromAutoCount`'s upsert — matches on `Item.Code`, the sync key |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `Name` | `GetItems` — the catalog a Requester picks from when raising a PR |
| `Add(Item item)` | — | Called from inside `SyncItemsFromAutoCountCommandHandler` for codes not seen before |

No `Update`/`Remove` beyond the shared convention note: items are pulled from AutoCount, never hand-edited through this system (Scope Document, clause G) — the upsert inside `SyncItemsFromAutoCount` is a mutate-tracked-entity operation exactly like every other update in this codebase, not a separate repository method.

---

## 5. ApprovalSettingRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByLevelAsync(ApprovalLevel level, ct)` | Tracked | `UpdateApprovalSettings` (load, mutate `MinAmount`/`MaxAmount`/`ApproverRole`, save); also used inside `ApproveRequestQuotation`/`RejectRequestQuotation` to resolve the data-driven `ApproverRole` for the authorization check described in `Application.md`'s Coding Conventions |
| `GetAllAsync(ct)` | `AsNoTracking` | `GetApprovalSettings` — displays both `L1` and `L2` rows together |

No `Add`, no `Remove`: exactly two rows (`L1`, `L2`) ever exist, created once by `ApplicationDbContextSeed.cs`. There is deliberately no "create an approval setting" use case.

---

## 6. PurchaseRequestRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked, `.Include(x => x.Items).Include(x => x.SupplierQuoteCopies)` | `ApprovePurchaseRequest`/`RejectPurchaseRequest` need the full aggregate — approval has to validate that `selectedSupplierCopyId` belongs to this PR and has `Status = Submitted`, which means the `SupplierQuoteCopies` collection must already be loaded |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `CreatedAt` descending | `GetPurchaseRequests` — a list view does not need every line item and quote copy loaded for every row; that would be an expensive query for no benefit |
| `Add(PurchaseRequest purchaseRequest)` | — | `CreatePurchaseRequest` |

`GetPurchaseRequests` returning `Requester`'s own PRs only, versus everyone's, per the API surface's access rule, is filtering logic that belongs in the Query Handler (it depends on `ICurrentUserService`, not on anything this repository should know about) — this repository's `GetAllAsync` returns everything and lets the Handler decide what the caller is allowed to see.

---

## 7. SupplierQuoteRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByTokenAsync(string token, ct)` | Tracked, `.Include(x => x.PurchaseRequest)` | Both the no-authentication view endpoint and `SubmitSupplierQuote` — the latter mutates `Status`/`TotalAmount`/`SubmittedAt` on this same tracked instance, and both need the parent `PurchaseRequest` loaded to show the supplier what they are quoting on |
| `GetByIdAsync(int id, ct)` | Tracked | Used from `ApprovePurchaseRequestCommandHandler` to re-validate the `selectedSupplierCopyId` independently, if not already available via the `PurchaseRequest.SupplierQuoteCopies` collection loaded by `PurchaseRequestRepository` |
| `GetBySupplierIdAsync(int supplierId, ct)` | `AsNoTracking`, ordered by `SentAt` descending | `GetSupplierQuoteHistory` (`/suppliers/me/quotes`) |
| `Add(SupplierQuoteCopy supplierQuoteCopy)` | — | Called up to three times inside `CreatePurchaseRequestCommandHandler`, once per supplier the PR is distributed to |

No `GetAllAsync`: there is no use case anywhere in this system for "every quote copy across every supplier" as a single list — see `Application.md`'s Coding Conventions for the general rule that a repository method only exists when a real use case calls for it.

---

## 8. RequestQuotationRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked, `.Include(x => x.Items).Include(x => x.Approvals)` | `ApproveRequestQuotation`/`RejectRequestQuotation`/`ConvertToPurchaseOrder` all need the approval trail loaded to determine the current stage (`PendingL1` vs `PendingL2`) and validate the `level` argument against it |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `CreatedAt` descending | `GetRequestQuotations` |
| `Add(RequestQuotation requestQuotation)` | — | `CreateFromApprovedPR` (internal) |

---

## 9. PurchaseOrderRepository

**Status: planned.**

| Method | Tracking | Used for |
|---|---|---|
| `GetByIdAsync(int id, ct)` | Tracked, `.Include(x => x.Items)` | `SyncPurchaseOrderToAutoCount` needs the line items to build the `AutoCountPurchaseOrderRequest` payload described in `Application.md`'s Integrations section |
| `GetAllAsync(ct)` | `AsNoTracking`, ordered by `CreatedAt` descending | `GetPurchaseOrders` |
| `Add(PurchaseOrder purchaseOrder)` | — | `CreateFromRequestQuotation` (internal) |

No `Remove`: a `PurchaseOrder` is never deleted once created — a failed AutoCount sync is recorded in-band via `Status`/`SyncError`/`SyncAttempts` (see `Application.md`'s Architectural Decisions for why this is a `Result<T>` case, not an exception), not by removing the row and starting over.
