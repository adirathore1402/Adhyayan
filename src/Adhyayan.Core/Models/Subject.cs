namespace Adhyayan.Core.Models;

public class Subject
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;      // e.g. "math", "english", "evs"
    public string Name { get; set; } = string.Empty;       // e.g. "Mathematics", "English", "Environmental Studies"
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
