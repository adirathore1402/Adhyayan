using System.Text.Json;
using Adhyayan.Core.Enums;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adhyayan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionRepository _questionRepo;
    private readonly IQuestionGeneratorService _questionGenerator;
    private readonly ICurriculumRepository _curriculumRepo;

    public QuestionsController(
        IQuestionRepository questionRepo,
        IQuestionGeneratorService questionGenerator,
        ICurriculumRepository curriculumRepo)
    {
        _questionRepo = questionRepo;
        _questionGenerator = questionGenerator;
        _curriculumRepo = curriculumRepo;
    }

    /// <summary>Get questions for a chapter (from DB)</summary>
    [HttpGet("chapter/{chapterId}")]
    public async Task<IActionResult> GetByChapter(
        int chapterId,
        [FromQuery] string? difficulty = null,
        [FromQuery] int count = 10)
    {
        DifficultyLevel? level = difficulty != null
            ? Enum.Parse<DifficultyLevel>(difficulty, true)
            : null;

        var questions = await _questionRepo.GetQuestionsByChapterAsync(chapterId, level, count);
        return Ok(questions.Select(MapQuestion));
    }

    /// <summary>Generate new AI questions for a chapter and save to DB</summary>
    [HttpPost("generate")]
    [Authorize]
    public async Task<IActionResult> GenerateQuestions(
        [FromQuery] int chapterId,
        [FromQuery] string difficulty = "easy",
        [FromQuery] int count = 5,
        [FromQuery] string? topic = null)
    {
        var chapter = await _curriculumRepo.GetChapterByIdAsync(chapterId);
        if (chapter == null) return NotFound(new { message = "Chapter not found." });

        var level = Enum.Parse<DifficultyLevel>(difficulty, true);

        var generated = await _questionGenerator.GenerateQuestionsAsync(
            chapter.Board.Name,
            chapter.Grade.GradeNumber,
            chapter.Subject.Name,
            chapter.Name,
            topic,
            level,
            count);

        var generatedList = generated.ToList();
        if (generatedList.Count == 0)
            return StatusCode(503, new { message = "Failed to generate questions. Check OpenAI configuration." });

        // Save to database
        var questions = generatedList.Select(g => new Question
        {
            ChapterId = chapterId,
            Difficulty = level,
            QuestionText = g.QuestionText,
            QuestionType = g.QuestionType,
            OptionsJson = JsonSerializer.Serialize(g.Options),
            CorrectAnswer = g.CorrectAnswer,
            Explanation = g.Explanation,
            IsAiGenerated = true
        }).ToList();

        await _questionRepo.AddQuestionsAsync(questions);

        return Ok(questions.Select(MapQuestion));
    }

    private static object MapQuestion(Question q) => new
    {
        q.Id,
        q.ChapterId,
        q.TopicId,
        Difficulty = q.Difficulty.ToString().ToLower(),
        q.QuestionText,
        q.QuestionType,
        Options = JsonSerializer.Deserialize<List<string>>(q.OptionsJson),
        q.CorrectAnswer,
        q.Explanation,
        q.IsAiGenerated
    };
}
