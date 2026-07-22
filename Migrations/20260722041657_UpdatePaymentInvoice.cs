using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "PaymentInvoices",
                newName: "ProductId");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "PaymentInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "PaymentInvoices");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "PaymentInvoices",
                newName: "SubscriptionId");
        }
    }
}
