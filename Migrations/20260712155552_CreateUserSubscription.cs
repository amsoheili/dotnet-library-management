using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibrarySubscriptions_Libraries_LibraryId",
                table: "LibrarySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_LibrarySubscriptions_LibraryId",
                table: "LibrarySubscriptions");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LibrarySubscriptions",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LibrarySubscriptions_LibraryId",
                table: "LibrarySubscriptions",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibrarySubscriptions_Libraries_LibraryId",
                table: "LibrarySubscriptions",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibrarySubscriptions_Libraries_LibraryId",
                table: "LibrarySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_LibrarySubscriptions_LibraryId",
                table: "LibrarySubscriptions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LibrarySubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_LibrarySubscriptions_LibraryId",
                table: "LibrarySubscriptions",
                column: "LibraryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LibrarySubscriptions_Libraries_LibraryId",
                table: "LibrarySubscriptions",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
