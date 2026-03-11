namespace Adhyayan.Core.Models;

public class Grade
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public int GradeNumber { get; set; }                   // 1–5
    public string DisplayName { get; set; } = string.Empty; // e.g. "Class 3"

    // Navigation
    public Board Board { get; set; } = null!;
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
