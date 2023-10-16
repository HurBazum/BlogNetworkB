using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogNetworkB.Migrations
{
    /// <inheritdoc />
    public partial class RoleDesccription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roles",
                type: "TEXT",
                nullable: true,
                defaultValue: "standart role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roles");
        }
    }
}
