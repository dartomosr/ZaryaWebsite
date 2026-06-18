using System.Text.Json;

namespace ZaryaSite;

public static class JsonHelper<T>
{
    public static async Task AddToJsonMassive(string path, T element)
    {
        var items = await ReadMassiveAsync(path);
        items.Add(element);
        await WriteMassiveAsync(path, items);
    }

    private static async Task<List<T>> ReadMassiveAsync(string path)
    {
        if (!File.Exists(path))
        {
            throw new Exception("Путь неверный");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json, options) ?? [];
    }

    private static async Task WriteMassiveAsync(string path, List<T> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        await File.WriteAllTextAsync(path, json);
    }
}
