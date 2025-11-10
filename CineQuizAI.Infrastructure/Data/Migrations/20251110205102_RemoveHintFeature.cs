using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineQuizAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHintFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hint_mode",
                table: "quiz_session");

            migrationBuilder.DropColumn(
                name: "hint_penalty",
                table: "question_answer");

            migrationBuilder.DropColumn(
                name: "hints_used",
                table: "question_answer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hint_mode",
                table: "quiz_session",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "hint_penalty",
                table: "question_answer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "hints_used",
                table: "question_answer",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
