using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Adhyayan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Adhyayan.Infrastructure.Repositories;

public class CurriculumRepository : ICurriculumRepository
{
    private readonly AdhyayanDbContext _db;

    public CurriculumRepository(AdhyayanDbContext db) => _db = db;

    public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        => await _db.Boards.Where(b => b.IsActive).OrderByDescending(b => b.IsPrimary).ThenBy(b => b.Name).ToListAsync();

    public async Task<Board?> GetBoardByCodeAsync(string code)
        => await _db.Boards.FirstOrDefaultAsync(b => b.Code == code);

    public async Task<IEnumerable<Grade>> GetGradesByBoardAsync(int boardId)
        => await _db.Grades.Where(g => g.BoardId == boardId).OrderBy(g => g.GradeNumber).ToListAsync();

    public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        => await _db.Subjects.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();

    public async Task<IEnumerable<Chapter>> GetChaptersAsync(int boardId, int gradeId, int subjectId)
        => await _db.Chapters
            .Include(c => c.Topics)
            .Where(c => c.BoardId == boardId && c.GradeId == gradeId && c.SubjectId == subjectId)
            .OrderBy(c => c.ChapterNumber)
            .ToListAsync();

    public async Task<Chapter?> GetChapterByIdAsync(int chapterId)
        => await _db.Chapters
            .Include(c => c.Topics)
            .Include(c => c.Board)
            .Include(c => c.Grade)
            .Include(c => c.Subject)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

    public async Task<IEnumerable<Topic>> GetTopicsByChapterAsync(int chapterId)
        => await _db.Topics.Where(t => t.ChapterId == chapterId).OrderBy(t => t.SortOrder).ToListAsync();

    public async Task<IEnumerable<ConceptMapping>> GetConceptMappingsAsync(string conceptId)
        => await _db.ConceptMappings
            .Include(cm => cm.Board)
            .Where(cm => cm.ConceptId == conceptId)
            .ToListAsync();

    public async Task<Chapter?> GetEquivalentChapterAsync(int sourceChapterId, int targetBoardId)
    {
        // Find concept mapping for the source chapter
        var sourceMapping = await _db.ConceptMappings
            .FirstOrDefaultAsync(cm => cm.ChapterId == sourceChapterId);

        if (sourceMapping == null) return null;

        // Find the target board's mapping for the same concept
        var targetMapping = await _db.ConceptMappings
            .FirstOrDefaultAsync(cm => cm.ConceptId == sourceMapping.ConceptId && cm.BoardId == targetBoardId);

        if (targetMapping?.ChapterId == null) return null;

        return await GetChapterByIdAsync(targetMapping.ChapterId.Value);
    }
}
