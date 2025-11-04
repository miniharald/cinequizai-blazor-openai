using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineQuizAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       // QuizSession constraints
      migrationBuilder.Sql(@"
       ALTER TABLE quiz_session 
    ADD CONSTRAINT chk_quiz_session_difficulty 
   CHECK (difficulty IN ('Easy', 'Medium', 'Hard'))
      ");

            migrationBuilder.Sql(@"
        ALTER TABLE quiz_session 
      ADD CONSTRAINT chk_quiz_session_category 
     CHECK (category IN ('Movie', 'Tv'))
            ");

          migrationBuilder.Sql(@"
                ALTER TABLE quiz_session 
  ADD CONSTRAINT chk_quiz_session_visibility 
             CHECK (visibility IN ('Public', 'Friends', 'Private'))
      ");

     migrationBuilder.Sql(@"
                ALTER TABLE quiz_session 
       ADD CONSTRAINT chk_quiz_session_title_type 
     CHECK (title_type IN ('Movie', 'Tv'))
         ");

        migrationBuilder.Sql(@"
        ALTER TABLE quiz_session 
     ADD CONSTRAINT chk_quiz_session_question_count 
                CHECK (question_count >= 3 AND question_count <= 10)
   ");

        migrationBuilder.Sql(@"
    ALTER TABLE quiz_session 
     ADD CONSTRAINT chk_quiz_session_score 
     CHECK (score >= 0)
            ");

      // QuizQuestion constraints
            migrationBuilder.Sql(@"
           ALTER TABLE quiz_question 
         ADD CONSTRAINT chk_quiz_question_options_length 
   CHECK (jsonb_array_length(options) = 4)
   ");

        migrationBuilder.Sql(@"
      ALTER TABLE quiz_question 
   ADD CONSTRAINT chk_quiz_question_correct_index 
     CHECK (correct_index >= 0 AND correct_index <= 3)
      ");

       migrationBuilder.Sql(@"
  ALTER TABLE quiz_question 
    ADD CONSTRAINT chk_quiz_question_ordinal 
    CHECK (ordinal > 0)
            ");

            // QuestionAnswer constraints
            migrationBuilder.Sql(@"
                ALTER TABLE question_answer 
         ADD CONSTRAINT chk_question_answer_selected_index 
    CHECK (selected_index >= 0 AND selected_index <= 3)
   ");

  migrationBuilder.Sql(@"
      ALTER TABLE question_answer 
       ADD CONSTRAINT chk_question_answer_time_spent 
         CHECK (time_spent_ms >= 0)
          ");

  migrationBuilder.Sql(@"
             ALTER TABLE question_answer 
     ADD CONSTRAINT chk_question_answer_hints_used 
         CHECK (hints_used >= 0 AND hints_used <= 2)
  ");

migrationBuilder.Sql(@"
                ALTER TABLE question_answer 
             ADD CONSTRAINT chk_question_answer_hint_penalty 
        CHECK (hint_penalty >= 0)
      ");

        // TitleFactsSnapshot constraints
    migrationBuilder.Sql(@"
       ALTER TABLE title_facts_snapshot 
            ADD CONSTRAINT chk_title_facts_snapshot_title_type 
                CHECK (title_type IN ('Movie', 'Tv'))
");

            migrationBuilder.Sql(@"
      ALTER TABLE title_facts_snapshot 
  ADD CONSTRAINT chk_title_facts_snapshot_year 
         CHECK (year >= 1888 AND year <= 2100)
     ");

     // QuestionSet constraints
migrationBuilder.Sql(@"
        ALTER TABLE question_set 
          ADD CONSTRAINT chk_question_set_title_type 
     CHECK (title_type IN ('Movie', 'Tv'))
          ");

          migrationBuilder.Sql(@"
          ALTER TABLE question_set 
         ADD CONSTRAINT chk_question_set_difficulty 
   CHECK (difficulty IN ('Easy', 'Medium', 'Hard'))
            ");

            migrationBuilder.Sql(@"
              ALTER TABLE question_set 
        ADD CONSTRAINT chk_question_set_uses_count 
                CHECK (uses_count >= 0)
         ");

    migrationBuilder.Sql(@"
    ALTER TABLE question_set 
            ADD CONSTRAINT chk_question_set_prompt_version 
          CHECK (prompt_version > 0)
 ");

   // QuestionSetQuestion constraints
 migrationBuilder.Sql(@"
             ALTER TABLE question_set_question 
              ADD CONSTRAINT chk_question_set_question_options_length 
      CHECK (jsonb_array_length(options) = 4)
            ");

        migrationBuilder.Sql(@"
                ALTER TABLE question_set_question 
  ADD CONSTRAINT chk_question_set_question_correct_index 
         CHECK (correct_index >= 0 AND correct_index <= 3)
            ");

            migrationBuilder.Sql(@"
 ALTER TABLE question_set_question 
     ADD CONSTRAINT chk_question_set_question_ordinal 
  CHECK (ordinal > 0)
    ");

        // UserPreference constraints
            migrationBuilder.Sql(@"
       ALTER TABLE user_preference 
    ADD CONSTRAINT chk_user_preference_default_category 
     CHECK (default_category IN ('Movie', 'Tv'))
     ");

   migrationBuilder.Sql(@"
                ALTER TABLE user_preference 
           ADD CONSTRAINT chk_user_preference_default_difficulty 
   CHECK (default_difficulty IN ('Easy', 'Medium', 'Hard'))
        ");

    // Friendship constraints
            migrationBuilder.Sql(@"
ALTER TABLE friendship 
    ADD CONSTRAINT chk_friendship_status 
        CHECK (status IN ('Pending', 'Accepted', 'Blocked', 'Declined'))
     ");

    migrationBuilder.Sql(@"
  ALTER TABLE friendship 
         ADD CONSTRAINT chk_friendship_different_users 
          CHECK (requester_user_id <> addressee_user_id)
            ");

 // PrivateLeaderboard constraints
            migrationBuilder.Sql(@"
       ALTER TABLE private_leaderboard 
          ADD CONSTRAINT chk_private_leaderboard_visibility 
    CHECK (visibility IN ('FriendsOnly', 'InviteOnly'))
            ");

   // PrivateLeaderboardMember constraints
  migrationBuilder.Sql(@"
            ALTER TABLE private_leaderboard_member 
                ADD CONSTRAINT chk_private_leaderboard_member_role 
   CHECK (role IN ('Owner', 'Admin', 'Member'))
            ");

     // BlockedTitle constraints
   migrationBuilder.Sql(@"
    ALTER TABLE blocked_title 
     ADD CONSTRAINT chk_blocked_title_title_type 
                CHECK (title_type IN ('Movie', 'Tv'))
   ");

    // QuestionReport constraints
      migrationBuilder.Sql(@"
                ALTER TABLE question_report 
   ADD CONSTRAINT chk_question_report_status 
         CHECK (status IN ('Open', 'Reviewed', 'Dismissed'))
            ");
     }

    /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
    {
    // QuizSession
  migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_difficulty");
migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_category");
 migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_visibility");
            migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_title_type");
            migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_question_count");
     migrationBuilder.Sql("ALTER TABLE quiz_session DROP CONSTRAINT IF EXISTS chk_quiz_session_score");

            // QuizQuestion
  migrationBuilder.Sql("ALTER TABLE quiz_question DROP CONSTRAINT IF EXISTS chk_quiz_question_options_length");
            migrationBuilder.Sql("ALTER TABLE quiz_question DROP CONSTRAINT IF EXISTS chk_quiz_question_correct_index");
        migrationBuilder.Sql("ALTER TABLE quiz_question DROP CONSTRAINT IF EXISTS chk_quiz_question_ordinal");

        // QuestionAnswer
  migrationBuilder.Sql("ALTER TABLE question_answer DROP CONSTRAINT IF EXISTS chk_question_answer_selected_index");
      migrationBuilder.Sql("ALTER TABLE question_answer DROP CONSTRAINT IF EXISTS chk_question_answer_time_spent");
            migrationBuilder.Sql("ALTER TABLE question_answer DROP CONSTRAINT IF EXISTS chk_question_answer_hints_used");
  migrationBuilder.Sql("ALTER TABLE question_answer DROP CONSTRAINT IF EXISTS chk_question_answer_hint_penalty");

            // TitleFactsSnapshot
      migrationBuilder.Sql("ALTER TABLE title_facts_snapshot DROP CONSTRAINT IF EXISTS chk_title_facts_snapshot_title_type");
            migrationBuilder.Sql("ALTER TABLE title_facts_snapshot DROP CONSTRAINT IF EXISTS chk_title_facts_snapshot_year");

          // QuestionSet
            migrationBuilder.Sql("ALTER TABLE question_set DROP CONSTRAINT IF EXISTS chk_question_set_title_type");
            migrationBuilder.Sql("ALTER TABLE question_set DROP CONSTRAINT IF EXISTS chk_question_set_difficulty");
    migrationBuilder.Sql("ALTER TABLE question_set DROP CONSTRAINT IF EXISTS chk_question_set_uses_count");
          migrationBuilder.Sql("ALTER TABLE question_set DROP CONSTRAINT IF EXISTS chk_question_set_prompt_version");

            // QuestionSetQuestion
       migrationBuilder.Sql("ALTER TABLE question_set_question DROP CONSTRAINT IF EXISTS chk_question_set_question_options_length");
   migrationBuilder.Sql("ALTER TABLE question_set_question DROP CONSTRAINT IF EXISTS chk_question_set_question_correct_index");
       migrationBuilder.Sql("ALTER TABLE question_set_question DROP CONSTRAINT IF EXISTS chk_question_set_question_ordinal");

      // UserPreference
 migrationBuilder.Sql("ALTER TABLE user_preference DROP CONSTRAINT IF EXISTS chk_user_preference_default_category");
   migrationBuilder.Sql("ALTER TABLE user_preference DROP CONSTRAINT IF EXISTS chk_user_preference_default_difficulty");

        // Friendship
   migrationBuilder.Sql("ALTER TABLE friendship DROP CONSTRAINT IF EXISTS chk_friendship_status");
migrationBuilder.Sql("ALTER TABLE friendship DROP CONSTRAINT IF EXISTS chk_friendship_different_users");

            // PrivateLeaderboard
            migrationBuilder.Sql("ALTER TABLE private_leaderboard DROP CONSTRAINT IF EXISTS chk_private_leaderboard_visibility");

            // PrivateLeaderboardMember
 migrationBuilder.Sql("ALTER TABLE private_leaderboard_member DROP CONSTRAINT IF EXISTS chk_private_leaderboard_member_role");

// BlockedTitle
            migrationBuilder.Sql("ALTER TABLE blocked_title DROP CONSTRAINT IF EXISTS chk_blocked_title_title_type");

// QuestionReport
        migrationBuilder.Sql("ALTER TABLE question_report DROP CONSTRAINT IF EXISTS chk_question_report_status");
        }
  }
}
