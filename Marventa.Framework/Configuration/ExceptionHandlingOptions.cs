namespace Marventa.Framework.Configuration;

public class ExceptionHandlingOptions
{
    public const string SectionName = "ExceptionHandling";

    public bool IncludeStackTrace { get; set; } = false;
    public bool IncludeExceptionDetails { get; set; } = false;
    public bool LogExceptions { get; set; } = true;
    public string DefaultErrorMessage { get; set; } = "An error occurred processing your request.";
    public Dictionary<string, int> CustomStatusCodes { get; set; } = new();
    public List<string> SensitiveDataKeys { get; set; } = new() { "password", "token", "secret", "key" };
}
