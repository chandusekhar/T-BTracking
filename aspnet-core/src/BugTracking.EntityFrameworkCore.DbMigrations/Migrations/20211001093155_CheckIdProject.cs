using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracking.Migrations
{
    public partial class CheckIdProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "BugTrackingConversations");

            migrationBuilder.DropColumn(
                name: "ReceivedId",
                table: "BugTrackingConversations");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "BugTrackingConversations",
                newName: "idProject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "idProject",
                table: "BugTrackingConversations",
                newName: "SenderId");

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "BugTrackingConversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedId",
                table: "BugTrackingConversations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
