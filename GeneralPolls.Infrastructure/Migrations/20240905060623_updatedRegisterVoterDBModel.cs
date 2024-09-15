using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPolls.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedRegisterVoterDBModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RegisteredVotersTable",
                newName: "VoterId");

            migrationBuilder.AddColumn<string>(
                name: "VoterEmail",
                table: "RegisteredVotersTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoterEmail",
                table: "RegisteredVotersTable");

            migrationBuilder.RenameColumn(
                name: "VoterId",
                table: "RegisteredVotersTable",
                newName: "UserId");
        }
    }
}
