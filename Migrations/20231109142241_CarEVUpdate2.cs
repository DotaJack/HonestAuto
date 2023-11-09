using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class CarEVUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "CarEvaluations");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImage",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImage",
                table: "Mechanics",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "CarImage",
                table: "Cars",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "EVImage",
                table: "CarEvaluations",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Mechanics");

            migrationBuilder.DropColumn(
                name: "CarImage",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "EVImage",
                table: "CarEvaluations");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CarEvaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
