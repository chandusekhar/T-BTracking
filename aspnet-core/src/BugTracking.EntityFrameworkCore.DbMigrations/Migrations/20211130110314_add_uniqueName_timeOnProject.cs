using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracking.Migrations
{
    public partial class add_uniqueName_timeOnProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueName",
                table: "BugTrackingTimeOnProject",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueName",
                table: "BugTrackingTimeOnProject");
        }
    }
}
