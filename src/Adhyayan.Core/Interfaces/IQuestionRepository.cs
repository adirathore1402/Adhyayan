using Adhyayan.Core.Enums;
using Adhyayan.Core.Models;

namespace Adhyayan.Core.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetQuestionsByChapterAsync(int chapterId, DifficultyLevel? difficulty = null, int? count = null);
    Task<IEnumerable<Question>> GetQuestionsByTopicAsync(int topicId, DifficultyLevel? difficulty = null);
    Task<Question?> GetQuestionByIdAsync(int questionId);
    Task<Question> AddQuestionAsync(Question question);
    Task AddQuestionsAsync(IEnumerable<Question> questions);
}
