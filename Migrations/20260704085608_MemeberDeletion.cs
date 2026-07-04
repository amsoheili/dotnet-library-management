using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class MemeberDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_People_MemberId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_People_MemberId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBooks_People_MemberId",
                table: "FavoriteBooks");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "FavoriteBooks",
                newName: "LibraryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteBooks_MemberId",
                table: "FavoriteBooks",
                newName: "IX_FavoriteBooks_LibraryUserId");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "BorrowedBooks",
                newName: "LibraryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowedBooks_MemberId",
                table: "BorrowedBooks",
                newName: "IX_BorrowedBooks_LibraryUserId");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Books",
                newName: "LibraryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_MemberId",
                table: "Books",
                newName: "IX_Books_LibraryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_People_LibraryUserId",
                table: "Books",
                column: "LibraryUserId",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_People_LibraryUserId",
                table: "BorrowedBooks",
                column: "LibraryUserId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBooks_People_LibraryUserId",
                table: "FavoriteBooks",
                column: "LibraryUserId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_People_LibraryUserId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_People_LibraryUserId",
                table: "BorrowedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteBooks_People_LibraryUserId",
                table: "FavoriteBooks");

            migrationBuilder.RenameColumn(
                name: "LibraryUserId",
                table: "FavoriteBooks",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteBooks_LibraryUserId",
                table: "FavoriteBooks",
                newName: "IX_FavoriteBooks_MemberId");

            migrationBuilder.RenameColumn(
                name: "LibraryUserId",
                table: "BorrowedBooks",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowedBooks_LibraryUserId",
                table: "BorrowedBooks",
                newName: "IX_BorrowedBooks_MemberId");

            migrationBuilder.RenameColumn(
                name: "LibraryUserId",
                table: "Books",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_LibraryUserId",
                table: "Books",
                newName: "IX_Books_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_People_MemberId",
                table: "Books",
                column: "MemberId",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_People_MemberId",
                table: "BorrowedBooks",
                column: "MemberId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteBooks_People_MemberId",
                table: "FavoriteBooks",
                column: "MemberId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
