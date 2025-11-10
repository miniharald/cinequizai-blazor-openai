using System;
using CineQuizAI.Domain.Entities;
using CineQuizAI.Domain.Enums;
using CineQuizAI.Domain.ValueObjects;
using CineQuizAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineQuizAI.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      // Core Quiz
        public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
        public DbSet<TitleFactsSnapshot> TitleFactsSnapshots => Set<TitleFactsSnapshot>();
        public DbSet<QuizSession> QuizSessions => Set<QuizSession>();
        public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
        public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();

        // Question Pool System
        public DbSet<QuestionSet> QuestionSets => Set<QuestionSet>();
        public DbSet<QuestionSetQuestion> QuestionSetQuestions => Set<QuestionSetQuestion>();
        public DbSet<UserQuestionSetPlay> UserQuestionSetPlays => Set<UserQuestionSetPlay>();

        // Social Features
        public DbSet<Friendship> Friendships => Set<Friendship>();
        public DbSet<PrivateLeaderboard> PrivateLeaderboards => Set<PrivateLeaderboard>();
    public DbSet<PrivateLeaderboardMember> PrivateLeaderboardMembers => Set<PrivateLeaderboardMember>();

   // Admin & Moderation
        public DbSet<BlockedTitle> BlockedTitles => Set<BlockedTitle>();
        public DbSet<QuestionReport> QuestionReports => Set<QuestionReport>();

        protected override void OnModelCreating(ModelBuilder builder)
 {
       base.OnModelCreating(builder);

   // Configure PostgreSQL naming convention (snake_case)
        ConfigureNamingConvention(builder);

   // Configure entities
 ConfigureUserPreference(builder);
    ConfigureTitleFactsSnapshot(builder);
ConfigureQuizSession(builder);
   ConfigureQuizQuestion(builder);
       ConfigureQuestionAnswer(builder);
            ConfigureQuestionSet(builder);
    ConfigureQuestionSetQuestion(builder);
  ConfigureUserQuestionSetPlay(builder);
         ConfigureFriendship(builder);
     ConfigurePrivateLeaderboard(builder);
      ConfigurePrivateLeaderboardMember(builder);
   ConfigureBlockedTitle(builder);
            ConfigureQuestionReport(builder);
        }

        private void ConfigureNamingConvention(ModelBuilder builder)
        {
         // Map table names to snake_case
         builder.Entity<UserPreference>().ToTable("user_preference");
       builder.Entity<TitleFactsSnapshot>().ToTable("title_facts_snapshot");
     builder.Entity<QuizSession>().ToTable("quiz_session");
            builder.Entity<QuizQuestion>().ToTable("quiz_question");
      builder.Entity<QuestionAnswer>().ToTable("question_answer");
          builder.Entity<QuestionSet>().ToTable("question_set");
            builder.Entity<QuestionSetQuestion>().ToTable("question_set_question");
     builder.Entity<UserQuestionSetPlay>().ToTable("user_question_set_play");
    builder.Entity<Friendship>().ToTable("friendship");
            builder.Entity<PrivateLeaderboard>().ToTable("private_leaderboard");
        builder.Entity<PrivateLeaderboardMember>().ToTable("private_leaderboard_member");
            builder.Entity<BlockedTitle>().ToTable("blocked_title");
         builder.Entity<QuestionReport>().ToTable("question_report");
        }

     private void ConfigureUserPreference(ModelBuilder builder)
     {
            builder.Entity<UserPreference>(entity =>
   {
    entity.HasKey(e => e.UserId);
      entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.DefaultCategory)
       .HasColumnName("default_category")
     .HasConversion<string>()
    .HasMaxLength(20);
      entity.Property(e => e.DefaultDifficulty)
              .HasColumnName("default_difficulty")
                 .HasConversion<string>()
            .HasMaxLength(20);
        entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(10);
    entity.Property(e => e.CreatedAt).HasColumnName("created_at");
         entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

         // Foreign key to AspNetUsers
      entity.HasOne<AppUser>()
              .WithMany()
          .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    });
        }

        private void ConfigureTitleFactsSnapshot(ModelBuilder builder)
   {
            builder.Entity<TitleFactsSnapshot>(entity =>
            {
  entity.HasKey(e => e.Id);
       entity.Property(e => e.Id).HasColumnName("id");
   entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");
   entity.Property(e => e.TitleType)
    .HasColumnName("title_type")
   .HasConversion<string>()
          .HasMaxLength(20);
    entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(10);
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(500);
   entity.Property(e => e.Overview).HasColumnName("overview");
        entity.Property(e => e.Year).HasColumnName("year");
      
   // JSONB columns
  entity.Property(e => e.Genres).HasColumnName("genres").HasColumnType("jsonb");
      entity.Property(e => e.CastTop).HasColumnName("cast_top").HasColumnType("jsonb");
           entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasColumnType("jsonb");
          entity.Property(e => e.SeasonsCount).HasColumnName("seasons_count");
     entity.Property(e => e.ProductionCompanies).HasColumnName("production_companies").HasColumnType("jsonb");
          entity.Property(e => e.OriginCountry).HasColumnName("origin_country").HasColumnType("jsonb");
   entity.Property(e => e.OriginalLanguage).HasColumnName("original_language").HasMaxLength(10);
                entity.Property(e => e.PosterUrl).HasColumnName("poster_url").HasMaxLength(500);
        entity.Property(e => e.Keywords).HasColumnName("keywords").HasColumnType("jsonb");
    entity.Property(e => e.BelongsToCollection).HasColumnName("belongs_to_collection").HasColumnType("jsonb");
    entity.Property(e => e.FetchedAt).HasColumnName("fetched_at");

                // Unique constraint
   entity.HasIndex(e => new { e.TmdbId, e.TitleType, e.LanguageCode }).IsUnique();
            });
        }

        private void ConfigureQuizSession(ModelBuilder builder)
      {
builder.Entity<QuizSession>(entity =>
   {
                entity.HasKey(e => e.Id);
entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.UserId).HasColumnName("user_id");
         entity.Property(e => e.Visibility)
         .HasColumnName("visibility")
              .HasConversion<string>()
       .HasMaxLength(20);
      entity.Property(e => e.Category)
              .HasColumnName("category")
          .HasConversion<string>()
         .HasMaxLength(20);
entity.Property(e => e.Difficulty)
         .HasColumnName("difficulty")
     .HasConversion<string>()
    .HasMaxLength(20);
          entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(10);
       entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(500);
   entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");
           entity.Property(e => e.TitleType)
       .HasColumnName("title_type")
        .HasConversion<string>()
        .HasMaxLength(20);
  entity.Property(e => e.SnapshotId).HasColumnName("snapshot_id");
            entity.Property(e => e.QuestionCount).HasColumnName("question_count");
            entity.Property(e => e.Score).HasColumnName("score");
       entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");

     // Foreign keys
    entity.HasOne(e => e.Snapshot)
            .WithMany(s => s.QuizSessions)
    .HasForeignKey(e => e.SnapshotId)
                .OnDelete(DeleteBehavior.Restrict);

         entity.HasOne<AppUser>()
       .WithMany()
       .HasForeignKey(e => e.UserId)
  .OnDelete(DeleteBehavior.SetNull);

       // Indexes
    entity.HasIndex(e => e.FinishedAt).HasDatabaseName("ix_quiz_session_finished_at");
            entity.HasIndex(e => new { e.UserId, e.FinishedAt }).HasDatabaseName("ix_quiz_session_user_finished");

           // Check constraints will be added in migration
      });
        }

