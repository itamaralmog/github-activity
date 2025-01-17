using System;
using System.Net.Http;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: GitHubActivityCLI <username>");
            return;
        }

        var username = args[0];
        var apiUrl = $"http://localhost:5030/user/{username}";

        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: Unable to fetch activity for user {username}. Status Code: {response.StatusCode}");
                return;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var activities = JsonSerializer.Deserialize<List<string>>(responseBody);

            if (activities == null || activities.Count == 0)
            {
                Console.WriteLine($"No recent activity found for user '{username}'.");
            }
            else
            {
                Console.WriteLine($"GitHub Activity for user '{username}':");
                activities.ForEach(activity => Console.WriteLine($"- {activity}"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
