using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PFP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approvalsettings",
                columns: table => new
                {
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approvalsettings", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "counters",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counters", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RefPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InAutoCount = table.Column<bool>(type: "bit", nullable: false),
                    CreditorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegistrationToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchaseorderdetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseorderdetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchaseorders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestQuotationId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    IsNewSupplier = table.Column<bool>(type: "bit", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutoCountPORef = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AutoCountCreditorRef = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SyncError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SyncAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseorders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_purchaseorders_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchaserequestdetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaserequestdetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "purchaserequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Quoting"),
                    PmRemarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SelectedSupplierCopyId = table.Column<int>(type: "int", nullable: true),
                    CreditorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreditorName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaserequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_purchaserequests_users_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchaserequests_users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "requestquotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiresL2 = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestquotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestquotations_purchaserequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalTable: "purchaserequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requestquotations_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplierquotecopies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseRequestId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplierquotecopies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_supplierquotecopies_purchaserequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalTable: "purchaserequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supplierquotecopies_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "requestquotationdetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestQuotationId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestquotationdetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestquotationdetails_requestquotations_RequestQuotationId",
                        column: x => x.RequestQuotationId,
                        principalTable: "requestquotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rqapprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestQuotationId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ApproverId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rqapprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rqapprovals_requestquotations_RequestQuotationId",
                        column: x => x.RequestQuotationId,
                        principalTable: "requestquotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rqapprovals_users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplierquotedetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierQuoteCopyId = table.Column<int>(type: "int", nullable: false),
                    PurchaseRequestItemId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplierquotedetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_supplierquotedetails_purchaserequestdetails_PurchaseRequestItemId",
                        column: x => x.PurchaseRequestItemId,
                        principalTable: "purchaserequestdetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supplierquotedetails_supplierquotecopies_SupplierQuoteCopyId",
                        column: x => x.SupplierQuoteCopyId,
                        principalTable: "supplierquotecopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_items_Code",
                table: "items",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchaseorderdetails_PurchaseOrderId",
                table: "purchaseorderdetails",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaseorders_DocNo",
                table: "purchaseorders",
                column: "DocNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchaseorders_RequestQuotationId",
                table: "purchaseorders",
                column: "RequestQuotationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchaseorders_SupplierId",
                table: "purchaseorders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequestdetails_PurchaseRequestId",
                table: "purchaserequestdetails",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequests_DecidedByUserId",
                table: "purchaserequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequests_DocNo",
                table: "purchaserequests",
                column: "DocNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequests_RequesterId",
                table: "purchaserequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequests_SelectedSupplierCopyId",
                table: "purchaserequests",
                column: "SelectedSupplierCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_requestquotationdetails_RequestQuotationId",
                table: "requestquotationdetails",
                column: "RequestQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_requestquotations_DocNo",
                table: "requestquotations",
                column: "DocNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_requestquotations_PurchaseRequestId",
                table: "requestquotations",
                column: "PurchaseRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_requestquotations_SupplierId",
                table: "requestquotations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_rqapprovals_ApproverId",
                table: "rqapprovals",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_rqapprovals_RequestQuotationId",
                table: "rqapprovals",
                column: "RequestQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_supplierquotecopies_PurchaseRequestId",
                table: "supplierquotecopies",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_supplierquotecopies_SupplierId",
                table: "supplierquotecopies",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_supplierquotecopies_Token",
                table: "supplierquotecopies",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplierquotedetails_PurchaseRequestItemId",
                table: "supplierquotedetails",
                column: "PurchaseRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_supplierquotedetails_SupplierQuoteCopyId",
                table: "supplierquotedetails",
                column: "SupplierQuoteCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_CreditorCode",
                table: "suppliers",
                column: "CreditorCode",
                unique: true,
                filter: "[CreditorCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Email",
                table: "suppliers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_RegistrationToken",
                table: "suppliers",
                column: "RegistrationToken",
                unique: true,
                filter: "[RegistrationToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaseorderdetails_purchaseorders_PurchaseOrderId",
                table: "purchaseorderdetails",
                column: "PurchaseOrderId",
                principalTable: "purchaseorders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaseorders_requestquotations_RequestQuotationId",
                table: "purchaseorders",
                column: "RequestQuotationId",
                principalTable: "requestquotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaserequestdetails_purchaserequests_PurchaseRequestId",
                table: "purchaserequestdetails",
                column: "PurchaseRequestId",
                principalTable: "purchaserequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaserequests_supplierquotecopies_SelectedSupplierCopyId",
                table: "purchaserequests",
                column: "SelectedSupplierCopyId",
                principalTable: "supplierquotecopies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_supplierquotecopies_suppliers_SupplierId",
                table: "supplierquotecopies");

            migrationBuilder.DropForeignKey(
                name: "FK_supplierquotecopies_purchaserequests_PurchaseRequestId",
                table: "supplierquotecopies");

            migrationBuilder.DropTable(
                name: "approvalsettings");

            migrationBuilder.DropTable(
                name: "counters");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "purchaseorderdetails");

            migrationBuilder.DropTable(
                name: "requestquotationdetails");

            migrationBuilder.DropTable(
                name: "rqapprovals");

            migrationBuilder.DropTable(
                name: "supplierquotedetails");

            migrationBuilder.DropTable(
                name: "purchaseorders");

            migrationBuilder.DropTable(
                name: "purchaserequestdetails");

            migrationBuilder.DropTable(
                name: "requestquotations");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "purchaserequests");

            migrationBuilder.DropTable(
                name: "supplierquotecopies");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
