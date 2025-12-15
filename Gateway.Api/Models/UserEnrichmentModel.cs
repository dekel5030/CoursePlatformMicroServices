namespace Gateway.Api.Models;

public class UserEnrichmentModel
{
    public string UserId { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}