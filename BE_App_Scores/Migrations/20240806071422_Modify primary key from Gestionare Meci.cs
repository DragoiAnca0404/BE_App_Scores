using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_App_Scores.Migrations
{
    /// <inheritdoc />
    public partial class ModifyprimarykeyfromGestionareMeci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GestionareMeciuri",
                table: "GestionareMeciuri");

            migrationBuilder.DropIndex(
                name: "IX_GestionareMeciuri_IdActivitate",
                table: "GestionareMeciuri");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GestionareMeciuri");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GestionareMeciuri",
                table: "GestionareMeciuri",
                columns: new[] { "IdActivitate", "IdEchipa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GestionareMeciuri",
                table: "GestionareMeciuri");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GestionareMeciuri",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GestionareMeciuri",
                table: "GestionareMeciuri",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GestionareMeciuri_IdActivitate",
                table: "GestionareMeciuri",
                column: "IdActivitate");
        }
    }
}
