using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZaryaSite;
using static System.Net.Mime.MediaTypeNames;

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
    public NewsInput Input { get; set; } = new();

    public string? Message { get; private set; }

    public void OnGet()
    {
    }

    private bool IsCreateErrorMassage()
    {
        return string.IsNullOrWhiteSpace(Input.Title) 
            || string.IsNullOrWhiteSpace(Input.Description)
            || string.IsNullOrWhiteSpace(Input.Content)
            || Input.Image == null;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if(IsCreateErrorMassage())
        {
            Message = "Заполни заголовок, краткое описание, изображение и полный текст.";
            return Page();
        }

        string pathSavedPicture;
        try
        {
            pathSavedPicture = await SavePicture();
        }
        catch (InvalidOperationException exception)
        {
            Message = exception.Message;
            return Page();
        }

        var newsPath = Path.Combine(_environment.WebRootPath, "Additions", "news.json");
        var item = new NewsItem
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy"),
            Title = Input.Title.Trim(),
            Description = Input.Description.Trim(),
            Image = pathSavedPicture,
            Content = Input.Content
        };
        await JsonHelper<NewsItem>.AddToJsonMassive(newsPath, item);

        Message = $"Новость добавлена: {Input.Title}";
        return Redirect("/news.html");
    }

    private async Task<string> SavePicture()
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "Additions", "Media", "News");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = Path.GetFileName(Input.Image!.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);
        ThrowIfNewsFileExists(filePath);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await Input.Image.CopyToAsync(stream);
        }

        return $"/Additions/Media/News/{fileName}";
    }

    private static void ThrowIfNewsFileExists(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            throw new InvalidOperationException("Файл с таким именем уже есть в папке News.");
        }
    }

    public class NewsInput
    {
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public IFormFile? Image { get; set; }

        public string Content { get; set; } = "";
    }

    public class NewsItem
    {
        [JsonPropertyName("date")]
        public required string Date { get; set; } = "";

        [JsonPropertyName("title")]
        public required string Title { get; set; } = "";

        [JsonPropertyName("description")]
        public required string Description { get; set; } = "";

        [JsonPropertyName("image")]
        public required string Image { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }
    }
}
