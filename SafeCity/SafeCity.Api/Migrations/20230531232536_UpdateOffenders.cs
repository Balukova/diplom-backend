using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeCity.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOffenders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageEncoding",
                table: "Offenders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageEncoding",
                table: "Offenders");
        }
    }
}
