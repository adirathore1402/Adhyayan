using Adhyayan.Core.Enums;

namespace Adhyayan.Core.Models;

public class Question
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public int? TopicId { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "mcq";      // mcq, fill_blank, true_false
    public string OptionsJson { get; set; } = "[]";         // JSON array of options
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public bool IsAiGenerated { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Chapter Chapter { get; set; } = null!;
    public Topic? Topic { get; set; }
}
