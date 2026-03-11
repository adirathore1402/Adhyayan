namespace Adhyayan.Core.Models;

public class Topic
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Name { get; set; } = string.Empty;       // e.g. "Basic Fractions"
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    // Navigation
    public Chapter Chapter { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
