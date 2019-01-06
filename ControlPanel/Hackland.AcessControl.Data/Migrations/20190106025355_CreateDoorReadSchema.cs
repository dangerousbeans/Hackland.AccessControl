using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class CreateDoorReadSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoorRead",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TokenValue = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    DoorId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: true),
                    IsSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorRead", x => x.Id);
                    table.ForeignKey("FK_DoorRead_Door", x => x.DoorId, "Door", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_DoorRead_Person", x => x.PersonId, "Person", "Id", onDelete: ReferentialAction.Restrict);
                });

        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DoorRead");
        }
    }
}