using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CZDMS.Db.Migrations
{
    public partial class Init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastWriteTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Key = table.Column<string>(nullable: true),
                    ParentId = table.Column<long>(nullable: true),
                    IsFolder = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    Data = table.Column<byte[]>(nullable: true),
                    SSMA_TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: false),
                    OwnerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Vorname = table.Column<string>(maxLength: 100, nullable: true),
                    Username = table.Column<string>(maxLength: 100, nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileItems");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
