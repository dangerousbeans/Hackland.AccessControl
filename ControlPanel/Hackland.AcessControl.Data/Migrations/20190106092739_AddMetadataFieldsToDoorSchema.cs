using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class AddMetadataFieldsToDoorSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql("delete from DoorRead");
            //migrationBuilder.Sql("delete from PersonDoor");
            //migrationBuilder.Sql("delete from Door");

            migrationBuilder.AddColumn<DateTime>(
               name: "CreatedTimestamp",
               table: "Door",
               nullable: false,
               defaultValueSql: "getdate()");
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
            //migrationBuilder.AddForeignKey("FK_Door_CreatedUser", "Door", "CreatedByUserId", "User", onDelete: ReferentialAction.NoAction);
            //migrationBuilder.AddForeignKey("FK_Door_UpdatedUser", "Door", "UpdatedByUserId", "User", onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey("FK_Door_CreatedUser", "Door");
            //migrationBuilder.DropForeignKey("FK_Door_UpdatedUser", "Door");
            migrationBuilder.DropColumn("CreatedTimestamp", "Door");
            migrationBuilder.DropColumn("CreatedByUserId", "Door");
            migrationBuilder.DropColumn("UpdatedTimestamp", "Door");
            migrationBuilder.DropColumn("UpdatedByUserId", "Door");
        }
    }
}
