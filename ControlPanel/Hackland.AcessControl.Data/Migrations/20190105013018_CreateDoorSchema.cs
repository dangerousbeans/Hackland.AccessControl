using Hackland.AccessControl.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class CreateDoorSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Door",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    MacAddress = table.Column<string>(nullable: false),
                    LastHeartbeatTimestamp = table.Column<DateTime>(nullable: false),
                    LastReadTimestamp = table.Column<DateTime>(nullable: true),
                    Status = table.Column<DoorStatus>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Door", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Door");
        }
    }
}
