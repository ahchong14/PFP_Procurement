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
2. [实体关系图](#2-实体关系图)
3. [完整目录结构](#3-完整目录结构)
4. [枚举速查](#4-枚举速查)
5. [实体清单](#5-实体清单)
6. [状态机](#6-状态机)
7. [标记接口](#7-标记接口)
8. [已知问题](#8-已知问题)

---

## 1. 设计原则

- **贫血模型（Anemic Model）。** 每个实体只有属性，没有方法，不内嵌任何业务规则。类似"`PurchaseRequest`能不能从`Quoting`变成`PmReview`"这种状态机规则，不写在实体上，而是写在`PFP.Application/Features/`下对应的Handler里。
- **零框架依赖。** 不引用EF Core，不引用任何ORM或Web框架的包。这让这一层可以被任何上层复用，也可以脱离数据库单独做单元测试。
- **标记接口（Marker Interface）。** 诸如"有一个int类型的id"、"会被映射成Dto对外暴露"这类能力，用空方法体的接口（`IEntity`、`IExposableEntity`）表达，而不是在调用代码里到处写`is`判断。

---

## 2. 实体关系图

跟`PFP.Infrastructure/Infrastructure.zh.md`里那张图是同样的14个实体，但这里是纯业务视角——不带删除行为、不带持久化细节。要看这些关系在EF Core/SQL Server层面具体怎么处理，去那份文档。

```mermaid
erDiagram
    User ||--o{ PurchaseRequest : "发起，作为Requester"
    User ||--o{ PurchaseRequest : "决定，作为DecidedByUser"
    User ||--o{ RQApproval : "审批，作为Approver"

    PurchaseRequest ||--o{ PurchaseRequestDetail : "明细行"
    PurchaseRequest ||--o{ SupplierQuoteCopy : "分发给最多3家供应商"
    PurchaseRequest |o--o| SupplierQuoteCopy : "选定"
    PurchaseRequest ||--o{ RequestQuotation : "转换成"

    Supplier ||--o{ SupplierQuoteCopy : "收到"
    Supplier ||--o{ RequestQuotation : "被报价"
    Supplier ||--o{ PurchaseOrder : "履约"

    SupplierQuoteCopy ||--o{ SupplierQuoteDetail : "提交的报价"
    PurchaseRequestDetail ||--o{ SupplierQuoteDetail : "被报价"

    RequestQuotation ||--o{ RequestQuotationDetail : "明细行（快照）"
    RequestQuotation ||--o{ RQApproval : "审批记录"
    RequestQuotation ||--o| PurchaseOrder : "转换成"

    PurchaseOrder ||--o{ PurchaseOrderDetail : "明细行（快照）"
```

`Item`（物料主数据）和`Counter`（单据编号生成器）跟其它任何东西都没有外键关系，图上省略——`Item`只是被字符串编码快照引用（`PurchaseRequestDetail.ItemCode`），不是真正的外键。

图上有两条关系被画成"1对多"，是因为C#导航属性现在就是这么定义的，但这两条背后的业务规则其实是"1对0或1"：`PurchaseRequest -> RequestQuotation`（一张PR最多转出一张RQ）和`RequestQuotation -> PurchaseOrder`（一张RQ最多转出一张PO）。具体记录见"已知问题"那一节。

---

## 3. 完整目录结构

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

## 4. 枚举速查

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

## 5. 实体清单

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
| RequesterId | int |
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

## 6. 状态机

四个`Status`枚举各自驱动一套状态机，靠`PFP.Application/Features/`下的Handler强制执行，不是实体自己（呼应第1节的贫血模型原则）。下面的图描述的是设计意图，不会因为图这么画就自动生效——每一条箭头都对应一个具体的Handler。

### `PurchaseRequest.Status`（`PRStatus`）

```mermaid
stateDiagram-v2
    [*] --> Quoting : CreatePurchaseRequest
    Quoting --> PmReview : 供应商都提交完或PM打开审核
    PmReview --> Approved : ApprovePurchaseRequest
    PmReview --> Rejected : RejectPurchaseRequest
    Approved --> Converted : CreateFromApprovedPR（内部）
    Rejected --> [*]
    Converted --> [*]
```

### `RequestQuotation.Status`（`RQStatus`）

```mermaid
stateDiagram-v2
    [*] --> PendingL1 : CreateFromApprovedPR（内部）
    PendingL1 --> PendingL2 : ApproveRequestQuotation（level=L1，金额需要L2）
    PendingL1 --> Approved : ApproveRequestQuotation（level=L1，金额在L1范围内）
    PendingL1 --> Rejected : RejectRequestQuotation（level=L1）
    PendingL2 --> Approved : ApproveRequestQuotation（level=L2）
    PendingL2 --> Rejected : RejectRequestQuotation（level=L2）
    Approved --> Converted : ConvertToPurchaseOrder
    Rejected --> [*]
    Converted --> [*]
```

每张`RequestQuotation`是不是都必须走完`PendingL1`和`PendingL2`两级，还是金额够小可以从`PendingL1`直接到`Approved`，这是对着Scope文档提出的一个开放问题——详见`Application.zh.md`里针对"Purchase Order Conversion"那条的API说明。

### `SupplierQuoteCopy.Status`（`Copystatus`）

```mermaid
stateDiagram-v2
    [*] --> Pending : PR分发给供应商
    Pending --> Submitted : SubmitSupplierQuote
    Submitted --> [*]
```

单向，不可逆——供应商只允许提交一次（Scope文档Full Flow第3条）。

### `PurchaseOrder.Status`（`POStatus`）

```mermaid
stateDiagram-v2
    [*] --> Created : CreateFromRequestQuotation（内部）
    Created --> Synced : SyncPurchaseOrderToAutoCount成功
    Created --> SyncFailed : SyncPurchaseOrderToAutoCount失败
    SyncFailed --> Synced : 重试成功
    Synced --> [*]
```

`PurchaseOrder`上的`SyncAttempts`和`SyncError`，每走一次`SyncFailed -> Synced`这条重试路径都会更新。

---

## 7. 标记接口

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

## 8. 已知问题

对照真实源码核对本文档时发现的问题。这些问题目前都不影响编译，记录在这里是为了不被重复发现，也方便以后有计划地处理。

| 编号 | 位置 | 问题 | 影响 |
|---|---|---|---|
| 1 | `PurchaseRequest.RequesterId` | 已解决——之前误写成`RequestedId`，现在已经跟配对的导航属性`Requester`对上了。 | — |
| 2 | `SupplierQuoteDetail.purchaseRequest` | 类型是`PurchaseRequest`，但配对的外键是`PurchaseRequestItemId`；导航属性应该是`PurchaseRequestDetail`类型 | 类型和外键不匹配——配置EF Core关系时容易出问题 |
| 3 | `RequestQuotation.PurchaseOrder` | 类型是`ICollection<PurchaseOrder>`，但`PurchaseOrder.RequestQuotationId`建立的是1对1关系（一个RQ最多转出一个PO） | 应该是`PurchaseOrder?` |
| 4 | `Enums/CopyStatus.cs` | 类型声明为`Copystatus`（小写"s"），跟`PRStatus`/`RQStatus`/`POStatus`的PascalCase命名风格不一致 | 仅命名规范问题 |
| 5 | `ApprovalSetting.level` | 属性名小写开头，是代码库里唯一一个没有遵循PascalCase的属性 | 仅命名规范问题 |
| 6 | `User.Department`、`PurchaseRequest.Department` | 两处都是普通`string`；尽管`Department`在最初的Scope核对记录里是一个明确的业务概念，这个项目里目前没有对应的枚举 | 如果前端假设Department是固定取值集合，后端目前不会做这种校验 |
| 7 | `Supplier.Id` | 声明为`required int Id` | 每次`new Supplier { ... }`都会被强制要求显式赋值`Id`，但这本应是数据库自增生成的值；跟`User.Id`/`PurchaseRequest.Id`（普通`int`）不一致 |
| 8 | `PurchaseRequest.RequestQuotations` | 类型是`ICollection<RequestQuotation>`；Scope文档Full Flow描述的是一张PR转成**一张**RQ（单数），`CreateFromApprovedPRCommandHandler`的设计也是一次性转换 | 跟第3条是同一类问题——很可能应该是`RequestQuotation?`，还没确认 |

第2、3、8条涉及改字段类型，等对应的`Features/`模块实现之后可能有连带影响。第4-7条改动范围小、风险低。第1条已解决。
