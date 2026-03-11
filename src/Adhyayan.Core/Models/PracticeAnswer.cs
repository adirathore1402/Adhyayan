namespace Adhyayan.Core.Models;

public class PracticeAnswer
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public PracticeSession Session { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
