using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class fixplz2233 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CarImage",
                table: "Cars",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarImage",
                table: "Cars");
        }
    }
}
