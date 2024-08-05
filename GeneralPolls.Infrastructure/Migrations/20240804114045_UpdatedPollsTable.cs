using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPolls.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPollsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateCount",
                table: "PollsTable");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PollsTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PollsTable");

            migrationBuilder.AddColumn<int>(
                name: "CandidateCount",
                table: "PollsTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
