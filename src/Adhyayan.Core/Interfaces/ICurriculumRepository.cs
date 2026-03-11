using Adhyayan.Core.Models;

namespace Adhyayan.Core.Interfaces;

public interface ICurriculumRepository
{
    // Boards
    Task<IEnumerable<Board>> GetAllBoardsAsync();
    Task<Board?> GetBoardByCodeAsync(string code);

    // Grades
    Task<IEnumerable<Grade>> GetGradesByBoardAsync(int boardId);

    // Subjects
    Task<IEnumerable<Subject>> GetAllSubjectsAsync();

    // Chapters
    Task<IEnumerable<Chapter>> GetChaptersAsync(int boardId, int gradeId, int subjectId);
    Task<Chapter?> GetChapterByIdAsync(int chapterId);

    // Topics
    Task<IEnumerable<Topic>> GetTopicsByChapterAsync(int chapterId);

    // Concept mappings
    Task<IEnumerable<ConceptMapping>> GetConceptMappingsAsync(string conceptId);
    Task<Chapter?> GetEquivalentChapterAsync(int sourceChapterId, int targetBoardId);
}
