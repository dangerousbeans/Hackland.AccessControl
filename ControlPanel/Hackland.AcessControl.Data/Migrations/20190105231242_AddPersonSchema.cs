using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class AddPersonSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Person",
               columns: table => new
               {
                   Id = table.Column<int>()
                       .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                   Name = table.Column<string>(nullable: false),
                   CreatedTimestamp = table.Column<DateTime>(nullable: false),
                   CreatedByUserId = table.Column<Guid>(nullable: false),
                   UpdatedTimestamp = table.Column<DateTime>(nullable: true),
                   UpdatedByUserId = table.Column<Guid>(nullable: true),
                   LastSeenTimestamp = table.Column<DateTime>(nullable: true),
                   IsDeleted = table.Column<bool>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Person", x => x.Id);
                   table.ForeignKey("FK_Person_CreatedUser", x => x.CreatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
                   table.ForeignKey("FK_Person_UpdatedUser", x => x.UpdatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
               });

            migrationBuilder.CreateTable(
              name: "PersonDoor",
              columns: table => new
              {
                  Id = table.Column<int>()
                      .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                  CreatedTimestamp = table.Column<DateTime>(nullable: false),
                  CreatedByUserId = table.Column<Guid>(nullable: false),
                  UpdatedTimestamp = table.Column<DateTime>(nullable: true),
                  UpdatedByUserId = table.Column<Guid>(nullable: true),
                  IsDeleted = table.Column<bool>(nullable: false),
                  PersonId = table.Column<int>(nullable: false),
                  DoorId = table.Column<int>(nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_PersonDoor", x => x.Id);
                  table.ForeignKey("FK_PersonDoor_CreatedUser", x => x.CreatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_UpdatedUser", x => x.UpdatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_Person", x => x.PersonId, "Person", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_Door", x => x.DoorId, "Door", "Id", onDelete: ReferentialAction.Restrict);

              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PersonDoor");
            migrationBuilder.DropTable(name: "Person");
        }
    }
}
