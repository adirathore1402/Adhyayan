using Adhyayan.Core.Models;

namespace Adhyayan.Core.Interfaces;

public interface IPracticeRepository
{
    Task<PracticeSession> CreateSessionAsync(PracticeSession session);
    Task<PracticeSession?> GetSessionByIdAsync(int sessionId);
    Task<PracticeAnswer> AddAnswerAsync(PracticeAnswer answer);
    Task CompleteSessionAsync(int sessionId);
    Task<IEnumerable<PracticeSession>> GetSessionsByChildAsync(int childId);
}
