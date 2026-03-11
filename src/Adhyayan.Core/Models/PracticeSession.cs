using Adhyayan.Core.Enums;

namespace Adhyayan.Core.Models;

public class PracticeSession
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public int? ChapterId { get; set; }
    public PracticeMode Mode { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Child Child { get; set; } = null!;
    public Chapter? Chapter { get; set; }
    public ICollection<PracticeAnswer> Answers { get; set; } = new List<PracticeAnswer>();
}
