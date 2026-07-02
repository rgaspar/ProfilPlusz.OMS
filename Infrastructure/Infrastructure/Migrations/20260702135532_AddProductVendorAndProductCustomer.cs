using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVendorAndProductCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "VendorContact");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "EmailAddressInvoice",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "EmailAddressOrderConfirmation",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "EmailAddressPurchaseOrder",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Customer");

            migrationBuilder.CreateTable(
                name: "ProductCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCustomer_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCustomer_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVendor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    VendorId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVendor_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVendor_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCustomer_CustomerId",
                table: "ProductCustomer",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCustomer_ProductId",
                table: "ProductCustomer",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVendor_ProductId",
                table: "ProductVendor",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVendor_VendorId",
                table: "ProductVendor",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCustomer");

            migrationBuilder.DropTable(
                name: "ProductVendor");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "VendorContact",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Vendor",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Vendor",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Vendor",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "CustomerContact",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressInvoice",
                table: "CustomerContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressOrderConfirmation",
                table: "CustomerContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressPurchaseOrder",
                table: "CustomerContact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
