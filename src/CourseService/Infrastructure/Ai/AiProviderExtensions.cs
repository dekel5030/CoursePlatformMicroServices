using Courses.Application.Abstractions.Ai;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using OpenAI.Responses;

namespace Courses.Infrastructure.Ai;

internal static class AiProviderExtensions
{
    public static IServiceCollection AddAiProvider(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
            string apiKey = config["OpenAi:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key is missing");
            //return new ChatClient("gpt-5.2-chat-latest", apiKey);

            return new ChatClient("gpt-5-mini", apiKey);
            //return new ChatClient("gpt-4o-mini", apiKey);
        });

        services.AddTransient(typeof(IAiProvider<>), typeof(OpenAiProvider<>));

        return services;
    }
}
