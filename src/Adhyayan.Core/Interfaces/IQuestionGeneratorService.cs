using Adhyayan.Core.DTOs;
using Adhyayan.Core.Enums;

namespace Adhyayan.Core.Interfaces;

public interface IQuestionGeneratorService
{
    Task<IEnumerable<GeneratedQuestionDto>> GenerateQuestionsAsync(
        string board,
        int grade,
        string subject,
        string chapter,
        string? topic,
        DifficultyLevel difficulty,
        int count = 5);
}
