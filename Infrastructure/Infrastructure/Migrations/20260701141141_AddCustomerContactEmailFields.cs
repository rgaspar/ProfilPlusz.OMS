using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerContactEmailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPersonName",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "EmailAddressInvoice",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "EmailAddressOrderConfirmation",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "EmailAddressPurchaseOrder",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customer");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddressInvoice",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "EmailAddressOrderConfirmation",
                table: "CustomerContact");

            migrationBuilder.DropColumn(
                name: "EmailAddressPurchaseOrder",
                table: "CustomerContact");

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonName",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressInvoice",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressOrderConfirmation",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddressPurchaseOrder",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Customer",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
