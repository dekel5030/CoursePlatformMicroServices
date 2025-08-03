using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using Common;
using Common.Errors;
using Common.Serialization;
using Common.Web.Extensions;

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

        var currentCulture = CultureInfo.CurrentUICulture.Name;
        _client.DefaultRequestHeaders.AcceptLanguage.Clear();
        _client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(currentCulture);

        var response = await _client.PostAsync(_config["UserService"], httpContent);

        var rawString = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Raw Response: " + rawString);

        var result = await response.ToResultAsync<UserReadDto>();

        // Testing
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        Console.WriteLine("Result Response: " + json);
        // End of testing

        return result;
    }

    public Task<Result<UserReadDto>> DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
    }
}