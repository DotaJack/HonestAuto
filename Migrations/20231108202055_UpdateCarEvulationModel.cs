using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarEvulationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Cars",
                newName: "UserID");

            migrationBuilder.AddColumn<string>(
                name: "EvaluationStatus",
                table: "CarEvaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluationStatus",
                table: "CarEvaluations");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Cars",
                newName: "SellerID");
        }
    }
}