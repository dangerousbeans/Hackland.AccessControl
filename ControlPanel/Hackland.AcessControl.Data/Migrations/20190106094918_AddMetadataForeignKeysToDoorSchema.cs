using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hackland.AccessControl.Data.Migrations
{
    public partial class AddMetadataForeignKeysToDoorSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddForeignKey("FK_Door_CreatedUser", "Door", "CreatedByUserId", "User", onDelete: ReferentialAction.NoAction);
            //migrationBuilder.AddForeignKey("FK_Door_UpdatedUser", "Door", "UpdatedByUserId", "User", onDelete: ReferentialAction.NoAction);
            migrationBuilder.Sql("ALTER TABLE Door WITH CHECK ADD CONSTRAINT FK_Door_CreatedUser FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id)");
            migrationBuilder.Sql("ALTER TABLE Door WITH CHECK ADD CONSTRAINT FK_Door_UpdatedUser FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_Door_CreatedUser", "Door");
            migrationBuilder.DropForeignKey("FK_Door_UpdatedUser", "Door");
        }
    }
}
