using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNota.Migrations
{
    /// <inheritdoc />
    public partial class AddAudioRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorName",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "AudioRecordID",
                table: "AudioRecords",
                newName: "AudioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AudioID",
                table: "AudioRecords",
                newName: "AudioRecordID");

            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
