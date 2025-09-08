using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicCatalog.Migrations
{
    /// <inheritdoc />
    public partial class ChangeYearToRecordingDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Musics");

            migrationBuilder.AddColumn<DateOnly>(
                name: "RecordingDate",
                table: "Musics",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordingDate",
                table: "Musics");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Musics",
                type: "integer",
                nullable: true);
        }
    }
}
