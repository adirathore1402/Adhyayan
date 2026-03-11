using Adhyayan.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Adhyayan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurriculumController : ControllerBase
{
    private readonly ICurriculumRepository _curriculumRepo;

    public CurriculumController(ICurriculumRepository curriculumRepo)
    {
        _curriculumRepo = curriculumRepo;
    }

    /// <summary>Get all active boards</summary>
    [HttpGet("boards")]
    public async Task<IActionResult> GetBoards()
    {
        var boards = await _curriculumRepo.GetAllBoardsAsync();
        return Ok(boards.Select(b => new
        {
            b.Id,
            b.Code,
            b.Name,
            b.Description,
            b.IsPrimary
        }));
    }

    /// <summary>Get grades for a board</summary>
    [HttpGet("boards/{boardId}/grades")]
    public async Task<IActionResult> GetGrades(int boardId)
    {
        var grades = await _curriculumRepo.GetGradesByBoardAsync(boardId);
        return Ok(grades.Select(g => new
        {
            g.Id,
            g.GradeNumber,
            g.DisplayName
        }));
    }

    /// <summary>Get all subjects</summary>
    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects()
    {
        var subjects = await _curriculumRepo.GetAllSubjectsAsync();
        return Ok(subjects.Select(s => new
        {
            s.Id,
            s.Code,
            s.Name
        }));
    }

    /// <summary>Get chapters for a specific board, grade, and subject</summary>
    [HttpGet("chapters")]
    public async Task<IActionResult> GetChapters(
        [FromQuery] int boardId,
        [FromQuery] int gradeId,
        [FromQuery] int subjectId)
    {
        var chapters = await _curriculumRepo.GetChaptersAsync(boardId, gradeId, subjectId);
        return Ok(chapters.Select(c => new
        {
            c.Id,
            c.ChapterNumber,
            c.Name,
            c.Description,
            Topics = c.Topics.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.SortOrder
            })
        }));
    }

    /// <summary>Get a specific chapter with full details</summary>
    [HttpGet("chapters/{chapterId}")]
    public async Task<IActionResult> GetChapter(int chapterId)
    {
        var chapter = await _curriculumRepo.GetChapterByIdAsync(chapterId);
        if (chapter == null) return NotFound();

        return Ok(new
        {
            chapter.Id,
            chapter.ChapterNumber,
            chapter.Name,
            chapter.Description,
            Board = new { chapter.Board.Id, chapter.Board.Code, chapter.Board.Name },
            Grade = new { chapter.Grade.Id, chapter.Grade.GradeNumber, chapter.Grade.DisplayName },
            Subject = new { chapter.Subject.Id, chapter.Subject.Code, chapter.Subject.Name },
            Topics = chapter.Topics.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.SortOrder
            })
        });
    }

    /// <summary>Get topics for a chapter</summary>
    [HttpGet("chapters/{chapterId}/topics")]
    public async Task<IActionResult> GetTopics(int chapterId)
    {
        var topics = await _curriculumRepo.GetTopicsByChapterAsync(chapterId);
        return Ok(topics.Select(t => new
        {
            t.Id,
            t.Name,
            t.Description,
            t.SortOrder
        }));
    }

    /// <summary>Get concept mappings across boards</summary>
    [HttpGet("concept-mappings/{conceptId}")]
    public async Task<IActionResult> GetConceptMappings(string conceptId)
    {
        var mappings = await _curriculumRepo.GetConceptMappingsAsync(conceptId);
        return Ok(mappings.Select(m => new
        {
            m.Id,
            m.ConceptId,
            m.ConceptName,
            Board = new { m.Board.Id, m.Board.Code, m.Board.Name },
            m.BoardChapterName,
            m.ChapterId
        }));
    }

    /// <summary>Find equivalent chapter in another board</summary>
    [HttpGet("chapters/{chapterId}/equivalent")]
    public async Task<IActionResult> GetEquivalentChapter(int chapterId, [FromQuery] int targetBoardId)
    {
        var chapter = await _curriculumRepo.GetEquivalentChapterAsync(chapterId, targetBoardId);
        if (chapter == null) return NotFound(new { message = "No equivalent chapter found." });

        return Ok(new
        {
            chapter.Id,
            chapter.ChapterNumber,
            chapter.Name,
            chapter.Description
        });
    }
}
