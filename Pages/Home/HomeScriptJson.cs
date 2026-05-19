using System.Text.Json;

namespace rosa_testovoye.Pages.Home;

internal static class HomeScriptJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);
}
