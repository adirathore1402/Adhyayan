using Adhyayan.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Adhyayan.Infrastructure.Data;

public class AdhyayanDbContext : DbContext
{
    public AdhyayanDbContext(DbContextOptions<AdhyayanDbContext> options) : base(options) { }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<ConceptMapping> ConceptMappings => Set<ConceptMapping>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
    public DbSet<PracticeAnswer> PracticeAnswers => Set<PracticeAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Board
        modelBuilder.Entity<Board>(e =>
        {
            e.HasIndex(b => b.Code).IsUnique();
            e.Property(b => b.Code).HasMaxLength(50);
            e.Property(b => b.Name).HasMaxLength(200);
        });

        // Grade
        modelBuilder.Entity<Grade>(e =>
        {
            e.HasOne(g => g.Board).WithMany(b => b.Grades).HasForeignKey(g => g.BoardId);
            e.HasIndex(g => new { g.BoardId, g.GradeNumber }).IsUnique();
        });

        // Subject
        modelBuilder.Entity<Subject>(e =>
        {
            e.HasIndex(s => s.Code).IsUnique();
            e.Property(s => s.Code).HasMaxLength(50);
            e.Property(s => s.Name).HasMaxLength(200);
        });

        // Chapter
        modelBuilder.Entity<Chapter>(e =>
        {
            e.HasOne(c => c.Board).WithMany(b => b.Chapters).HasForeignKey(c => c.BoardId);
            e.HasOne(c => c.Grade).WithMany(g => g.Chapters).HasForeignKey(c => c.GradeId);
            e.HasOne(c => c.Subject).WithMany(s => s.Chapters).HasForeignKey(c => c.SubjectId);
            e.HasIndex(c => new { c.BoardId, c.GradeId, c.SubjectId, c.ChapterNumber }).IsUnique();
        });

        // Topic
        modelBuilder.Entity<Topic>(e =>
        {
            e.HasOne(t => t.Chapter).WithMany(c => c.Topics).HasForeignKey(t => t.ChapterId);
        });

        // ConceptMapping
        modelBuilder.Entity<ConceptMapping>(e =>
        {
            e.HasOne(cm => cm.Board).WithMany(b => b.ConceptMappings).HasForeignKey(cm => cm.BoardId);
            e.HasOne(cm => cm.Chapter).WithMany().HasForeignKey(cm => cm.ChapterId);
            e.HasIndex(cm => new { cm.ConceptId, cm.BoardId }).IsUnique();
        });

        // Question
        modelBuilder.Entity<Question>(e =>
        {
            e.HasOne(q => q.Chapter).WithMany(c => c.Questions).HasForeignKey(q => q.ChapterId);
            e.HasOne(q => q.Topic).WithMany(t => t.Questions).HasForeignKey(q => q.TopicId);
        });

        // Parent
        modelBuilder.Entity<Parent>(e =>
        {
            e.HasIndex(p => p.Email).IsUnique();
            e.Property(p => p.Email).HasMaxLength(256);
        });

        // Child
        modelBuilder.Entity<Child>(e =>
        {
            e.HasOne(c => c.Parent).WithMany(p => p.Children).HasForeignKey(c => c.ParentId);
            e.HasOne(c => c.Board).WithMany().HasForeignKey(c => c.BoardId);
        });

        // PracticeSession
        modelBuilder.Entity<PracticeSession>(e =>
        {
            e.HasOne(ps => ps.Child).WithMany(c => c.PracticeSessions).HasForeignKey(ps => ps.ChildId);
            e.HasOne(ps => ps.Chapter).WithMany().HasForeignKey(ps => ps.ChapterId);
        });

        // PracticeAnswer
        modelBuilder.Entity<PracticeAnswer>(e =>
        {
            e.HasOne(pa => pa.Session).WithMany(ps => ps.Answers).HasForeignKey(pa => pa.SessionId);
            e.HasOne(pa => pa.Question).WithMany().HasForeignKey(pa => pa.QuestionId);
        });
    }
}