private void ConfigureQuizQuestion(ModelBuilder builder)
        {
   builder.Entity<QuizQuestion>(entity =>
  {
           entity.HasKey(e => e.Id);
entity.Property(e => e.Id).HasColumnName("id");
       entity.Property(e => e.SessionId).HasColumnName("session_id");
                entity.Property(e => e.Ordinal).HasColumnName("ordinal");
         entity.Property(e => e.Text).HasColumnName("text");
      entity.Property(e => e.Options).HasColumnName("options").HasColumnType("jsonb");
            entity.Property(e => e.CorrectIndex).HasColumnName("correct_index");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
 entity.Property(e => e.SourceReference).HasColumnName("source_reference").HasMaxLength(200);
   entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            // Foreign key
             entity.HasOne(e => e.Session)
  .WithMany(s => s.Questions)
           .HasForeignKey(e => e.SessionId)
  .OnDelete(DeleteBehavior.Cascade);

  // Index
           entity.HasIndex(e => new { e.SessionId, e.Ordinal }).HasDatabaseName("ix_quiz_question_session_ordinal");
 });
 }

      private void ConfigureQuestionAnswer(ModelBuilder builder)
        {
          builder.Entity<QuestionAnswer>(entity =>
   {
  entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
      entity.Property(e => e.QuestionId).HasColumnName("question_id");
      entity.Property(e => e.SelectedIndex).HasColumnName("selected_index");
                entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
     entity.Property(e => e.TimeSpentMs).HasColumnName("time_spent_ms");
          entity.Property(e => e.AnsweredAt).HasColumnName("answered_at");

      // Foreign key (one-to-one)
                entity.HasOne(e => e.Question)
      .WithOne(q => q.Answer)
               .HasForeignKey<QuestionAnswer>(e => e.QuestionId)
   .OnDelete(DeleteBehavior.Cascade);
      });
    }

        private void ConfigureQuestionSet(ModelBuilder builder)
   {
            builder.Entity<QuestionSet>(entity =>
            {
         entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
      entity.Property(e => e.TitleType)
       .HasColumnName("title_type")
          .HasConversion<string>()
      .HasMaxLength(20);
                entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");
                entity.Property(e => e.Difficulty)
        .HasColumnName("difficulty")
    .HasConversion<string>()
 .HasMaxLength(20);
    entity.Property(e => e.LanguageCode).HasColumnName("language_code").HasMaxLength(10);
         entity.Property(e => e.FactsHash).HasColumnName("facts_hash").HasMaxLength(64);
entity.Property(e => e.PromptVersion).HasColumnName("prompt_version");
 entity.Property(e => e.CreatedAt).HasColumnName("created_at");
    entity.Property(e => e.UsesCount).HasColumnName("uses_count");
          entity.Property(e => e.IsActive).HasColumnName("is_active");

      // Index for lookup
    entity.HasIndex(e => new { e.TmdbId, e.TitleType, e.Difficulty, e.LanguageCode, e.IsActive })
             .HasDatabaseName("ix_question_set_lookup");
            });
 }

        private void ConfigureQuestionSetQuestion(ModelBuilder builder)
{
            builder.Entity<QuestionSetQuestion>(entity =>
  {
    entity.HasKey(e => e.Id);
     entity.Property(e => e.Id).HasColumnName("id");
  entity.Property(e => e.SetId).HasColumnName("set_id");
           entity.Property(e => e.Ordinal).HasColumnName("ordinal");
 entity.Property(e => e.Text).HasColumnName("text");
      entity.Property(e => e.Options).HasColumnName("options").HasColumnType("jsonb");
        entity.Property(e => e.CorrectIndex).HasColumnName("correct_index");
       entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
          entity.Property(e => e.SourceReference).HasColumnName("source_reference").HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

          // Foreign key
     entity.HasOne(e => e.Set)
  .WithMany(s => s.Questions)
    .HasForeignKey(e => e.SetId)
              .OnDelete(DeleteBehavior.Cascade);

        // Index
       entity.HasIndex(e => new { e.SetId, e.Ordinal }).HasDatabaseName("ix_question_set_question_ordinal");
 });
        }

     private void ConfigureUserQuestionSetPlay(ModelBuilder builder)
        {
            builder.Entity<UserQuestionSetPlay>(entity =>
       {
   entity.HasKey(e => new { e.UserId, e.SetId });
   entity.Property(e => e.UserId).HasColumnName("user_id");
       entity.Property(e => e.SetId).HasColumnName("set_id");
    entity.Property(e => e.PlayedAt).HasColumnName("played_at");

    // Foreign keys
                entity.HasOne<AppUser>()
     .WithMany()
          .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Set)
            .WithMany(s => s.Plays)
           .HasForeignKey(e => e.SetId)
             .OnDelete(DeleteBehavior.Cascade);
            });
    }

        private void ConfigureFriendship(ModelBuilder builder)
        {
            builder.Entity<Friendship>(entity =>
            {
 entity.HasKey(e => e.Id);
  entity.Property(e => e.Id).HasColumnName("id");
  entity.Property(e => e.RequesterUserId).HasColumnName("requester_user_id");
                entity.Property(e => e.AddresseeUserId).HasColumnName("addressee_user_id");
    entity.Property(e => e.Status)
     .HasColumnName("status")
          .HasConversion<string>()
  .HasMaxLength(20);
       entity.Property(e => e.CreatedAt).HasColumnName("created_at");
       entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

      // Foreign keys
                entity.HasOne<AppUser>()
        .WithMany()
           .HasForeignKey(e => e.RequesterUserId)
         .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne<AppUser>()
      .WithMany()
                  .HasForeignKey(e => e.AddresseeUserId)
             .OnDelete(DeleteBehavior.Cascade);

      // Index
                entity.HasIndex(e => new { e.Status, e.UpdatedAt }).HasDatabaseName("ix_friendship_status_updated");
            });
        }

        private void ConfigurePrivateLeaderboard(ModelBuilder builder)
   {
            builder.Entity<PrivateLeaderboard>(entity =>
 {
            entity.HasKey(e => e.Id);
       entity.Property(e => e.Id).HasColumnName("id");
     entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
    entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200);
          entity.Property(e => e.InviteCode).HasColumnName("invite_code").HasMaxLength(50);
    entity.Property(e => e.Visibility)
         .HasColumnName("visibility")
        .HasConversion<string>()
                  .HasMaxLength(20);
     entity.Property(e => e.CreatedAt).HasColumnName("created_at");

           // Foreign key
                entity.HasOne<AppUser>()
                    .WithMany()
         .HasForeignKey(e => e.OwnerUserId)
        .OnDelete(DeleteBehavior.Cascade);

      // Unique invite code
                entity.HasIndex(e => e.InviteCode).IsUnique();
       });
        }

        private void ConfigurePrivateLeaderboardMember(ModelBuilder builder)
        {
            builder.Entity<PrivateLeaderboardMember>(entity =>
   {
  entity.HasKey(e => new { e.LeaderboardId, e.UserId });
     entity.Property(e => e.LeaderboardId).HasColumnName("leaderboard_id");
       entity.Property(e => e.UserId).HasColumnName("user_id");
  entity.Property(e => e.Role)
           .HasColumnName("role")
             .HasConversion<string>()
     .HasMaxLength(20);
      entity.Property(e => e.JoinedAt).HasColumnName("joined_at");

      // Foreign keys
       entity.HasOne(e => e.Leaderboard)
      .WithMany(l => l.Members)
           .HasForeignKey(e => e.LeaderboardId)
 .OnDelete(DeleteBehavior.Cascade);

  entity.HasOne<AppUser>()
        .WithMany()
      .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

   // Index
     entity.HasIndex(e => e.LeaderboardId).HasDatabaseName("ix_private_leaderboard_member_leaderboard");
         });
      }

        private void ConfigureBlockedTitle(ModelBuilder builder)
   {
            builder.Entity<BlockedTitle>(entity =>
    {
           entity.HasKey(e => e.Id);
           entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");
         entity.Property(e => e.TitleType)
       .HasColumnName("title_type")
         .HasConversion<string>()
        .HasMaxLength(20);
          entity.Property(e => e.Reason).HasColumnName("reason");
     entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
      entity.Property(e => e.CreatedAt).HasColumnName("created_at");

        // Foreign key
      entity.HasOne<AppUser>()
         .WithMany()
             .HasForeignKey(e => e.CreatedByUserId)
      .OnDelete(DeleteBehavior.Restrict);

              // Unique constraint
        entity.HasIndex(e => new { e.TmdbId, e.TitleType }).IsUnique();
            });
        }

        private void ConfigureQuestionReport(ModelBuilder builder)
      {
       builder.Entity<QuestionReport>(entity =>
            {
        entity.HasKey(e => e.Id);
              entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.QuestionId).HasColumnName("question_id");
              entity.Property(e => e.ReporterUserId).HasColumnName("reporter_user_id");
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
         entity.Property(e => e.Status)
    .HasColumnName("status")
          .HasConversion<string>()
        .HasMaxLength(20);

           // Foreign keys
       entity.HasOne(e => e.Question)
          .WithMany()
             .HasForeignKey(e => e.QuestionId)
           .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne<AppUser>()
        .WithMany()
          .HasForeignKey(e => e.ReporterUserId)
         .OnDelete(DeleteBehavior.Cascade);
  });
        }
    }
}
