using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class NewCar232499909 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModelID",
                table: "Cars",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "BrandID",
                table: "Cars",
                newName: "BrandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "Cars",
                newName: "ModelID");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Cars",
                newName: "BrandID");
        }
    }
}
