using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MagnumPhotos.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Photographers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false),
                    DateOfDeath = table.Column<DateTimeOffset>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    Genre = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photographers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CopyrightDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    PhotographerId = table.Column<Guid>(nullable: false),
                    PhotographerId1 = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Photographers_PhotographerId",
                        column: x => x.PhotographerId,
                        principalTable: "Photographers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Photographers_PhotographerId1",
                        column: x => x.PhotographerId1,
                        principalTable: "Photographers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_PhotographerId",
                table: "Books",
                column: "PhotographerId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PhotographerId1",
                table: "Books",
                column: "PhotographerId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Photographers");
        }
    }
}
