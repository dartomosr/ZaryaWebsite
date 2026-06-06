using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages.Admin;

[Authorize(Roles = "Admin")]
public class AddNewsModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public AddNewsModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [BindProperty]
    public NewsInput Input { get; set; } = new()
    {
        Date = DateTime.Now.ToString("dd.MM.yyyy")
    };

    public string? Message { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Title) ||
            string.IsNullOrWhiteSpace(Input.Description) ||
            string.IsNullOrWhiteSpace(Input.Content))
        {
            Message = "Заполни заголовок, краткое описание и полный текст.";
            return Page();
        }

        var newsPath = Path.Combine(_environment.WebRootPath, "Additions", "news.json");
        var newsItems = await ReadNewsItemsAsync(newsPath);

        var id = string.IsNullOrWhiteSpace(Input.Id)
            ? CreateId(Input.Title)
            : CreateId(Input.Id);

        if (newsItems.Any(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase)))
        {
            id = $"{id}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        }

        newsItems.Add(new NewsItem
        {
            Id = id,
            Date = string.IsNullOrWhiteSpace(Input.Date) ? DateTime.Now.ToString("dd.MM.yyyy") : Input.Date.Trim(),
            Title = Input.Title.Trim(),
            Description = Input.Description.Trim(),
            Image = Input.Image?.Trim() ?? "",
            Content = Input.Content
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToList()
        });

        var json = JsonSerializer.Serialize(newsItems, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        await System.IO.File.WriteAllTextAsync(newsPath, json);

        Message = $"Новость добавлена: {Input.Title}";
        Input = new NewsInput
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy")
        };

        return Redirect("/news.html");
    }

    private static async Task<List<NewsItem>> ReadNewsItemsAsync(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return [];
        }

        var json = await System.IO.File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<NewsItem>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }

    private static string CreateId(string text)
    {
        var chars = text
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray();

        var id = string.Join('-', new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(id) ? $"news-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}" : id;
    }

    public class NewsInput
    {
        public string? Id { get; set; }
        public string? Date { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string? Image { get; set; }
        public string Content { get; set; } = "";
    }

    public class NewsItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("date")]
        public string Date { get; set; } = "";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("image")]
        public string Image { get; set; } = "";

        [JsonPropertyName("content")]
        public List<string> Content { get; set; } = [];
    }
}
