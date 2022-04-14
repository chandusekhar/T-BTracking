using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracking.Migrations
{
    public partial class addidParentissueupdatechildissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BugTrackingChildsIssues");

            migrationBuilder.AddColumn<Guid>(
                name: "IdParent",
                table: "BugTrackingIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdChild",
                table: "BugTrackingChildsIssues",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdParent",
                table: "BugTrackingIssues");

            migrationBuilder.DropColumn(
                name: "IdChild",
                table: "BugTrackingChildsIssues");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BugTrackingChildsIssues",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
