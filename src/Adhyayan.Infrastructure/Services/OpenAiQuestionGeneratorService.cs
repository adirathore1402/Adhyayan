using System.Text.Json;
using Adhyayan.Core.DTOs;
using Adhyayan.Core.Enums;
using Adhyayan.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Adhyayan.Infrastructure.Services;

public class OpenAiQuestionGeneratorService : IQuestionGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly string _deploymentName;
    private readonly ILogger<OpenAiQuestionGeneratorService> _logger;

    public OpenAiQuestionGeneratorService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenAiQuestionGeneratorService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AzureOpenAI:ApiKey"] ?? "";
        _endpoint = (configuration["AzureOpenAI:Endpoint"] ?? "").TrimEnd('/');
        _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4o-mini";
        _logger = logger;
    }

    public async Task<IEnumerable<GeneratedQuestionDto>> GenerateQuestionsAsync(
        string board,
        int grade,
        string subject,
        string chapter,
        string? topic,
        DifficultyLevel difficulty,
        int count = 5)
    {
        var ageRange = GetAgeRange(grade);
        var difficultyStr = difficulty.ToString().ToLower();

        var topicClause = string.IsNullOrEmpty(topic) ? "" : $" specifically on the topic \"{topic}\"";

        var jsonFormat = "[{\"questionText\": \"...\", \"questionType\": \"mcq\", \"options\": [\"A) ...\", \"B) ...\", \"C) ...\", \"D) ...\"], \"correctAnswer\": \"A\", \"explanation\": \"...\"}]";

        var prompt = $"You are an expert Indian school curriculum question creator.\n\n" +
            $"Generate {count} original {difficultyStr}-level {subject} questions aligned with the {board.ToUpper()} curriculum for Class {grade}, chapter \"{chapter}\"{topicClause}.\n\n" +
            $"IMPORTANT RULES:\n" +
            $"- Questions must be suitable for {ageRange} year old children.\n" +
            $"- Do NOT copy directly from any NCERT textbook. Create original questions.\n" +
            $"- Each question must be a multiple-choice question with exactly 4 options.\n" +
            $"- One option must be clearly correct.\n" +
            $"- Include a brief child-friendly explanation for the correct answer.\n" +
            $"- Questions should be progressively interesting and engaging.\n\n" +
            $"Respond ONLY with a JSON array in this exact format (no markdown, no extra text):\n{jsonFormat}";

        try
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = "You are an expert educational content creator for Indian school children. You create original, curriculum-aligned questions." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 3000
            };

            var apiVersion = "2024-08-01-preview";
            var url = $"{_endpoint}/openai/deployments/{_deploymentName}/chat/completions?api-version={apiVersion}";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("api-key", _apiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Azure OpenAI returned {StatusCode}: {Body}", response.StatusCode, responseBody);
                return Enumerable.Empty<GeneratedQuestionDto>();
            }

            using var doc = JsonDocument.Parse(responseBody);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrEmpty(content))
                return Enumerable.Empty<GeneratedQuestionDto>();

            // Clean up potential markdown wrapping
            content = content.Trim();
            if (content.StartsWith("```json"))
                content = content[7..];
            if (content.StartsWith("```"))
                content = content[3..];
            if (content.EndsWith("```"))
                content = content[..^3];
            content = content.Trim();

            var questions = JsonSerializer.Deserialize<List<GeneratedQuestionDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return questions ?? Enumerable.Empty<GeneratedQuestionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate questions via Azure OpenAI for {Board} Grade {Grade} {Subject} - {Chapter}",
                board, grade, subject, chapter);
            return Enumerable.Empty<GeneratedQuestionDto>();
        }
    }

    private static string GetAgeRange(int grade) => grade switch
    {
        1 => "5-7",
        2 => "6-8",
        3 => "7-9",
        4 => "8-10",
        5 => "9-11",
        _ => "6-12"
    };
}
