using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Marventa.Framework.Core.Utilities;

public static class FileHelper
{
    public static async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllBytesAsync(path, cancellationToken);
    }

    public static async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        await File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    public static async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(path, cancellationToken);
    }

    public static async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public static void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public static string GetMimeType(string fileName)
    {
        var extension = GetFileExtension(fileName);

        return extension switch
        {
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".doc" => "application/vnd.ms-word",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    public static long GetFileSizeInBytes(string path)
    {
        return new FileInfo(path).Length;
    }

    public static string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:n1} {suffixes[counter]}";
    }

    public static bool IsValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var invalidChars = Path.GetInvalidFileNameChars();
        return fileName.IndexOfAny(invalidChars) == -1;
    }

    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "file";

        var invalidChars = Path.GetInvalidFileNameChars();

        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName;
    }
}