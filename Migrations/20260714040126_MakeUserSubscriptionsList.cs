using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserSubscriptionsList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_People_LibraryUserId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions",
                column: "LibraryUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions",
                column: "LibraryUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
            name: "FK_UserSubscriptions_People_LibraryUserId",
            table: "UserSubscriptions",
            column: "LibraryUserId",
            principalTable: "People",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
        }
    }
}
