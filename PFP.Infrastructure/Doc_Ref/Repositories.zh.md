# Repository 设计参考

**每个Repository一节，覆盖`Persistence/Repositories/`下的每一个实现。** 写这份文档的时候，只有`UserRepository`和`SupplierRepository`真的有内容，其余6个记录的是"该照这个方案去实现"的计划——整个项目的状态见`Infrastructure.zh.md`。

每个Repository都实现`PFP.Application/Abstractions/Persistence/`里同名的接口。本文档假设你已经了解下面这批通用规范，每个Repository那一节不会重复讲一遍，只讲这个Repository特有的东西。

---

## 目录

1. [所有Repository共用的规范](#1-所有repository共用的规范)
2. [UserRepository](#2-userrepository)——已实现
3. [SupplierRepository](#3-supplierrepository)——已实现
4. [ItemRepository](#4-itemrepository)——待实现
5. [ApprovalSettingRepository](#5-approvalsettingrepository)——待实现
6. [PurchaseRequestRepository](#6-purchaserequestrepository)——待实现
7. [SupplierQuoteRepository](#7-supplierquoterepository)——待实现
8. [RequestQuotationRepository](#8-requestquotationrepository)——待实现
9. [PurchaseOrderRepository](#9-purchaseorderrepository)——待实现

---

## 1. 所有Repository共用的规范

| 规则 | 为什么 |
|---|---|
| `internal sealed class`，主构造函数接收`ApplicationDbContext` | `PFP.Infrastructure`之外的任何代码都不该拿到具体的Repository类型——只能通过接口，靠DI解析出来。 |
| `CancellationToken`是必填参数，不给`= default` | 每一个调用方都是`Features/`里的Handler，`IRequestHandler.Handle`本身就已经给了Handler一个真实的token，它自己也没有默认值。Repository的参数给可选值，在这个项目里换不来任何真实的便利，反而更容易在某一次调用里悄悄漏传、丢掉取消信号。 |
| 返回列表的方法用`Task<IReadOnlyList<T>>`，不用`Task<List<T>>` | 告诉调用方这是一份只读视图，也不暴露内部具体用的是哪种集合类型。 |
| 任何Repository都没有`Update(T entity)`方法 | Handler通过`GetByIdAsync`这类方法拿到追踪中的实体，直接改属性，调`IUnitOfWork.SaveChangesAsync()`，EF Core的变更追踪单靠这一步就能生成正确的`UPDATE`语句。显式的`Update()`容易被实现成整个实体全字段覆盖（`DbSet<T>.Update()`会把所有列都标记成"已修改"，不只是真的改动过的那些），有用不完整的对象覆盖已有数据的风险。 |
| 没有真实用例需要的话，不加`Remove(T entity)`方法 | 现在九个`Features/`模块里没有一个是"物理删除"的用例——具体到每个实体为什么不需要，见下面各自的小节。 |
| 被某个会修改数据的Handler使用的单条查询，保持追踪状态（不加`.AsNoTracking()`） | Handler需要这个追踪中的实例去改属性、存盘，不需要再多查一次。 |
| 只被Query使用的列表查询，加`.AsNoTracking()`，并且显式加`.OrderBy(...)` | 只读路径不需要追踪开销；显式排序能保证结果顺序稳定（SQL Server在没有`ORDER BY`的情况下不保证行的顺序）。 |
| 聚合根的`GetByIdAsync`如果带子集合，加对应的`.Include(...)` | Handler期待拿到完整的聚合根，不加的话子集合会悄悄变成空的。 |

---

## 2. UserRepository

**状态：已实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪 | Handler要修改这个`User`时用（`UpdateUserRole`、`ActivateUser`、`DeactivateUser`），或者单纯查询（`GetUserById`） |
| `GetByEmailAsync(string email, ct)` | 追踪 | `Login`（校验凭证）、`CreateUser`（建号前查重） |
| `GetAllAsync(ct)` | `AsNoTracking`，按`Name`排序 | `GetUsers` |
| `Add(User user)` | ——（同步，没有真正的I/O，要等`SaveChangesAsync`） | `CreateUser` |

没有`Remove`：用户账号是靠`DeactivateUser`（`IsActive = false`）停用的，不会被删除——而且`UserConfiguration`里`PurchaseRequests`/`RQApprovals`两个关系都是`Restrict`删除行为，任何有历史记录的用户如果真的执行物理删除都会被数据库拒绝，这个方法对真正想删除的那批用户反而是用不了的。

---

## 3. SupplierRepository

**状态：已实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪 | 单纯查询（`GetSupplierById`） |
| `GetByEmailAsync(string email, ct)` | 追踪 | `Login`——供应商跟内部用户走同一个登录端点 |
| `GetByRegistrationTokenAsync(string token, ct)` | 追踪 | `CompleteSupplierRegistration`——Handler按token查出供应商，在同一个追踪中的实例上设密码哈希、把`AccountStatus`改成`Registered`、清空token |
| `GetByCreditorCodeAsync(string creditorCode, ct)` | 追踪 | `SyncSuppliersFromAutoCount`的upsert——这是`Domain.zh.md`里描述的那个同步匹配键 |
| `GetAllAsync(ct)` | `AsNoTracking`，按`Name`排序 | `GetSuppliers` |
| `Add(Supplier supplier)` | —— | `CreateSupplier` |

没有`Remove`：跟`User`同样的道理——供应商账号是被停用（`AccountStatus = Suspended`），不是被删除，任何有`RequestQuotations`/`PurchaseOrders`历史记录的供应商，`Restrict`删除行为也会拒绝物理删除。

---

## 4. ItemRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪 | 单纯查询（`GetItemById`） |
| `GetByCodeAsync(string code, ct)` | 追踪 | `SyncItemsFromAutoCount`的upsert——按`Item.Code`这个同步键匹配 |
| `GetAllAsync(ct)` | `AsNoTracking`，按`Name`排序 | `GetItems`——Requester建PR时选品用的目录 |
| `Add(Item item)` | —— | 在`SyncItemsFromAutoCountCommandHandler`内部，遇到之前没见过的编码时调用 |

除了通用规范提到的以外，没有`Update`/`Remove`：物料是从AutoCount拉取的，不会在这个系统里手动编辑（Scope文档G条）——`SyncItemsFromAutoCount`内部的upsert本质上跟这个代码库其它任何一次更新一样，是"改追踪中的实体属性再存盘"，不是一个单独的Repository方法。

---

## 5. ApprovalSettingRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByLevelAsync(ApprovalLevel level, ct)` | 追踪 | `UpdateApprovalSettings`（查出来、改`MinAmount`/`MaxAmount`/`ApproverRole`、存盘）；也被`ApproveRequestQuotation`/`RejectRequestQuotation`内部用来查出数据驱动的`ApproverRole`，做`Application.zh.md`"编码规范"里说的那种动态权限检查 |
| `GetAllAsync(ct)` | `AsNoTracking` | `GetApprovalSettings`——把`L1`和`L2`两条一起展示 |

没有`Add`，也没有`Remove`：永远只有两条记录（`L1`、`L2`），由`ApplicationDbContextSeed.cs`一次性建好。这个实体故意没有"新建一条审批设置"这个用例。

---

## 6. PurchaseRequestRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪，`.Include(x => x.Items).Include(x => x.SupplierQuoteCopies)` | `ApprovePurchaseRequest`/`RejectPurchaseRequest`需要完整的聚合根——审批要校验`selectedSupplierCopyId`确实属于这张PR、而且`Status = Submitted`，这意味着`SupplierQuoteCopies`这个集合必须已经被加载出来 |
| `GetAllAsync(ct)` | `AsNoTracking`，按`CreatedAt`倒序 | `GetPurchaseRequests`——列表页不需要每一行都带上全部明细和报价副本，那样查询代价很高但没有实际好处 |
| `Add(PurchaseRequest purchaseRequest)` | —— | `CreatePurchaseRequest` |

`GetPurchaseRequests`要不要按"`Requester`只看自己发起的"这条权限规则过滤，这个过滤逻辑该写在Query Handler里（因为它依赖`ICurrentUserService`，跟这个Repository该知道的事情无关）——这个Repository的`GetAllAsync`就是老老实实返回全部，由Handler决定调用者能看到哪些。

---

## 7. SupplierQuoteRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByTokenAsync(string token, ct)` | 追踪，`.Include(x => x.PurchaseRequest)` | 免登录查看端点和`SubmitSupplierQuote`都要用——后者要在同一个追踪中的实例上改`Status`/`TotalAmount`/`SubmittedAt`，两者都需要把父级`PurchaseRequest`一起加载出来，好让供应商看清楚自己在给哪张单子报价 |
| `GetByIdAsync(int id, ct)` | 追踪 | 如果`PurchaseRequestRepository`已经通过`PurchaseRequest.SupplierQuoteCopies`集合把数据带出来了，`ApprovePurchaseRequestCommandHandler`不一定需要单独调这个方法；留着是为了需要单独二次校验`selectedSupplierCopyId`的场景 |
| `GetBySupplierIdAsync(int supplierId, ct)` | `AsNoTracking`，按`SentAt`倒序 | `GetSupplierQuoteHistory`（`/suppliers/me/quotes`） |
| `Add(SupplierQuoteCopy supplierQuoteCopy)` | —— | 在`CreatePurchaseRequestCommandHandler`内部最多调用三次，一个供应商一次 |

没有`GetAllAsync`：这个系统里没有任何用例需要"查所有供应商的所有报价副本"这种全表列表——参照`Application.zh.md`"编码规范"里那条"Repository方法只在有真实用例需要时才加"的原则。

---

## 8. RequestQuotationRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪，`.Include(x => x.Items).Include(x => x.Approvals)` | `ApproveRequestQuotation`/`RejectRequestQuotation`/`ConvertToPurchaseOrder`都需要加载完整的审批记录，才能判断现在处于哪个阶段（`PendingL1`还是`PendingL2`），并且校验传进来的`level`参数对不对 |
| `GetAllAsync(ct)` | `AsNoTracking`，按`CreatedAt`倒序 | `GetRequestQuotations` |
| `Add(RequestQuotation requestQuotation)` | —— | `CreateFromApprovedPR`（内部专用） |

---

## 9. PurchaseOrderRepository

**状态：待实现。**

| 方法 | 追踪状态 | 用在哪 |
|---|---|---|
| `GetByIdAsync(int id, ct)` | 追踪，`.Include(x => x.Items)` | `SyncPurchaseOrderToAutoCount`需要明细行才能组装`Application.zh.md`"对接集成"那节说的`AutoCountPurchaseOrderRequest`请求体 |
| `GetAllAsync(ct)` | `AsNoTracking`，按`CreatedAt`倒序 | `GetPurchaseOrders` |
| `Add(PurchaseOrder purchaseOrder)` | —— | `CreateFromRequestQuotation`（内部专用） |

没有`Remove`：`PurchaseOrder`一旦建出来就不会被删除——AutoCount同步失败是靠`Status`/`SyncError`/`SyncAttempts`原地记录的（`Application.zh.md`"架构决策"那节解释过为什么这是`Result<T>`场景而不是异常），不是删掉这条记录重新来一次。
