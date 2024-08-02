using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPolls.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PictureToCandidateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CandidatePicturePath",
                table: "CandidateTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidatePicturePath",
                table: "CandidateTable");
        }
    }
}
