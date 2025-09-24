using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Marventa.Framework.Core.Utilities;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public static string Serialize<T>(T obj, bool prettyPrint = false)
    {
        var options = prettyPrint ? PrettyOptions : DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    public static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public static object? Deserialize(string json, Type type)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize(json, type, DefaultOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static bool TryDeserialize<T>(string json, out T? result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static JsonDocument ParseDocument(string json)
    {
        return JsonDocument.Parse(json);
    }

    public static T? GetPropertyValue<T>(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            try
            {
                return JsonSerializer.Deserialize<T>(property.GetRawText(), DefaultOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        return default;
    }
}