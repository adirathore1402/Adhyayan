using Adhyayan.Core.Enums;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Adhyayan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Adhyayan.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AdhyayanDbContext _db;

    public QuestionRepository(AdhyayanDbContext db) => _db = db;

    public async Task<IEnumerable<Question>> GetQuestionsByChapterAsync(int chapterId, DifficultyLevel? difficulty = null, int? count = null)
    {
        var query = _db.Questions.Where(q => q.ChapterId == chapterId);

        if (difficulty.HasValue)
            query = query.Where(q => q.Difficulty == difficulty.Value);

        // Randomize order for variety
        query = query.OrderBy(q => EF.Functions.Random());

        if (count.HasValue)
            query = query.Take(count.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsByTopicAsync(int topicId, DifficultyLevel? difficulty = null)
    {
        var query = _db.Questions.Where(q => q.TopicId == topicId);

        if (difficulty.HasValue)
            query = query.Where(q => q.Difficulty == difficulty.Value);

        return await query.OrderBy(q => EF.Functions.Random()).ToListAsync();
    }

    public async Task<Question?> GetQuestionByIdAsync(int questionId)
        => await _db.Questions.FindAsync(questionId);

    public async Task<Question> AddQuestionAsync(Question question)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        return question;
    }

    public async Task AddQuestionsAsync(IEnumerable<Question> questions)
    {
        _db.Questions.AddRange(questions);
        await _db.SaveChangesAsync();
    }
}
