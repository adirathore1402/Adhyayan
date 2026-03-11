namespace Adhyayan.Core.DTOs;

public class GeneratedQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "mcq";
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}
