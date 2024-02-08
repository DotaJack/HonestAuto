using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class NewCar23249990 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Cars",
                newName: "ModelID");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Cars",
                newName: "BrandID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModelID",
                table: "Cars",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "BrandID",
                table: "Cars",
                newName: "Brand");
        }
    }
}
