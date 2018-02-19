using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LibraryApi.Migrations
{
    public partial class AddDateOfDeath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId1",
                table: "Books",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Authors",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfDeath",
                table: "Authors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId1",
                table: "Books",
                column: "AuthorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId1",
                table: "Books",
                column: "AuthorId1",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId1",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Authors",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
