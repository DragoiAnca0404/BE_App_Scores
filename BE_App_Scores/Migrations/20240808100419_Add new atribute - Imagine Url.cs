using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_App_Scores.Migrations
{
    /// <inheritdoc />
    public partial class AddnewatributeImagineUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagineUrl",
                table: "Activitati",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagineUrl",
                table: "Activitati");
        }
    }
}
