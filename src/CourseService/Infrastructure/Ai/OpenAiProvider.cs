using System.ClientModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Courses.Application.Abstractions.Ai;
using OpenAI.Chat;

namespace Courses.Infrastructure.Ai;

public class OpenAiProvider<T> : IAiProvider<T>
{
    private readonly ChatClient _chatClient;
    private const int MaxTranscriptLength = 12000;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAiProvider(string apiKey)
    {
        _chatClient = new("gpt-4o-mini", apiKey);
    }

    public async Task<T> SendAsync(string prompt, CancellationToken cancellationToken = default)
    {
        string optimizedPrompt = BuildOptimizedPrompt(prompt);

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        List<ChatMessage> messages = [new UserChatMessage(optimizedPrompt)];

        ClientResult<ChatCompletion> completion = await _chatClient
            .CompleteChatAsync(messages, options, cancellationToken);

        string json = completion.Value.Content[0].Text;

        return JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
    }

    private static string BuildOptimizedPrompt(string originalPrompt)
    {
        var sb = new StringBuilder();

        sb.AppendLine("### CRITICAL: OUTPUT FORMAT RULES ###");
        sb.AppendLine("You MUST return only a valid JSON object.");
        sb.Append("The JSON object must contain exactly these fields: ");

        IEnumerable<string> propertyNames = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => $"\"{p.Name.ToLowerInvariant()}\"");

        sb.AppendLine(string.Join(", ", propertyNames) + ".");
        sb.AppendLine("Do not include any explanation, markdown tags, or extra text.");
        sb.AppendLine("### END OF RULES ###");
        sb.AppendLine();

        sb.AppendLine(originalPrompt);

        string finalPrompt = sb.ToString();

        if (finalPrompt.Length > MaxTranscriptLength)
        {
            return finalPrompt[..MaxTranscriptLength] + "... [Content truncated]";
        }

        return finalPrompt;
    }
}
