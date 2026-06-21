using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library_management.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLibraryUsersDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "Update People set Discriminator = 'LibraryUser' where Discriminator = 'User';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    "UPDATE People SET Discriminator = 'User' WHERE Discriminator = 'LibraryUser';");
        }
    }
}
