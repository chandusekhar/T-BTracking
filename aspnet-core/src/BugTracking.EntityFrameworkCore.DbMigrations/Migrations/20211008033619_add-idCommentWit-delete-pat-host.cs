using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracking.Migrations
{
    public partial class addidCommentWitdeletepathost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PAT",
                table: "BugTrackingAzures");

            migrationBuilder.AddColumn<int>(
                name: "WitCommentId",
                table: "BugTrackingComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WitCommentId",
                table: "BugTrackingComments");

            migrationBuilder.AddColumn<string>(
                name: "PAT",
                table: "BugTrackingAzures",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
