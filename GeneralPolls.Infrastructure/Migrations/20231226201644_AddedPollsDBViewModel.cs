using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPolls.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPollsDBViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollsDB",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ElectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CandidateCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollsDB", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollsDB");
        }
    }
}
