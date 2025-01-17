using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/user/{username}", async (HttpContext context, string username) =>
{
    string url = $"https://api.github.com/users/{username}/events";
    var client = new HttpClient();

    // GitHub API requires a User-Agent header
    client.DefaultRequestHeaders.Add("User-Agent", "C# App");

    try
    {
        // Fetch user events from GitHub
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return Results.Json(new { error = $"GitHub API error: {response.StatusCode}" }, statusCode: (int)response.StatusCode);
        }

        var responseData = await response.Content.ReadAsStringAsync();
        var events = JsonSerializer.Deserialize<List<JsonElement>>(responseData);

        // Process the events
        var activity = new List<string>();
        foreach (var evt in events)
        {
            string type = evt.GetProperty("type").GetString();
            string repo = evt.GetProperty("repo").GetProperty("name").GetString();

            if (type == "PushEvent")
            {
                var commits = evt.GetProperty("payload").GetProperty("commits").EnumerateArray().Count();
                activity.Add($"Pushed {commits} commit(s) to {repo}");
            }
            else if (type == "IssuesEvent")
            {
                string action = evt.GetProperty("payload").GetProperty("action").GetString();
                activity.Add($"{char.ToUpper(action[0]) + action[1..]} an issue in {repo}");
            }
            else if (type == "WatchEvent")
            {
                activity.Add($"Starred {repo}");
            }
            else
            {
                activity.Add($"Performed {type} on {repo}");
            }
        }

        return Results.Json(activity);
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
});

app.Run();
