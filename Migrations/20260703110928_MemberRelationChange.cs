using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class MemberRelationChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Libraries_LibraryId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Libraries_LibraryUser_LibraryId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_LibraryUser_LibraryId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "LibraryUser_LibraryId",
                table: "People");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Libraries_LibraryId",
                table: "People",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Libraries_LibraryId",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "LibraryUser_LibraryId",
                table: "People",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_People_LibraryUser_LibraryId",
                table: "People",
                column: "LibraryUser_LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Libraries_LibraryId",
                table: "People",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Libraries_LibraryUser_LibraryId",
                table: "People",
                column: "LibraryUser_LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id");
        }
    }
}
