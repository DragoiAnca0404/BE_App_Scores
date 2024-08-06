using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_App_Scores.Migrations
{
    /// <inheritdoc />
    public partial class BuilddiagramActivityScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activitati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titlu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activitati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GestionareMeciuri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdActivitate = table.Column<int>(type: "int", nullable: false),
                    IdEchipa = table.Column<int>(type: "int", nullable: false),
                    Scor = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GestionareMeciuri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GestionareMeciuri_Activitati_IdActivitate",
                        column: x => x.IdActivitate,
                        principalTable: "Activitati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GestionareMeciuri_Echipe_IdEchipa",
                        column: x => x.IdEchipa,
                        principalTable: "Echipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GestionareMeciuri_IdActivitate",
                table: "GestionareMeciuri",
                column: "IdActivitate");

            migrationBuilder.CreateIndex(
                name: "IX_GestionareMeciuri_IdEchipa",
                table: "GestionareMeciuri",
                column: "IdEchipa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GestionareMeciuri");

            migrationBuilder.DropTable(
                name: "Activitati");
        }
    }
}
