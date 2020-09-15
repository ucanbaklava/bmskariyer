using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // GLOBAL QUERY FILTERS
            builder.Entity<Question>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Announcement>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Organization>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<SubTopic>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Staff>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<PDFTest>().HasQueryFilter(e => !e.IsDeleted);

            // !GLOBAL QUERY FILTERS
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            // MAPPING SQL VIEWS
            builder
                .Entity<CommonSubject>(eb =>
                {
                    eb.HasNoKey();
                    eb.ToView("View_CommonSubjects");
                });
            // !MAPPING SQL VIEWS
            
            // ENTITIES FOR SP RESULTS
            builder.Entity<FieldSubject>().HasNoKey();
            builder.Entity<SolvedQuestionToday>().HasNoKey();
            builder.Entity<GetSubTopicsBySlug>().HasNoKey();
            builder.Entity<GetAllUsers>(e => e.HasNoKey());
            builder.Entity<LeaderboardResult>().HasNoKey();
            builder.Entity<GetUsersFiltered>().HasNoKey();
            builder.Entity<GetQuestionsFiltered>().HasNoKey();
            builder.Entity<GetUnionsFiltered>().HasNoKey();

            // !ENTITIES FOR SP RESULTS
            
            // SEED DATA
            builder.Entity<IdentityRole>(e => e.HasData(new List<IdentityRole>()
            {
                new IdentityRole(){Name = "admin", NormalizedName = "ADMIN"},
                new IdentityRole(){Name = "user", NormalizedName = "user".ToUpper()},
                new IdentityRole(){Name = "editor", NormalizedName = "EDITOR"}
            }));
            
            builder.Entity<Organization>().HasData(
                new Organization()
                {
                    Id = 1,
                    Name = "Adalet Bakanlığı",
                    OrgImage = "/assets/images/kurumlar/c85e4eb0-669f-4f19-a911-1c7e89b323ec.png"
                }
            );
            builder.Entity<Staff>().HasData(
                new Staff()
                {
                    Id = 1,

                     Name = "Yazı İşleri Müdürü", OrganizationId = 1
                }
            );
            builder.Entity<Subject>().HasData(
                new Subject()
                {
                    Id = 1,
                    Name = "GENEL KÜLTÜR",
                    Slug = "genel-kultur",
                    OrganizationId = null,
                },
                new Subject()
                {
                    Id = 2,
                    Name = "GENEL HUKUK BİLGİSİ",
                    Slug = "genel-hukuk-bilgisi",
                    OrganizationId = 1,
                }
            );
            builder.Entity<SubTopic>().HasData(
                new SubTopic()
                {
                    Id = 1,
                    Name = "GENEL KÜLTÜR",
                    Slug = "genel-kultur",
                    SubjectId = 1
                },
                new SubTopic()
                {
                    Id = 2,
                    Name = "ADALET BAKANLIĞI DİSİPLİN YÖNETMELİĞİ",
                    Slug = "adalet-bakanligi-disiplin-yonetmeligi",
                    SubjectId = 2
                }
            );
            

            // !SEED DATA
        }



        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Options> Options { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<SubTopic> SubTopic { get; set; }
        public DbSet<LectureNote> LectureNotes { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<PDFTest> PdfTest { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UnionBranch> UnionBranches { get; set; }

        public DbSet<PdfTestResult> PdfTestResult { get; set; }
        public DbSet<QuestionReport> QuestionReports { get; set; }

        [NotMapped]
        public DbSet<GetAllUsers> GetAllUsers { get; set; }
        public DbSet<CommonSubject> CommonSubjects { get; set; }
        public DbSet<FieldSubject> FieldSubjects { get; set; }
        public DbSet<LeaderboardResult> LeaderboardResults { get; set; }
        public DbSet<GetUsersFiltered> GetUsersFiltered { get; set; }
        public DbSet<GetQuestionsFiltered> GetQuestionsFiltered { get; set; }
        public DbSet<GetUnionsFiltered> GetUnionsFiltered { get; set; }

        public DbSet<SolvedQuestionToday> SolvedQuestionToday { get; set; }
        public DbSet<GetSubTopicsBySlug> GetSubTopicsBySlug { get; set; }



    }
}