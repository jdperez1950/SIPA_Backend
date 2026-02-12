using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pavis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Organizations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Organizations");
        }
    }
}
