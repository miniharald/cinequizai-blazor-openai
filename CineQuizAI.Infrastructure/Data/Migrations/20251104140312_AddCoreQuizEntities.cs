using System;
using System.Collections.Generic;
using CineQuizAI.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineQuizAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreQuizEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blocked_title",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<long>(type: "bigint", nullable: false),
                    title_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blocked_title", x => x.id);
                    table.ForeignKey(
                        name: "FK_blocked_title_AspNetUsers_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "friendship",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    addressee_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_AspNetUsers_addressee_user_id",
                        column: x => x.addressee_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendship_AspNetUsers_requester_user_id",
                        column: x => x.requester_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "private_leaderboard",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    invite_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    visibility = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_private_leaderboard", x => x.id);
                    table.ForeignKey(
                        name: "FK_private_leaderboard_AspNetUsers_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_set",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tmdb_id = table.Column<long>(type: "bigint", nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    facts_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    prompt_version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uses_count = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_set", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "title_facts_snapshot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tmdb_id = table.Column<long>(type: "bigint", nullable: false),
                    title_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    overview = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    genres = table.Column<List<string>>(type: "jsonb", nullable: false),
                    cast_top = table.Column<List<CastMember>>(type: "jsonb", nullable: false),
                    created_by = table.Column<List<string>>(type: "jsonb", nullable: false),
                    seasons_count = table.Column<int>(type: "integer", nullable: true),
                    production_companies = table.Column<List<string>>(type: "jsonb", nullable: false),
                    origin_country = table.Column<List<string>>(type: "jsonb", nullable: false),
                    original_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    poster_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    keywords = table.Column<List<string>>(type: "jsonb", nullable: false),
                    belongs_to_collection = table.Column<Collection>(type: "jsonb", nullable: true),
                    fetched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_title_facts_snapshot", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_preference",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preference", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_preference_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "private_leaderboard_member",
                columns: table => new
                {
                    leaderboard_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_private_leaderboard_member", x => new { x.leaderboard_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_private_leaderboard_member_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_private_leaderboard_member_private_leaderboard_leaderboard_~",
                        column: x => x.leaderboard_id,
                        principalTable: "private_leaderboard",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_set_question",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    options = table.Column<List<string>>(type: "jsonb", nullable: false),
                    correct_index = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_set_question", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_set_question_question_set_set_id",
                        column: x => x.set_id,
                        principalTable: "question_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_question_set_play",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    played_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_question_set_play", x => new { x.user_id, x.set_id });
                    table.ForeignKey(
                        name: "FK_user_question_set_play_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_question_set_play_question_set_set_id",
                        column: x => x.set_id,
                        principalTable: "question_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    visibility = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tmdb_id = table.Column<long>(type: "bigint", nullable: false),
                    title_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hint_mode = table.Column<bool>(type: "boolean", nullable: false),
                    question_count = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_quiz_session_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_quiz_session_title_facts_snapshot_snapshot_id",
                        column: x => x.snapshot_id,
                        principalTable: "title_facts_snapshot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_question",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    options = table.Column<List<string>>(type: "jsonb", nullable: false),
                    correct_index = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_question", x => x.id);
                    table.ForeignKey(
                        name: "FK_quiz_question_quiz_session_session_id",
                        column: x => x.session_id,
                        principalTable: "quiz_session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_index = table.Column<int>(type: "integer", nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    time_spent_ms = table.Column<int>(type: "integer", nullable: false),
                    hints_used = table.Column<short>(type: "smallint", nullable: false),
                    hint_penalty = table.Column<int>(type: "integer", nullable: false),
                    answered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_answer", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_answer_quiz_question_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reporter_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_report_AspNetUsers_reporter_user_id",
                        column: x => x.reporter_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_report_quiz_question_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blocked_title_created_by_user_id",
                table: "blocked_title",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_blocked_title_tmdb_id_title_type",
                table: "blocked_title",
                columns: new[] { "tmdb_id", "title_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_friendship_addressee_user_id",
                table: "friendship",
                column: "addressee_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_requester_user_id",
                table: "friendship",
                column: "requester_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_friendship_status_updated",
                table: "friendship",
                columns: new[] { "status", "updated_at" });

            migrationBuilder.CreateIndex(
                name: "IX_private_leaderboard_invite_code",
                table: "private_leaderboard",
                column: "invite_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_private_leaderboard_owner_user_id",
                table: "private_leaderboard",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_private_leaderboard_member_leaderboard",
                table: "private_leaderboard_member",
                column: "leaderboard_id");

            migrationBuilder.CreateIndex(
                name: "IX_private_leaderboard_member_user_id",
                table: "private_leaderboard_member",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_answer_question_id",
                table: "question_answer",
                column: "question_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_question_report_question_id",
                table: "question_report",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_report_reporter_user_id",
                table: "question_report",
                column: "reporter_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_set_lookup",
                table: "question_set",
                columns: new[] { "tmdb_id", "title_type", "difficulty", "language_code", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_question_set_question_ordinal",
                table: "question_set_question",
                columns: new[] { "set_id", "ordinal" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_session_ordinal",
                table: "quiz_question",
                columns: new[] { "session_id", "ordinal" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_session_finished_at",
                table: "quiz_session",
                column: "finished_at");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_session_snapshot_id",
                table: "quiz_session",
                column: "snapshot_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_session_user_finished",
                table: "quiz_session",
                columns: new[] { "user_id", "finished_at" });

            migrationBuilder.CreateIndex(
                name: "IX_title_facts_snapshot_tmdb_id_title_type_language_code",
                table: "title_facts_snapshot",
                columns: new[] { "tmdb_id", "title_type", "language_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_question_set_play_set_id",
                table: "user_question_set_play",
                column: "set_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blocked_title");

            migrationBuilder.DropTable(
                name: "friendship");

            migrationBuilder.DropTable(
                name: "private_leaderboard_member");

            migrationBuilder.DropTable(
                name: "question_answer");

            migrationBuilder.DropTable(
                name: "question_report");

            migrationBuilder.DropTable(
                name: "question_set_question");

            migrationBuilder.DropTable(
                name: "user_preference");

            migrationBuilder.DropTable(
                name: "user_question_set_play");

            migrationBuilder.DropTable(
                name: "private_leaderboard");

            migrationBuilder.DropTable(
                name: "quiz_question");

            migrationBuilder.DropTable(
                name: "question_set");

            migrationBuilder.DropTable(
                name: "quiz_session");

            migrationBuilder.DropTable(
                name: "title_facts_snapshot");
        }
    }
}
