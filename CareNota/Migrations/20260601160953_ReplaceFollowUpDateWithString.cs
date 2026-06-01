using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNota.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFollowUpDateWithString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "Visits");

            migrationBuilder.AddColumn<string>(
                name: "FollowUp",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUp",
                table: "Visits");

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpDate",
                table: "Visits",
                type: "datetime2",
                nullable: true);
        }
    }
}
