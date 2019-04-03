using Microsoft.EntityFrameworkCore.Migrations;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class DoorRemoteUnlockRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
              name: "RemoteUnlockRequestSeconds",
              table: "Door",
              nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
              name: "RemoteUnlockRequestSeconds",
              table: "Door"
              );
        }
    }
}
