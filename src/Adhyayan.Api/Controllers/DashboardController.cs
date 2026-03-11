using System.Security.Claims;
using Adhyayan.Core.DTOs;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adhyayan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IPracticeRepository _practiceRepo;

    public DashboardController(IUserRepository userRepo, IPracticeRepository practiceRepo)
    {
        _userRepo = userRepo;
        _practiceRepo = practiceRepo;
    }

    /// <summary>Get children for the logged-in parent</summary>
    [HttpGet("children")]
    public async Task<IActionResult> GetChildren()
    {
        var parentId = GetParentId();
        var children = await _userRepo.GetChildrenByParentAsync(parentId);
        return Ok(children.Select(c => new
        {
            c.Id,
            c.Name,
            c.GradeNumber,
            Board = new { c.Board.Id, c.Board.Code, c.Board.Name },
            c.DateOfBirth,
            c.AvatarUrl
        }));
    }

    /// <summary>Add a child profile</summary>
    [HttpPost("children")]
    public async Task<IActionResult> AddChild([FromBody] AddChildRequest request)
    {
        var parentId = GetParentId();

        var child = new Child
        {
            ParentId = parentId,
            Name = request.Name,
            GradeNumber = request.GradeNumber,
            BoardId = request.BoardId,
            DateOfBirth = request.DateOfBirth
        };

        await _userRepo.CreateChildAsync(child);
        return Ok(new { child.Id, child.Name, child.GradeNumber });
    }

    /// <summary>Get progress for a specific child</summary>
    [HttpGet("children/{childId}/progress")]
    public async Task<IActionResult> GetChildProgress(int childId)
    {
        var child = await _userRepo.GetChildByIdAsync(childId);
        if (child == null) return NotFound();

        var sessions = await _practiceRepo.GetSessionsByChildAsync(childId);

        // Group by chapter and calculate accuracy
        var chapterProgress = sessions
            .Where(s => s.Chapter != null)
            .GroupBy(s => s.ChapterId)
            .Select(g =>
            {
                var allAnswers = g.SelectMany(s => s.Answers).ToList();
                var chapter = g.First().Chapter!;
                return new ChapterProgressDto
                {
                    ChapterId = chapter.Id,
                    ChapterName = chapter.Name,
                    SubjectName = chapter.Subject?.Name ?? "",
                    GradeNumber = chapter.Grade?.GradeNumber ?? child.GradeNumber,
                    TotalQuestions = allAnswers.Count,
                    CorrectAnswers = allAnswers.Count(a => a.IsCorrect),
                    AccuracyPercent = allAnswers.Count > 0
                        ? Math.Round(allAnswers.Count(a => a.IsCorrect) * 100.0 / allAnswers.Count, 1)
                        : 0
                };
            })
            .ToList();

        return Ok(new ChildProgressDto
        {
            ChildId = child.Id,
            ChildName = child.Name,
            GradeNumber = child.GradeNumber,
            BoardName = child.Board?.Name ?? "",
            ChapterProgress = chapterProgress
        });
    }

    private int GetParentId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(claim?.Value ?? "0");
    }
}
