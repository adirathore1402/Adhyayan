namespace Adhyayan.Core.Models;

public class Chapter
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int GradeId { get; set; }
    public int SubjectId { get; set; }
    public int ChapterNumber { get; set; }
    public string Name { get; set; } = string.Empty;       // e.g. "Fractions"
    public string Description { get; set; } = string.Empty;

    // Navigation
    public Board Board { get; set; } = null!;
    public Grade Grade { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
