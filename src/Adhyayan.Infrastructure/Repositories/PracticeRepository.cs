using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Adhyayan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Adhyayan.Infrastructure.Repositories;

public class PracticeRepository : IPracticeRepository
{
    private readonly AdhyayanDbContext _db;

    public PracticeRepository(AdhyayanDbContext db) => _db = db;

    public async Task<PracticeSession> CreateSessionAsync(PracticeSession session)
    {
        _db.PracticeSessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }

    public async Task<PracticeSession?> GetSessionByIdAsync(int sessionId)
        => await _db.PracticeSessions
            .Include(ps => ps.Answers)
                .ThenInclude(a => a.Question)
            .Include(ps => ps.Chapter)
            .FirstOrDefaultAsync(ps => ps.Id == sessionId);

    public async Task<PracticeAnswer> AddAnswerAsync(PracticeAnswer answer)
    {
        _db.PracticeAnswers.Add(answer);
        await _db.SaveChangesAsync();
        return answer;
    }

    public async Task CompleteSessionAsync(int sessionId)
    {
        var session = await _db.PracticeSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PracticeSession>> GetSessionsByChildAsync(int childId)
        => await _db.PracticeSessions
            .Include(ps => ps.Answers)
            .Include(ps => ps.Chapter)
                .ThenInclude(c => c!.Subject)
            .Include(ps => ps.Chapter)
                .ThenInclude(c => c!.Grade)
            .Where(ps => ps.ChildId == childId)
            .OrderByDescending(ps => ps.StartedAt)
            .ToListAsync();
}
