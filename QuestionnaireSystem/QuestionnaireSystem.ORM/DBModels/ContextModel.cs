using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QuestionnaireSystem.ORM.DBModels
{
    public partial class ContextModel : DbContext
    {
        public ContextModel()
            : base("name=ConnectionString")
        {
        }

        public virtual DbSet<Common> Commons { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<ReplyInfo> ReplyInfoes { get; set; }
        public virtual DbSet<Static> Statics { get; set; }
        public virtual DbSet<SystemInfo> SystemInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReplyInfo>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<SystemInfo>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<SystemInfo>()
                .Property(e => e.Account)
                .IsUnicode(false);

            modelBuilder.Entity<SystemInfo>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
