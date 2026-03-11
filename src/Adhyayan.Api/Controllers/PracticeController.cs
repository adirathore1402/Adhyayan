using System.Security.Claims;
using Adhyayan.Core.DTOs;
using Adhyayan.Core.Enums;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adhyayan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PracticeController : ControllerBase
{
    private readonly IPracticeRepository _practiceRepo;
    private readonly IQuestionRepository _questionRepo;
    private readonly ICurriculumRepository _curriculumRepo;

    public PracticeController(
        IPracticeRepository practiceRepo,
        IQuestionRepository questionRepo,
        ICurriculumRepository curriculumRepo)
    {
        _practiceRepo = practiceRepo;
        _questionRepo = questionRepo;
        _curriculumRepo = curriculumRepo;
    }

    /// <summary>Start a new practice session</summary>
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request)
    {
        var mode = request.Mode == "daily_adventure"
            ? PracticeMode.DailyAdventure
            : PracticeMode.ChapterPractice;

        var session = new PracticeSession
        {
            ChildId = request.ChildId,
            ChapterId = request.ChapterId,
            Mode = mode
        };

        await _practiceRepo.CreateSessionAsync(session);

        // Fetch questions for the session
        IEnumerable<Question> questions;
        if (mode == PracticeMode.DailyAdventure)
        {
            // Daily adventure: mostly easy questions from random chapters
            questions = request.ChapterId.HasValue
                ? await _questionRepo.GetQuestionsByChapterAsync(request.ChapterId.Value, DifficultyLevel.Easy, 10)
                : Enumerable.Empty<Question>();
        }
        else
        {
            if (!request.ChapterId.HasValue)
                return BadRequest(new { message = "Chapter is required for chapter practice." });

            questions = await _questionRepo.GetQuestionsByChapterAsync(request.ChapterId.Value, count: 10);
        }

        return Ok(new
        {
            SessionId = session.Id,
            Mode = mode.ToString(),
            Questions = questions.Select(q => new
            {
                q.Id,
                q.QuestionText,
                q.QuestionType,
                Options = System.Text.Json.JsonSerializer.Deserialize<List<string>>(q.OptionsJson),
                Difficulty = q.Difficulty.ToString().ToLower()
            })
        });
    }

    /// <summary>Submit an answer</summary>
    [HttpPost("{sessionId}/answer")]
    public async Task<IActionResult> SubmitAnswer(int sessionId, [FromBody] SubmitAnswerRequest request)
    {
        var question = await _questionRepo.GetQuestionByIdAsync(request.QuestionId);
        if (question == null) return NotFound(new { message = "Question not found." });

        var isCorrect = string.Equals(request.SelectedAnswer, question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

        var answer = new PracticeAnswer
        {
            SessionId = sessionId,
            QuestionId = request.QuestionId,
            SelectedAnswer = request.SelectedAnswer,
            IsCorrect = isCorrect
        };

        await _practiceRepo.AddAnswerAsync(answer);

        return Ok(new
        {
            IsCorrect = isCorrect,
            CorrectAnswer = question.CorrectAnswer,
            Explanation = question.Explanation
        });
    }

    /// <summary>Complete a practice session</summary>
    [HttpPost("{sessionId}/complete")]
    public async Task<IActionResult> CompleteSession(int sessionId)
    {
        var session = await _practiceRepo.GetSessionByIdAsync(sessionId);
        if (session == null) return NotFound();

        await _practiceRepo.CompleteSessionAsync(sessionId);

        var total = session.Answers.Count;
        var correct = session.Answers.Count(a => a.IsCorrect);

        return Ok(new
        {
            SessionId = sessionId,
            TotalQuestions = total,
            CorrectAnswers = correct,
            AccuracyPercent = total > 0 ? Math.Round(correct * 100.0 / total, 1) : 0,
            ChapterName = session.Chapter?.Name
        });
    }
}
