# EF Core Configuration 设计参考

**每个实体一节，覆盖`Persistence/Configurations/`下每一个`IEntityTypeConfiguration<T>`的设计意图。** 这是一份设计规格文档，不是"已完成代码"的说明——写这份文档的时候，只有`UserConfiguration`和`SupplierConfiguration`真的有内容，其余12个记录的是"该照这个方案去实现"的计划。

本文档假设你已经了解`Infrastructure.zh.md`第5节记录的通用规范——枚举转字符串、可空唯一字段加过滤条件、`.IsRowVersion()`做乐观并发、`DeleteBehavior.Restrict`保护审计历史记录。这些规则不会在每个实体下面重复一遍，下面只讲每个实体特有的东西。

---

## 目录

1. [先读这个：一个贯穿多个实体的风险——多重级联路径](#1-先读这个一个贯穿多个实体的风险多重级联路径)
2. [UserConfiguration](#2-userconfiguration)——已实现
3. [SupplierConfiguration](#3-supplierconfiguration)——已实现
4. [ItemConfiguration](#4-itemconfiguration)——待实现
5. [PurchaseRequestConfiguration](#5-purchaserequestconfiguration)——待实现
6. [PurchaseRequestDetailConfiguration](#6-purchaserequestdetailconfiguration)——待实现
7. [SupplierQuoteCopyConfiguration](#7-supplierquotecopyconfiguration)——待实现
8. [SupplierQuoteDetailConfiguration](#8-supplierquotedetailconfiguration)——待实现
9. [RequestQuotationConfiguration](#9-requestquotationconfiguration)——待实现
10. [RequestQuotationDetailConfiguration](#10-requestquotationdetailconfiguration)——待实现
11. [RQApprovalConfiguration](#11-rqapprovalconfiguration)——待实现
12. [PurchaseOrderConfiguration](#12-purchaseorderconfiguration)——待实现
13. [PurchaseOrderDetailConfiguration](#13-purchaseorderdetailconfiguration)——待实现
14. [ApprovalSettingConfiguration](#14-approvalsettingconfiguration)——待实现
15. [CounterConfiguration](#15-counterconfiguration)——待实现

---

## 1. 先读这个：一个贯穿多个实体的风险——多重级联路径

`PurchaseRequest`和`SupplierQuoteCopy`互相引用对方：

- `SupplierQuoteCopy.PurchaseRequestId` → `PurchaseRequest`（一份报价副本属于一张PR）
- `PurchaseRequest.SelectedSupplierCopyId` → `SupplierQuoteCopy`（PR回指"最终选定的是哪一份副本"）

如果这两个关系都配成`DeleteBehavior.Cascade`，SQL Server在生成迁移的时候会直接拒绝建第二个外键（报错信息类似"可能造成循环或多重级联路径"）。本文档统一采用的规则是：**"子记录依附于父记录"这个方向（`SupplierQuoteCopy.PurchaseRequestId`）用级联删除；"引用兄弟记录"这个方向（`PurchaseRequest.SelectedSupplierCopyId`）不用级联**——改用`DeleteBehavior.NoAction`。下面两个对应的实体小节会再各自提一次，但根本原因就是这个循环关系。

---

## 2. UserConfiguration

**状态：已实现。** 这里重复列一遍是为了完整性，具体代码以真实文件为准。

| 项目 | 设计 |
|---|---|
| 表名 | `users` |
| 主键 | `Id` |
| 关键字段 | `Name`(150)、`Email`(255)、`Role`转字符串(30)、`Department`(100)、`IsActive`默认`true`、`PasswordHash`(500) |
| 索引 | `Email`唯一 |
| 关系 | `PurchaseRequests`（1对多，通过`PurchaseRequest.RequesterId`，`Restrict`）；`RQApprovals`（1对多，通过`RQApproval.ApproverId`，`Restrict`） |

---

## 3. SupplierConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `suppliers` |
| 主键 | `Id` |
| 关键字段 | `Name`(150)、`Email`(255)、`Contact`(100，可空)、`CreditorCode`(50，可空)、`AccountStatus`转字符串(20)、`RegistrationToken`(100，可空)、`PasswordHash`(500，可空) |
| 索引 | `Email`唯一；`CreditorCode`唯一+过滤（仅`IS NOT NULL`）；`RegistrationToken`唯一+过滤 |
| 关系 | `RequestQuotations`（1对多，通过`SupplierId`，`Restrict`）；`PurchaseOrders`（1对多，通过`SupplierId`，`Restrict`） |

---

## 4. ItemConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `items` |
| 主键 | `Id` |
| 关键字段 | `Code`(50)——AutoCount同步匹配键；`Name`(150)；`Uom`(20)；`RefPrice`——`decimal`，`.HasPrecision(18, 2)` |
| 索引 | `Code`唯一 |
| 关系 | 无。`PurchaseRequestDetail.ItemCode`是纯字符串快照，不是指向`Item`的外键——见第6节。 |

`RefPrice`需要显式加`.HasPrecision(18, 2)`（或者业务实际需要的精度）——不显式配置的话，EF Core生成迁移时会给一条"decimal属性没有配置精度"的警告，而且会悄悄选一个默认精度，不一定跟应用层的假设一致。

---

## 5. PurchaseRequestConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaserequests` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`Department`(100)；`Status`转字符串(20)，默认`"Quoting"`；`PmRemarks`(1000，可空)；`CreditorCode`/`CreditorName`(50/150，可空——快照，不是实时引用)；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一 |
| 关系 | `Requester`→`User`，通过`RequesterId`，`Restrict`（这个设计假设`Domain.zh.md`已知问题第1条提到的`RequestedId`已经改名成`RequesterId`）；`DecidedByUser`→`User?`，通过`DecidedByUserId`，`Restrict`；`SelectedSupplierCopy`→`SupplierQuoteCopy?`，通过`SelectedSupplierCopyId`，**`NoAction`**（见第1节）；`Items`→`PurchaseRequestDetail`（1对多，`Cascade`——明细行没有独立于PR的生命周期）；`SupplierQuoteCopies`（1对多，`Cascade`，理由相同）；`RequestQuotations`（1对多，`Restrict`） |

**写这个文件之前值得先确认的一个设计问题**：`PurchaseRequest.RequestQuotations`类型是`ICollection<RequestQuotation>`，但Scope文档的Full Flow描述的是"一张PR转成**一张**RQ"（单数），`CreateFromApprovedPRCommandHandler`的设计也是一次性转换。如果实际基数应该是1对0或1，这就跟`Domain.zh.md`已知问题第3条（`RequestQuotation.PurchaseOrder`）是同一类问题——建议先确认，如果确实如此，应该额外记一条已知问题，而不是照着现在的1对多类型去配置这个关系。

---

## 6. PurchaseRequestDetailConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaserequestdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500，可空)；`Location`(200，可空——`Domain.zh.md`实体表里标注过，这个字段的业务含义还没确认清楚)；`Uom`(20)；`Qty`——`decimal`，`.HasPrecision(18, 3)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，`Cascade`（明细行归属于它的PR） |

`QuotedBy`（指向这条明细的`SupplierQuoteDetail`集合）在对方那一侧配置——见第8节。

---

## 7. SupplierQuoteCopyConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `supplierquotecopies` |
| 主键 | `Id` |
| 关键字段 | `Token`(100)；`Status`转字符串(20)，默认`"Pending"`；`Remarks`(1000，可空)；`TotalAmount`——`decimal(18,2)`；`SentAt`/`SubmittedAt`——`datetime2` |
| 索引 | `Token`唯一（非空字段，不需要过滤条件） |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，**`Cascade`**；`Supplier`，通过`SupplierId`，`Restrict` |

这是第1节提到的那个循环关系里"被拥有"的那一侧——它朝`PurchaseRequest`的级联删除，正是逼着`PurchaseRequest.SelectedSupplierCopyId`必须用`NoAction`而不是`Cascade`的原因。

---

## 8. SupplierQuoteDetailConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `supplierquotedetails` |
| 主键 | `Id` |
| 关键字段 | `UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `SupplierQuoteCopy`，通过`SupplierQuoteCopyId`，`Cascade`；第二个关系通过`PurchaseRequestItemId`应该指向`PurchaseRequestDetail`，`Restrict` |

第二个关系依赖`Domain.zh.md`已知问题第2条（`SupplierQuoteDetail.purchaseRequest`现在类型是`PurchaseRequest`，而不是配对外键`PurchaseRequestItemId`该指向的`PurchaseRequestDetail`）。这个类型不修正，这个`Configuration`没法正确写完——针对错误的实体类型配置关系，跟外键实际指向的目标对不上。

---

## 9. RequestQuotationConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `requestquotations` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`TotalAmount`——`decimal(18,2)`；`Status`转字符串(20)；`RequiresL2`——`bool`；`CreatedAt`——`datetime2`；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一 |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，`Restrict`；`Supplier`，通过`SupplierId`，`Restrict`；`Items`→`RequestQuotationDetail`（1对多，`Cascade`）；`Approvals`→`RQApproval`（1对多，`Cascade`）；`PurchaseOrder` |

`PurchaseOrder`是`Domain.zh.md`已知问题第3条——类型是`ICollection<PurchaseOrder>`，但业务规则描述的是1对1关系（一张RQ最多转出一张PO）。要正确配置这个关系，要么先解决这个类型问题，要么——如果暂时先不改类型——至少在`PurchaseOrder.RequestQuotationId`上加唯一索引（见第12节），让数据库层面强制这条1对1约束，不管C#类型现在写的是什么。

---

## 10. RequestQuotationDetailConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `requestquotationdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500)；`Uom`(20)；`Qty`——`decimal(18,3)`；`UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Cascade` |

---

## 11. RQApprovalConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `rqapprovals` |
| 主键 | `Id` |
| 关键字段 | `Level`转字符串(10)；`Action`转字符串(20)；`Remark`(1000，可空)；`Timestamp`——`datetime2` |
| 索引 | 目前没有任何用例需要额外索引 |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Cascade`；`Approver`→`User`，通过`ApproverId`，`Restrict` |

---

## 12. PurchaseOrderConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaseorders` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`TotalAmount`——`decimal(18,2)`；`Status`转字符串(20)；`IsNewSupplier`——`bool`；`CreatedAt`/`SyncedAt`——`datetime2`（`SyncedAt`可空）；`AutoCountPORef`/`AutoCountCreditorRef`(100，可空)；`SyncError`(2000，可空)；`SyncAttempts`——`int`，默认`0`；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一；**`RequestQuotationId`唯一**——这才是真正在数据库层面强制"一张RQ最多转出一张PO"的地方，不管`RequestQuotation`那边的C#导航属性有没有被改对（见第9节） |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Restrict`；`Supplier`，通过`SupplierId`，`Restrict`；`Items`→`PurchaseOrderDetail`（1对多，`Cascade`） |

---

## 13. PurchaseOrderDetailConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaseorderdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500)；`Uom`(20)；`Qty`——`decimal(18,3)`；`UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `PurchaseOrder`，通过`PurchaseOrderId`，`Cascade` |

---

## 14. ApprovalSettingConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `approvalsettings` |
| 主键 | `Level`——**不是`Id`**。`ApprovalSetting`没有实现`IEntity`，它天然的主键就是`ApprovalLevel`这个值本身（这张表永远只有两条记录：`L1`、`L2`）。 |
| 关键字段 | `Level`转字符串(10，跟主键类型对应)；`ApproverRole`转字符串(30)；`MinAmount`——`decimal(18,2)`；`MaxAmount`——`decimal(18,2)`，可空（`null`=无上限） |
| 索引 | 除主键外无其它索引 |
| 关系 | 无。`ApproverRole`是一个`Role`枚举值，不是指向某个具体`User`记录的外键——运行时真正的审批人是拿当前登录`User.Role`去跟这个值比对出来的，不是靠数据库关系查出来的。 |

回顾`Infrastructure.zh.md`第3节：这张表的两条记录是`ApplicationDbContextSeed.cs`铺进去的，不是通过某个Command创建的——这个实体故意没有"新建"这个用例。

---

## 15. CounterConfiguration

**状态：待实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `counters` |
| 主键 | `Name`——一个字符串（`"PR"`、`"RQ"`、`"PO"`），不是`Id`。`Counter`没有实现任何标记接口。 |
| 关键字段 | `Name`(10)；`Seq`——`int`，默认`0` |
| 索引 | 除主键外无其它索引 |
| 关系 | 无 |

这里没有`RowVersion`、也不需要乐观并发——单据编号的原子自增预期是靠`ExecuteUpdateAsync`实现的（一条`UPDATE ... SET Seq = Seq + 1`语句），这种写法本身在并发访问下就是安全的，不需要在EF Core层面额外加并发令牌。
