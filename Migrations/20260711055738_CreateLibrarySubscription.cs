using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class CreateLibrarySubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LibrarySubscriptionId",
                table: "People",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LibrarySubscriptions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MonthlyCost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    YearlyCost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    LibraryId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibrarySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibrarySubscriptions_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_People_LibrarySubscriptionId",
                table: "People",
                column: "LibrarySubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_LibrarySubscriptions_LibraryId",
                table: "LibrarySubscriptions",
                column: "LibraryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_People_LibrarySubscriptions_LibrarySubscriptionId",
                table: "People",
                column: "LibrarySubscriptionId",
                principalTable: "LibrarySubscriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_LibrarySubscriptions_LibrarySubscriptionId",
                table: "People");

            migrationBuilder.DropTable(
                name: "LibrarySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_People_LibrarySubscriptionId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "LibrarySubscriptionId",
                table: "People");
        }
    }
}
