using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CZDMS.Db.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastWriteTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    IsFolder = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
                    Data = table.Column<byte[]>(nullable: true),
                    OptimisticLockField = table.Column<int>(nullable: true),
                    Gcrecord = table.Column<int>(nullable: true),
                    SSMA_TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileItems");
        }
    }
}
