using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNota.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRatingAddVisitSummaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorRating",
                table: "AISummaries");

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpDate",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhenToSeekHelp",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "WhenToSeekHelp",
                table: "Visits");

            migrationBuilder.AddColumn<float>(
                name: "DoctorRating",
                table: "AISummaries",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
