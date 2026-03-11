namespace Adhyayan.Core.Models;

public class Child
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GradeNumber { get; set; }
    public int BoardId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Parent Parent { get; set; } = null!;
    public Board Board { get; set; } = null!;
    public ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
}
