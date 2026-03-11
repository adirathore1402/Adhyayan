namespace Adhyayan.Core.Models;

/// <summary>
/// Maps equivalent chapters across different boards to the same concept.
/// E.g., CBSE "Fractions" == Maharashtra "Parts of a Whole"
/// </summary>
public class ConceptMapping
{
    public int Id { get; set; }
    public string ConceptId { get; set; } = string.Empty;           // e.g. "math_fractions"
    public string ConceptName { get; set; } = string.Empty;         // e.g. "Fractions"
    public int BoardId { get; set; }
    public string BoardChapterName { get; set; } = string.Empty;    // chapter name in this board
    public int? ChapterId { get; set; }                             // FK to the actual chapter

    // Navigation
    public Board Board { get; set; } = null!;
    public Chapter? Chapter { get; set; }
}
