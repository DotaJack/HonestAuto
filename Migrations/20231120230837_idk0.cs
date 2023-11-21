using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class idk0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
               name: "IX_CarEvaluations_CarID",
               table: "CarEvaluations",
               column: "CarID");

            migrationBuilder.AddForeignKey(
                name: "FK_CarEvaluations_Cars_CarID",
                table: "CarEvaluations",
                column: "CarID",
                principalTable: "Cars",
                principalColumn: "CarID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
    name: "FK_CarEvaluations_Cars_CarID",
    table: "CarEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_CarEvaluations_CarID",
                table: "CarEvaluations");
        }
    }
}