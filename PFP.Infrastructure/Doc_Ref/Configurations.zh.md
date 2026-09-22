# EF Core Configuration 设计参考

**每个实体一节，覆盖`Persistence/Configurations/`下每一个`IEntityTypeConfiguration<T>`。** 14个全部已实现，`InitialCreate`迁移也已经应用到真实的`PFP_Procurement`数据库（SQL Server LocalDB）上——这份文档现在描述的是已完成、经过核对的代码，不再是一份等着去实现的计划。下面每一列、每一个索引、每一条关系都对照过真实生成的迁移SQL，不是只凭`Configuration`源码读出来的。

本文档假设你已经了解`Infrastructure.zh.md`第7节记录的通用规范——枚举转字符串、可空唯一字段加过滤条件、`.IsRowVersion()`做乐观并发、`DeleteBehavior.Restrict`保护审计历史记录、以及在`.WithMany(x => x.Collection)`里永远显式写出反向导航属性。这些规则不会在每个实体下面重复一遍，下面只讲每个实体特有的东西。

---

## 目录

1. [先读这个：一个贯穿多个实体的风险——多重级联路径](#1-先读这个一个贯穿多个实体的风险多重级联路径)
2. [UserConfiguration](#2-userconfiguration)——已实现
3. [SupplierConfiguration](#3-supplierconfiguration)——已实现
4. [ItemConfiguration](#4-itemconfiguration)——已实现
5. [PurchaseRequestConfiguration](#5-purchaserequestconfiguration)——已实现
6. [PurchaseRequestDetailConfiguration](#6-purchaserequestdetailconfiguration)——已实现
7. [SupplierQuoteCopyConfiguration](#7-supplierquotecopyconfiguration)——已实现
8. [SupplierQuoteDetailConfiguration](#8-supplierquotedetailconfiguration)——已实现
9. [RequestQuotationConfiguration](#9-requestquotationconfiguration)——已实现
10. [RequestQuotationDetailConfiguration](#10-requestquotationdetailconfiguration)——已实现
11. [RQApprovalConfiguration](#11-rqapprovalconfiguration)——已实现
12. [PurchaseOrderConfiguration](#12-purchaseorderconfiguration)——已实现
13. [PurchaseOrderDetailConfiguration](#13-purchaseorderdetailconfiguration)——已实现
14. [ApprovalSettingConfiguration](#14-approvalsettingconfiguration)——已实现
15. [CounterConfiguration](#15-counterconfiguration)——已实现

---

## 1. 先读这个：一个贯穿多个实体的风险——多重级联路径

`PurchaseRequest`和`SupplierQuoteCopy`互相引用对方：

- `SupplierQuoteCopy.PurchaseRequestId` → `PurchaseRequest`（一份报价副本属于一张PR）
- `PurchaseRequest.SelectedSupplierCopyId` → `SupplierQuoteCopy`（PR回指"最终选定的是哪一份副本"）

如果这两个关系都配成`DeleteBehavior.Cascade`，SQL Server在生成迁移的时候会直接拒绝建第二个外键（报错信息类似"可能造成循环或多重级联路径"）。本文档统一采用的规则是：**"子记录依附于父记录"这个方向（`SupplierQuoteCopy.PurchaseRequestId`）用级联删除；"引用兄弟记录"这个方向（`PurchaseRequest.SelectedSupplierCopyId`）不用级联**——改用`DeleteBehavior.NoAction`。对照已应用的迁移确认过：`FK_supplierquotecopies_purchaserequests_PurchaseRequestId`是`ON DELETE CASCADE`；`FK_purchaserequests_supplierquotecopies_SelectedSupplierCopyId`完全没写`onDelete`，EF和SQL Server在这种情况下都默认成`NO ACTION`。

**生成第一个迁移时还发现了第二个、跟上面这个相关的坑**：一个多对一关系，如果"一"的那一侧有真实的集合导航（比如`Supplier.RequestQuotations`），"多"的那一侧写`.HasOne(x => x.Supplier)`时必须配一个命名的反向导航`.WithMany(x => x.RequestQuotations)`，不能用不带参数的`.WithMany()`。不带参数的`.WithMany()`不会绑定到那个真实集合——EF Core会转而为这个集合自动发现一条*另外的*、没配置过的关系，然后建出一个幽灵shadow外键列（`SupplierId1`），跟真的那个`SupplierId`并存。这个问题是`dotnet ef migrations add`时被EF自己的模型校验警告抓到的，在`PurchaseOrderConfiguration.cs`和`RequestQuotationConfiguration.cs`（下面第9、12节）里修掉了。本文档现在记录的每一条关系，只要两侧都有真实的导航属性，都会把两边显式命名出来。

---

## 2. UserConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `users` |
| 主键 | `Id` |
| 关键字段 | `Name`(150)、`Email`(255)、`Role`转字符串(30)、`Department`(100)、`IsActive`默认`true`、`PasswordHash`(500) |
| 索引 | `Email`唯一 |
| 关系 | `PurchaseRequests`（1对多，通过`PurchaseRequest.RequesterId`，`Restrict`）；`RQApprovals`（1对多，通过`RQApproval.ApproverId`，`Restrict`） |

`Name`/`Email`/`Department`/`PasswordHash`都没有显式调用`.IsRequired()`——这不是漏了。`User`对应的C#属性都是`required string`（项目全局开了`<Nullable>enable</Nullable>`，是不可空引用类型），EF Core自己的约定就已经把它们映射成`NOT NULL`，不需要再显式调用一次。对照迁移确认过：这四列都是`nullable: false`。

---

## 3. SupplierConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `suppliers` |
| 主键 | `Id` |
| 关键字段 | `Name`(150)、`Email`(255)、`Contact`(100，可空)、`CreditorCode`(50，可空)、`AccountStatus`转字符串(20)、`RegistrationToken`(100，可空)、`PasswordHash`(500，可空) |
| 索引 | `Email`唯一；`CreditorCode`唯一+过滤（仅`IS NOT NULL`）；`RegistrationToken`唯一+过滤 |
| 关系 | `RequestQuotations`（1对多，通过`RequestQuotation.SupplierId`，`Restrict`）；`PurchaseOrders`（1对多，通过`PurchaseOrder.SupplierId`，`Restrict`） |

`Supplier`的这两条关系在子实体那一侧（`RequestQuotationConfiguration.cs`/`PurchaseOrderConfiguration.cs`）也（重复）声明了一遍——见第1节。因为两边现在指向的是同一对导航属性，EF Core会把它们合并成一条关系，不会产生冲突。

---

## 4. ItemConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `items` |
| 主键 | `Id` |
| 关键字段 | `Code`(50)——AutoCount同步匹配键；`Name`(150)；`Uom`(20)；`RefPrice`——`decimal(18,2)` |
| 索引 | `Code`唯一 |
| 关系 | 无。`PurchaseRequestDetail.ItemCode`是纯字符串快照，不是指向`Item`的外键。 |

**发现并修复了一个bug**：这个文件原本把`x.Code`配置了两遍（"// Item Name"那段注释下面复制粘贴漏改了），`x.Name`反而完全没配置，于是`Name`落到了EF Core的默认约定——`nvarchar(max)`，没有长度上限——而不是一个真正的定长字段。这是在把本文档跟已应用的迁移对照核对时发现的，已经改成`builder.Property(x => x.Name).HasMaxLength(150).IsRequired();`，数据库也重新生成过了（直接删库重建——当时还没有任何数据，不需要额外写一条改名迁移）。

---

## 5. PurchaseRequestConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaserequests` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`Department`(100)；`Status`转字符串(20)，默认`"Quoting"`；`PmRemarks`(1000，可空)；`CreditorCode`/`CreditorName`(50/150，可空——快照，不是实时引用)；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一 |
| 关系 | `Requester`→`User`，通过`RequesterId`，`Restrict`；`DecidedByUser`→`User?`，通过`DecidedByUserId`，`Restrict`；`SelectedSupplierCopy`→`SupplierQuoteCopy?`，通过`SelectedSupplierCopyId`，**`NoAction`**（见第1节）；`Items`→`PurchaseRequestDetail`（1对多，`Cascade`）；`SupplierQuoteCopies`（1对多，`Cascade`） |

**本文档更早版本里记的那个基数问题已经解决。** `PurchaseRequest.RequestQuotations`现在是单个可空的`RequestQuotation?`导航，不是集合——这个文件里根本**不会**给它写`.HasMany(...)`。这条关系完全在另一侧、`RequestQuotationConfiguration.cs`（见第9节）里配置，那边负责一对一关系必须要有的`.HasForeignKey<RequestQuotation>(...)`调用。这个文件里原本还留着一段过时的`.HasMany(x => x.RequestQuotations)`（实体改成单个导航之前遗留下来的）——这段代码导致了一个真实的编译错误（`HasMany`没法绑定到一个不可枚举的属性），已经直接删掉而不是改成别的写法，因为`RequestQuotation`那一侧已经把这条关系配置完整了。

---

## 6. PurchaseRequestDetailConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaserequestdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500，可空)；`Location`(200，可空)；`Uom`(20)；`Qty`——`decimal(18,3)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，`Cascade`（明细行归属于它的PR） |

反向那一侧（`SupplierQuoteDetail.PurchaseRequestItemId`指向这条明细）在`SupplierQuoteDetailConfiguration.cs`里配置——见第8节。

---

## 7. SupplierQuoteCopyConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `supplierquotecopies` |
| 主键 | `Id` |
| 关键字段 | `Token`(100)；`Status`转字符串(20)，默认`"Pending"`；`Remarks`(1000，可空)；`TotalAmount`——`decimal(18,2)`；`SentAt`/`SubmittedAt`——`datetime2`（`SubmittedAt`可空） |
| 索引 | `Token`唯一（非空字段，不需要过滤条件） |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，**`Cascade`**，命名反向导航`.WithMany(x => x.SupplierQuoteCopies)`；`Supplier`，通过`SupplierId`，`Restrict`，不带参数的`.WithMany()` |

这是第1节提到的那个循环关系里"被拥有"的那一侧——它朝`PurchaseRequest`的级联删除，正是逼着`PurchaseRequest.SelectedSupplierCopyId`必须用`NoAction`而不是`Cascade`的原因。跟`RequestQuotation`/`PurchaseOrder`上那两条`Supplier`关系不一样，这里的`Supplier`关系用不带参数的`.WithMany()`是合理的——`Supplier`根本没有`SupplierQuoteCopies`这个集合导航可以绑定，没什么可命名的。

---

## 8. SupplierQuoteDetailConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `supplierquotedetails` |
| 主键 | `Id` |
| 关键字段 | `UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `SupplierQuoteCopy`，通过`SupplierQuoteCopyId`，`Cascade`，命名反向导航`.WithMany(x => x.QuotedDetail)`；`PurchaseRequestDetail`，通过`PurchaseRequestItemId`，`Restrict`，命名反向导航`.WithMany(x => x.QuotedBy)` |

第二个关系指向的是`PurchaseRequestDetail`（正确的实体，跟外键名`PurchaseRequestItemId`对得上）——本文档更早的版本记过这个要等Domain层一个类型问题先修好才能配对；那个修复已经落地，已应用迁移里的`FK_supplierquotedetails_purchaserequestdetails_PurchaseRequestItemId`确实指向`purchaserequestdetails`，可以确认。

---

## 9. RequestQuotationConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `requestquotations` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`TotalAmount`——`decimal(18,2)`；`Status`转字符串(20)；`RequiresL2`——`bool`；`CreatedAt`——`datetime2`；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一；**`PurchaseRequestId`唯一** |
| 关系 | `PurchaseRequest`，通过`PurchaseRequestId`，`Restrict`，`.HasForeignKey<RequestQuotation>(...)`；`Supplier`，通过`SupplierId`，`Restrict`，命名反向导航`.WithMany(x => x.RequestQuotations)`；`Items`→`RequestQuotationDetail`（1对多，`Cascade`）；`Approvals`→`RQApproval`（1对多，`Cascade`）；`PurchaseOrder` |

**本文档更早版本记的两个基数问题都已经解决。** `PurchaseRequest.RequestQuotations`和`RequestQuotation.PurchaseOrder`现在都是单个可空导航，而且两条关系都由真实的唯一索引在数据库层面强制1对0或1（这里是`IX_requestquotations_PurchaseRequestId`；`PurchaseOrder`那边是`IX_purchaseorders_RequestQuotationId`，见第12节）。`PurchaseRequest`这条关系的配置是`.HasOne(x => x.PurchaseRequest).WithOne(x => x.RequestQuotations).HasForeignKey<RequestQuotation>(x => x.PurchaseRequestId)`——一对一关系必须显式写出`HasForeignKey<TDependent>`这个泛型参数，因为单靠一个对称的`HasOne().WithOne()`链条，EF Core没法自己推断出到底是哪一侧持有外键。

这里的`Supplier`关系原本用的是不带参数的`.WithMany()`——就是第1节说的那个`SupplierId1`幽灵外键bug。已经改成`.WithMany(x => x.RequestQuotations)`。

---

## 10. RequestQuotationDetailConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `requestquotationdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500)；`Uom`(20)；`Qty`——`decimal(18,3)`；`UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Cascade`，命名反向导航`.WithMany(x => x.Items)` |

---

## 11. RQApprovalConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `rqapprovals` |
| 主键 | `Id` |
| 关键字段 | `Level`转字符串(10)；`Action`转字符串(20)；`Remark`(1000，可空)；`Timestamp`——`datetime2` |
| 索引 | 目前没有任何用例需要额外索引 |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Cascade`，命名反向导航`.WithMany(x => x.Approvals)`；`Approver`→`User`，通过`ApproverId`，`Restrict`，命名反向导航`.WithMany(x => x.RQApprovals)` |

---

## 12. PurchaseOrderConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaseorders` |
| 主键 | `Id` |
| 关键字段 | `DocNo`(50)；`TotalAmount`——`decimal(18,2)`；`Status`转字符串(20)；`IsNewSupplier`——`bool`；`CreatedAt`/`SyncedAt`——`datetime2`（`SyncedAt`可空）；`AutoCountPORef`/`AutoCountCreditorRef`(100，可空)；`SyncError`(2000，可空)；`SyncAttempts`——`int`，默认`0`；`RowVersion`用`.IsRowVersion()` |
| 索引 | `DocNo`唯一；**`RequestQuotationId`唯一**——这才是真正在数据库层面强制"一张RQ最多转出一张PO"的地方 |
| 关系 | `RequestQuotation`，通过`RequestQuotationId`，`Restrict`，`.HasForeignKey<PurchaseOrder>(...)`，命名反向导航`.WithOne(x => x.PurchaseOrder)`；`Supplier`，通过`SupplierId`，`Restrict`，命名反向导航`.WithMany(x => x.PurchaseOrders)`；`Items`→`PurchaseOrderDetail`（1对多，`Cascade`） |

跟`RequestQuotationConfiguration.cs`同样的shadow外键bug（见第1节），同样的修法：`Supplier`关系原本用不带参数的`.WithMany()`，改成了`.WithMany(x => x.PurchaseOrders)`。

---

## 13. PurchaseOrderDetailConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `purchaseorderdetails` |
| 主键 | `Id` |
| 关键字段 | `ItemCode`(50)；`Description`(500)；`Uom`(20)；`Qty`——`decimal(18,3)`；`UnitPrice`——`decimal(18,2)` |
| 索引 | 除外键外无其它索引 |
| 关系 | `PurchaseOrder`，通过`PurchaseOrderId`，`Cascade`，命名反向导航`.WithMany(x => x.Items)` |

---

## 14. ApprovalSettingConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `approvalsettings` |
| 主键 | `Level`——**不是`Id`**。`ApprovalSetting`没有实现`IEntity`，它天然的主键就是`ApprovalLevel`这个值本身（这张表永远只有两条记录：`L1`、`L2`）。 |
| 关键字段 | `Level`转字符串(10，跟主键类型对应)；`ApproverRole`转字符串(30)；`MinAmount`——`decimal(18,2)`；`MaxAmount`——`decimal(18,2)`，可空（`null`=无上限） |
| 索引 | 除主键外无其它索引 |
| 关系 | 无。`ApproverRole`是一个`Role`枚举值，不是指向某个具体`User`记录的外键——运行时真正的审批人是拿当前登录`User.Role`去跟这个值比对出来的，不是靠数据库关系查出来的。 |

这张表的两条记录本该是`ApplicationDbContextSeed.cs`铺进去的，但这个文件现在还是个没实现的空壳——见`Infrastructure.zh.md`"已知问题"第6条。表本身已经建好、结构也对，只是新建出来的数据库里现在一条记录都没有。

---

## 15. CounterConfiguration

**状态：已实现。**

| 项目 | 设计 |
|---|---|
| 表名 | `counters` |
| 主键 | `Name`——一个字符串（`"PR"`、`"RQ"`、`"PO"`），不是`Id`。`Counter`没有实现任何标记接口。 |
| 关键字段 | `Name`(10)；`Seq`——`int`，默认`0` |
| 索引 | 除主键外无其它索引 |
| 关系 | 无 |

这里没有`RowVersion`、也不需要乐观并发。真正的实现（`Persistence/DocumentNumbers/SequentialDocumentNumberGenerator.cs`，记录在`Infrastructure.zh.md`第1节和`Repositories.zh.md`）最后用的是一条原始SQL`UPDATE counters SET Seq = Seq + 1 OUTPUT INSERTED.Seq WHERE Name = @prefix`，通过`Database.SqlQuery<int>(...)`执行，而不是这里最初设想的`ExecuteUpdateAsync`——两种写法在并发访问下都是原子安全的，不需要额外的并发令牌，但选`SqlQuery`是因为这个自增操作完全绕开了change tracker，不会有风险去提前冲掉同一个`DbContext`上、调用它的Handler在这次请求里还没提交的其它改动。
