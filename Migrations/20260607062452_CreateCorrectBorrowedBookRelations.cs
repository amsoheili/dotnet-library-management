using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class CreateCorrectBorrowedBookRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_People_MemberId1",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_MemberId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "FavoriteBooks",
                columns: table => new
                {
                    FavoriteBooksId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MemberId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteBooks", x => new { x.FavoriteBooksId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_FavoriteBooks_Books_FavoriteBooksId",
                        column: x => x.FavoriteBooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteBooks_People_MemberId",
                        column: x => x.MemberId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBooks_MemberId",
                table: "FavoriteBooks",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteBooks");

            migrationBuilder.AddColumn<string>(
                name: "MemberId1",
                table: "Books",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Books_MemberId1",
                table: "Books",
                column: "MemberId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_People_MemberId1",
                table: "Books",
                column: "MemberId1",
                principalTable: "People",
                principalColumn: "Id");
        }
    }
}
