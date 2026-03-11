namespace Adhyayan.Core.DTOs;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ParentId { get; set; }
}

public class AddChildRequest
{
    public string Name { get; set; } = string.Empty;
    public int GradeNumber { get; set; }
    public int BoardId { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public class StartSessionRequest
{
    public int ChildId { get; set; }
    public int? ChapterId { get; set; }
    public string Mode { get; set; } = "chapter_practice"; // "daily_adventure" or "chapter_practice"
}

public class SubmitAnswerRequest
{
    public int QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = string.Empty;
}

public class ChapterProgressDto
{
    public int ChapterId { get; set; }
    public string ChapterName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int GradeNumber { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyPercent { get; set; }
}

public class ChildProgressDto
{
    public int ChildId { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public int GradeNumber { get; set; }
    public string BoardName { get; set; } = string.Empty;
    public List<ChapterProgressDto> ChapterProgress { get; set; } = new();
}
