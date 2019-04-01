using Hackland.AccessControl.Data.Enums;
using Hackland.AccessControl.Shared;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class FixIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpDown(migrationBuilder);
        }

        private static void UpDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PersonDoor");
            migrationBuilder.DropTable(name: "DoorRead");
            migrationBuilder.DropTable(name: "Person");
            migrationBuilder.DropTable(name: "Door");
            migrationBuilder.CreateTable(
                name: "Door",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation(Settings.UseSqlServer ? "SqlServer:ValueGenerationStrategy" : "MySql:ValueGeneratedOnAdd", Settings.UseSqlServer ? (object)SqlServerValueGenerationStrategy.IdentityColumn : true),
                    Name = table.Column<string>(nullable: false),
                    MacAddress = table.Column<string>(nullable: false),
                    LastHeartbeatTimestamp = table.Column<DateTime>(nullable: false),
                    LastReadTimestamp = table.Column<DateTime>(nullable: true),
                    Status = table.Column<DoorStatus>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Door", x => x.Id);
                });

            migrationBuilder.AddColumn<DateTime>(
              name: "CreatedTimestamp",
              table: "Door",
              nullable: false);

            migrationBuilder.AddColumn<Guid>(
               name: "CreatedByUserId",
               table: "Door",
               nullable: true
               );
            migrationBuilder.AddColumn<DateTime>(
               name: "UpdatedTimestamp",
               table: "Door",
               nullable: true
               );
            migrationBuilder.AddColumn<Guid>(
              name: "UpdatedByUserId",
              table: "Door",
              nullable: true
              );

            migrationBuilder.Sql("ALTER TABLE Door ADD CONSTRAINT FK_Door_CreatedUser FOREIGN KEY (CreatedByUserId) REFERENCES User(Id)");
            migrationBuilder.Sql("ALTER TABLE Door ADD CONSTRAINT FK_Door_UpdatedUser FOREIGN KEY (UpdatedByUserId) REFERENCES User(Id)");

            migrationBuilder.CreateTable(
              name: "Person",
              columns: table => new
              {
                  Id = table.Column<int>()
                      .Annotation(Settings.UseSqlServer ? "SqlServer:ValueGenerationStrategy" : "MySql:ValueGeneratedOnAdd", Settings.UseSqlServer ? (object)SqlServerValueGenerationStrategy.IdentityColumn : true),
                  Name = table.Column<string>(nullable: false),
                  EmailAddress = table.Column<string>(nullable: false),
                  PhoneNumber = table.Column<string>(nullable: true),
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
                  PersonId = table.Column<int>(nullable: false),
                  DoorId = table.Column<int>(nullable: false),
                  CreatedTimestamp = table.Column<DateTime>(nullable: false),
                  CreatedByUserId = table.Column<Guid>(nullable: false),
                  UpdatedTimestamp = table.Column<DateTime>(nullable: true),
                  UpdatedByUserId = table.Column<Guid>(nullable: true),
                  IsDeleted = table.Column<bool>(nullable: false),
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_PersonDoor", t => new { t.PersonId, t.DoorId });
                  table.ForeignKey("FK_PersonDoor_CreatedUser", x => x.CreatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_UpdatedUser", x => x.UpdatedByUserId, "User", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_Person", x => x.PersonId, "Person", "Id", onDelete: ReferentialAction.Restrict);
                  table.ForeignKey("FK_PersonDoor_Door", x => x.DoorId, "Door", "Id", onDelete: ReferentialAction.Restrict);

              });

            migrationBuilder.CreateTable(
               name: "DoorRead",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                       .Annotation(Settings.UseSqlServer ? "SqlServer:ValueGenerationStrategy" : "MySql:ValueGeneratedOnAdd", Settings.UseSqlServer ? (object)SqlServerValueGenerationStrategy.IdentityColumn : true),
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

            migrationBuilder.AddColumn<string>(
              name: "TokenValue",
              table: "Person",
              nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpDown(migrationBuilder);
        }
    }
}
