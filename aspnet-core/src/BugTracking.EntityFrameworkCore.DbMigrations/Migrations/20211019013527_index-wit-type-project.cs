using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracking.Migrations
{
    public partial class indexwittypeproject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WitType",
                table: "BugTrackingProjects",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BugTrackingProjects_WitType",
                table: "BugTrackingProjects",
                column: "WitType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BugTrackingProjects_WitType",
                table: "BugTrackingProjects");

            migrationBuilder.AlterColumn<string>(
                name: "WitType",
                table: "BugTrackingProjects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
