using System.Net;
using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using Common;
using Common.Errors;

namespace AuthService.SyncDataServices.Http;

public class HttpUserServiceDataClient : IUserServiceDataClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public HttpUserServiceDataClient(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }
    public async Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto)
    {
        Console.WriteLine("--> Sending User Create Request to UserService...");
        var httpContent = new StringContent(
                JsonSerializer.Serialize(userCreateDto),
                Encoding.UTF8,
                "application/json");

        var response = await _client.PostAsync(_config["UserService"], httpContent);

        var result = await response.Content.ReadFromJsonAsync<Result<UserReadDto>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result is null)
        {
            Console.WriteLine("-->❌ Could not parse response body as Result<UserReadDto>");
            return Result<UserReadDto>.Failure(Error.Unexpected);
        }

        return result;
    }
}