using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forsvik.Core.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrigin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RetrievedCreatedFromImage",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentIndexes");

            migrationBuilder.DropColumn(
                name: "RetrievedCreatedFromImage",
                table: "Files");

            migrationBuilder.CreateTable(
                name: "DocumentsIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsIndexes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsIndexes_Key_Value",
                table: "DocumentsIndexes",
                columns: new[] { "Key", "Value" });
        }
    }
}
