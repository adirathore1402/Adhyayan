using System.Text.Json;
using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Adhyayan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Adhyayan.Infrastructure.Services;

public class CurriculumLoaderService : ICurriculumLoaderService
{
    private readonly AdhyayanDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CurriculumLoaderService> _logger;

    public CurriculumLoaderService(
        AdhyayanDbContext db,
        IConfiguration configuration,
        ILogger<CurriculumLoaderService> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedCurriculumFromConfigAsync()
    {
        if (await _db.Boards.AnyAsync())
        {
            _logger.LogInformation("Curriculum data already exists. Skipping seed.");
            return;
        }

        var curriculumPath = _configuration["CurriculumPath"] ?? "curriculum";

        // Seed boards
        var boards = await SeedBoardsAsync();

        // Seed subjects
        var subjects = await SeedSubjectsAsync();

        // Seed CBSE curriculum from JSON files
        var cbseBoard = boards.First(b => b.Code == "cbse");
        for (int classNum = 1; classNum <= 5; classNum++)
        {
            var filePath = Path.Combine(curriculumPath, $"cbse_class{classNum}.json");
            if (File.Exists(filePath))
            {
                await LoadCurriculumFileAsync(filePath, cbseBoard, classNum, subjects);
                _logger.LogInformation("Loaded CBSE Class {ClassNum} curriculum", classNum);
            }
            else
            {
                _logger.LogWarning("Curriculum file not found: {FilePath}", filePath);
            }
        }

        // Load state board mappings
        var mappingsPath = Path.Combine(curriculumPath, "state_board_mappings.json");
        if (File.Exists(mappingsPath))
        {
            await LoadStateBoardMappingsAsync(mappingsPath, boards);
            _logger.LogInformation("Loaded state board mappings");
        }

        _logger.LogInformation("Curriculum seeding completed successfully.");
    }

    private async Task<List<Board>> SeedBoardsAsync()
    {
        var boards = new List<Board>
        {
            new() { Code = "cbse", Name = "CBSE", Description = "Central Board of Secondary Education", IsPrimary = true },
            new() { Code = "maharashtra", Name = "Maharashtra State Board", Description = "Maharashtra State Board of Secondary and Higher Secondary Education" },
            new() { Code = "karnataka", Name = "Karnataka State Board", Description = "Karnataka Secondary Education Examination Board" },
            new() { Code = "tamilnadu", Name = "Tamil Nadu State Board", Description = "Tamil Nadu Board of Secondary Education" },
            new() { Code = "uttarpradesh", Name = "Uttar Pradesh State Board", Description = "Uttar Pradesh Madhyamik Shiksha Parishad" },
            new() { Code = "madhyapradesh", Name = "Madhya Pradesh State Board", Description = "Madhya Pradesh Board of Secondary Education" },
        };

        _db.Boards.AddRange(boards);
        await _db.SaveChangesAsync();
        return boards;
    }

    private async Task<List<Subject>> SeedSubjectsAsync()
    {
        var subjects = new List<Subject>
        {
            new() { Code = "math", Name = "Mathematics" },
            new() { Code = "english", Name = "English" },
            new() { Code = "evs", Name = "Environmental Studies" },
        };

        _db.Subjects.AddRange(subjects);
        await _db.SaveChangesAsync();
        return subjects;
    }

    private async Task LoadCurriculumFileAsync(string filePath, Board board, int classNum, List<Subject> subjects)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var curriculumData = JsonSerializer.Deserialize<CurriculumFileData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (curriculumData == null) return;

        // Create grade
        var grade = new Grade
        {
            BoardId = board.Id,
            GradeNumber = classNum,
            DisplayName = $"Class {classNum}"
        };
        _db.Grades.Add(grade);
        await _db.SaveChangesAsync();

        // Create chapters and topics for each subject
        foreach (var subjectData in curriculumData.Subjects)
        {
            var subject = subjects.FirstOrDefault(s => s.Code == subjectData.Code);
            if (subject == null) continue;

            int chapterNum = 1;
            foreach (var chapterData in subjectData.Chapters)
            {
                var chapter = new Chapter
                {
                    BoardId = board.Id,
                    GradeId = grade.Id,
                    SubjectId = subject.Id,
                    ChapterNumber = chapterNum++,
                    Name = chapterData.Name,
                    Description = chapterData.Description ?? ""
                };
                _db.Chapters.Add(chapter);
                await _db.SaveChangesAsync();

                if (chapterData.Topics != null)
                {
                    int topicOrder = 1;
                    foreach (var topicData in chapterData.Topics)
                    {
                        _db.Topics.Add(new Topic
                        {
                            ChapterId = chapter.Id,
                            Name = topicData.Name,
                            Description = topicData.Description ?? "",
                            SortOrder = topicOrder++
                        });
                    }
                    await _db.SaveChangesAsync();
                }
            }
        }
    }

    private async Task LoadStateBoardMappingsAsync(string filePath, List<Board> boards)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var mappingsData = JsonSerializer.Deserialize<StateBoardMappingsFile>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (mappingsData?.Mappings == null) return;

        foreach (var mapping in mappingsData.Mappings)
        {
            foreach (var boardMapping in mapping.Boards)
            {
                var board = boards.FirstOrDefault(b => b.Code == boardMapping.BoardCode);
                if (board == null) continue;

                // Try to find the chapter in the database
                var chapter = await _db.Chapters
                    .FirstOrDefaultAsync(c => c.Board.Code == boardMapping.BoardCode && c.Name == boardMapping.ChapterName);

                _db.ConceptMappings.Add(new ConceptMapping
                {
                    ConceptId = mapping.ConceptId,
                    ConceptName = mapping.ConceptName,
                    BoardId = board.Id,
                    BoardChapterName = boardMapping.ChapterName,
                    ChapterId = chapter?.Id
                });
            }
        }
        await _db.SaveChangesAsync();
    }

    // JSON deserialization models (internal)
    private class CurriculumFileData
    {
        public int Class { get; set; }
        public List<SubjectData> Subjects { get; set; } = new();
    }

    private class SubjectData
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<ChapterData> Chapters { get; set; } = new();
    }

    private class ChapterData
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<TopicData>? Topics { get; set; }
    }

    private class TopicData
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    private class StateBoardMappingsFile
    {
        public List<ConceptMappingData> Mappings { get; set; } = new();
    }

    private class ConceptMappingData
    {
        public string ConceptId { get; set; } = string.Empty;
        public string ConceptName { get; set; } = string.Empty;
        public List<BoardMappingData> Boards { get; set; } = new();
    }

    private class BoardMappingData
    {
        public string BoardCode { get; set; } = string.Empty;
        public string ChapterName { get; set; } = string.Empty;
    }
}
