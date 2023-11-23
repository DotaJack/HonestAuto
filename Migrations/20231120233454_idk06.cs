using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class idk06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarValue",
                table: "CarEvaluations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CarValue",
                table: "CarEvaluations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
