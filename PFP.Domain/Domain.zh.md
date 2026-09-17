# PFP.Domain

**PFP.Procurement采购系统的Domain层。** 纯数据结构——不引用任何框架、不引用任何NuGet包，不知道`PFP.Application`、`PFP.Infrastructure`、`PFP.WebApi`的存在。

| | |
|---|---|
| 项目 | `PFP.Domain` |
| 依赖 | 无 |
| 被谁依赖 | `PFP.Application`（以及间接地，它之上的所有项目） |
| 状态 | 14个实体、9个枚举、6个标记接口——全部存在且编译通过 |
| 读者 | 后端维护者、前端对接人员、后续接手的开发者 |

本文档为英文版`Domain.md`的中文对照版本，内容保持一致。Application层的架构说明（Behaviour、Repository、请求管道）记录在`PFP.Application/Application.zh.md`，本文档不重复。

---

## 目录

1. [设计原则](#1-设计原则)
2. [完整目录结构](#2-完整目录结构)
3. [枚举速查](#3-枚举速查)
4. [实体清单](#4-实体清单)
5. [标记接口](#5-标记接口)
6. [已知问题](#6-已知问题)

---

## 1. 设计原则

- **贫血模型（Anemic Model）。** 每个实体只有属性，没有方法，不内嵌任何业务规则。类似"`PurchaseRequest`能不能从`Quoting`变成`PmReview`"这种状态机规则，不写在实体上，而是写在`PFP.Application/Features/`下对应的Handler里。
- **零框架依赖。** 不引用EF Core，不引用任何ORM或Web框架的包。这让这一层可以被任何上层复用，也可以脱离数据库单独做单元测试。
- **标记接口（Marker Interface）。** 诸如"有一个int类型的id"、"会被映射成Dto对外暴露"这类能力，用空方法体的接口（`IEntity`、`IExposableEntity`）表达，而不是在调用代码里到处写`is`判断。

---

## 2. 完整目录结构

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

> `Enums/`里没有`Department.cs`——`Department`目前在整个项目里都没有对应的枚举类型，详见"已知问题"第6条。

---

## 3. 枚举速查

枚举值序列化成JSON时是字符串（比如`"role": "HeadOfPurchase"`），不是数字。

| 枚举 | 可选值 | 用在哪 |
|---|---|---|
| `Role` | `Requester`、`PurchaseManager`、`DirectorL1`、`DirectorL2`、`HeadOfPurchase` | `User.Role`、`ApprovalSetting.ApproverRole` |
| `PRStatus` | `Quoting`、`PmReview`、`Approved`、`Rejected`、`Converted` | `PurchaseRequest.Status` |
| `Copystatus`（注：类型名是小写的"s"，不是`CopyStatus`，详见已知问题第4条） | `Pending`、`Submitted` | `SupplierQuoteCopy.Status`——一次性，不可逆 |
| `RQStatus` | `PendingL1`、`PendingL2`、`Approved`、`Rejected`、`Converted` | `RequestQuotation.Status` |
| `ApprovalLevel` | `L1`、`L2` | `RQApproval.Level`、`ApprovalSetting.level` |
| `ApprovalAction` | `Approved`、`Rejected` | `RQApproval.Action` |
| `POStatus` | `Created`、`Synced`、`SyncFailed` | `PurchaseOrder.Status` |
| `SupplierAccountStatus` | `Invited`、`Registered`、`Suspended` | `Supplier.AccountStatus` |
| `DocumentType` | `PurchaseRequest`、`RequestQuotation`、`PurchaseOrder` | `IDocumentNumberGenerator`（Application层）内部使用，前端一般不会直接接触 |

---

## 4. 实体清单

### `User`——内部账号

| 字段 | 类型 |
|---|---|
| Id | int |
| Name | string |
| Email | string |
| Role | Role |
| Department | string（必填） |
| IsActive | bool，默认`true` |
| PasswordHash | string |
| PurchaseRequests | ICollection\<PurchaseRequest\>（作为Requester发起的） |
| RQApprovals | ICollection\<RQApproval\>（作为Approver审批过的） |

### `Supplier`——供应商账号

| 字段 | 类型 |
|---|---|
| Id | int（必填，见已知问题第7条） |
| Name | string |
| Email | string |
| Contact | string? |
| InAutoCount | bool |
| CreditorCode | string?——AutoCount债权人编号，同步匹配键 |
| AccountStatus | SupplierAccountStatus，默认`Invited` |
| RegistrationToken | string? |
| RegisteredAt | DateTime? |
| PasswordHash | string? |
| RequestQuotations | ICollection\<RequestQuotation\> |
| PurchaseOrders | ICollection\<PurchaseOrder\> |

### `Item`——物料主数据

| 字段 | 类型 |
|---|---|
| Id | int |
| Code | string——唯一，AutoCount同步匹配键 |
| Name | string |
| Uom | string |
| RefPrice | decimal |

### `PurchaseRequest`

| 字段 | 类型 |
|---|---|
| Id | int |
| DocNo | string |
| RequestedId | int——应为`RequesterId`，见已知问题第1条 |
| Requester | User |
| Department | string |
| Status | PRStatus，默认`Quoting` |
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

| 字段 | 类型 |
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

### `SupplierQuoteCopy`——PR分发给每个供应商的独立副本

| 字段 | 类型 |
|---|---|
| Id | int |
| PurchaseRequestId / PurchaseRequest | int / PurchaseRequest |
| SupplierId / Supplier | int / Supplier |
| Token | string——不可猜测的访问凭证 |
| Status | Copystatus，默认`Pending` |
| Remarks | string? |
| TotalAmount | decimal |
| SentAt | DateTime |
| SubmittedAt | DateTime? |
| QuotedDetail | ICollection\<SupplierQuoteDetail\> |

### `SupplierQuoteDetail`——供应商实际提交的报价明细

| 字段 | 类型 |
|---|---|
| Id | int |
| SupplierQuoteCopyId / supplierQuoteCopy | int / SupplierQuoteCopy |
| PurchaseRequestItemId | int |
| purchaseRequest | PurchaseRequest——类型跟外键对不上，见已知问题第2条 |
| UnitPrice | decimal |

### `RequestQuotation`

| 字段 | 类型 |
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
| PurchaseOrder | ICollection\<PurchaseOrder\>——关系本应是1对1，集合类型不准确，见已知问题第3条 |

### `RequestQuotationDetail`——快照式存储

| 字段 | 类型 |
|---|---|
| Id | int |
| RequestQuotationId / RequestQuotation | int / RequestQuotation |
| ItemCode / Description / Uom | string |
| Qty / UnitPrice | decimal |

### `RQApproval`——每一次审批/拒绝的留痕记录

| 字段 | 类型 |
|---|---|
| Id | int |
| RequestQuotationId / RequestQuotation | int / RequestQuotation |
| Level | ApprovalLevel |
| ApproverId / Approver | int / User |
| Action | ApprovalAction |
| Remark | string? |
| Timestamp | DateTime |

### `PurchaseOrder`

| 字段 | 类型 |
|---|---|
| Id | int |
| DocNo | string |
| RequestQuotationId / RequestQuotation | int / RequestQuotation（1对1） |
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

### `PurchaseOrderDetail`——快照式存储

| 字段 | 类型 |
|---|---|
| Id | int |
| PurchaseOrderId | int |
| PurchaseOrder | PurchaseOrder |
| ItemCode / Description / Uom | string |
| Qty / UnitPrice | decimal |

### `ApprovalSetting`——只有L1/L2两条记录

| 字段 | 类型 |
|---|---|
| level | ApprovalLevel——小写开头，见已知问题第5条 |
| ApproverRole | Role |
| MinAmount | decimal |
| MaxAmount | decimal? |

### `Counter`——单据编号生成器

| 字段 | 类型 |
|---|---|
| Name | string（主键，"PR" / "RQ" / "PO"） |
| Seq | int |

---

## 5. 标记接口

| 接口 | 定义 | 谁实现 |
|---|---|---|
| `IEntity` | `int Id { get; }` | 大部分实体。`ApprovalSetting`（主键是`Level`）和`Counter`（主键是`Name`）不实现它。 |
| `IExposableEntity` | 空接口，标记"会被映射成Dto对外返回" | 除`Counter`外的所有实体。 |
| `IBaseEntity` | `IEntity` + `ICreationAuditable`的组合 | 需要记录创建时间的实体。 |
| `IBaseExposableEntity` | `IBaseEntity` + `IExposableEntity`的组合 | 同上一组。 |
| `ICreationAuditable`（`Auditables/`） | `DateTime CreatedAt { get; }` | 有创建时间的实体。 |
| `IConcurrencyAware`（`Concurrency/`） | `byte[] RowVersion { get; set; }` | `PurchaseRequest`、`RequestQuotation`、`PurchaseOrder`——需要乐观并发控制的三个核心单据实体。 |

这套接口让`PFP.Application`层可以写通用的基础设施代码——比如一个约束`T : IEntity`的`GetOrThrowAsync<T>`辅助方法，或者一个约束`T : IConcurrencyAware`的乐观并发重试辅助逻辑，不需要针对每个实体类型各写一份。

---

## 6. 已知问题

对照真实源码核对本文档时发现的问题。这些问题目前都不影响编译，记录在这里是为了不被重复发现，也方便以后有计划地处理。

| 编号 | 位置 | 问题 | 影响 |
|---|---|---|---|
| 1 | `PurchaseRequest.RequestedId` | 应为`RequesterId`——现在的名字跟配对的导航属性`Requester`对不上 | 纯拼写问题，无功能影响 |
| 2 | `SupplierQuoteDetail.purchaseRequest` | 类型是`PurchaseRequest`，但配对的外键是`PurchaseRequestItemId`；导航属性应该是`PurchaseRequestDetail`类型 | 类型和外键不匹配——配置EF Core关系时容易出问题 |
| 3 | `RequestQuotation.PurchaseOrder` | 类型是`ICollection<PurchaseOrder>`，但`PurchaseOrder.RequestQuotationId`建立的是1对1关系（一个RQ最多转出一个PO） | 应该是`PurchaseOrder?` |
| 4 | `Enums/CopyStatus.cs` | 类型声明为`Copystatus`（小写"s"），跟`PRStatus`/`RQStatus`/`POStatus`的PascalCase命名风格不一致 | 仅命名规范问题 |
| 5 | `ApprovalSetting.level` | 属性名小写开头，是代码库里唯一一个没有遵循PascalCase的属性 | 仅命名规范问题 |
| 6 | `User.Department`、`PurchaseRequest.Department` | 两处都是普通`string`；尽管`Department`在最初的Scope核对记录里是一个明确的业务概念，这个项目里目前没有对应的枚举 | 如果前端假设Department是固定取值集合，后端目前不会做这种校验 |
| 7 | `Supplier.Id` | 声明为`required int Id` | 每次`new Supplier { ... }`都会被强制要求显式赋值`Id`，但这本应是数据库自增生成的值；跟`User.Id`/`PurchaseRequest.Id`（普通`int`）不一致 |

第1-3条涉及改字段类型或名称，等对应的`Features/`模块实现之后可能有连带影响。第4-7条改动范围小、风险低。
