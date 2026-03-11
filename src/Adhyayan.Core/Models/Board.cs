namespace Adhyayan.Core.Models;

public class Board
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;      // e.g. "cbse", "maharashtra", "karnataka"
    public string Name { get; set; } = string.Empty;       // e.g. "CBSE", "Maharashtra State Board"
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsPrimary { get; set; }                    // true for CBSE
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public ICollection<ConceptMapping> ConceptMappings { get; set; } = new List<ConceptMapping>();
}
