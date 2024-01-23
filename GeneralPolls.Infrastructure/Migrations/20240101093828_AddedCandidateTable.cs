using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPolls.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCandidateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PollsDB",
                table: "PollsDB");

            migrationBuilder.RenameTable(
                name: "PollsDB",
                newName: "PollsTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollsTable",
                table: "PollsTable",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CandidateTable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ElectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoteCount = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PollsTable",
                table: "PollsTable");

            migrationBuilder.RenameTable(
                name: "PollsTable",
                newName: "PollsDB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PollsDB",
                table: "PollsDB",
                column: "Id");
        }
    }
}
