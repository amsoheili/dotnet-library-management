using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLibraryUserSubscription : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_People_LibrarySubscriptionId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "LibrarySubscriptionId",
                table: "People");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_People_LibraryUserId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "LibrarySubscriptionId",
                table: "People",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_LibraryUserId",
                table: "UserSubscriptions",
                column: "LibraryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_People_LibrarySubscriptionId",
                table: "People",
                column: "LibrarySubscriptionId");

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
