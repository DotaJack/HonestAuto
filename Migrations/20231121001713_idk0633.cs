using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestAuto.Migrations
{
    /// <inheritdoc />
    public partial class idk0633 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarEvaluations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarEvaluations",
                columns: table => new
                {
                    CarEvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarID = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MechanicID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarEvaluations", x => x.CarEvaluationID);
                    table.ForeignKey(
                        name: "FK_CarEvaluations_Cars_CarID",
                        column: x => x.CarID,
                        principalTable: "Cars",
                        principalColumn: "CarID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarEvaluations_Mechanics_MechanicID",
                        column: x => x.MechanicID,
                        principalTable: "Mechanics",
                        principalColumn: "MechanicID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarEvaluations_CarID",
                table: "CarEvaluations",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarEvaluations_MechanicID",
                table: "CarEvaluations",
                column: "MechanicID");
        }
    }
}
