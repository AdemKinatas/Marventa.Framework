using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Marventa.Framework.Infrastructure.Environment;

public static class EnvironmentHelper
{
    public static bool IsDevelopment(IWebHostEnvironment env) => env.IsDevelopment();

    public static bool IsStaging(IWebHostEnvironment env) => env.IsStaging();

    public static bool IsProduction(IWebHostEnvironment env) => env.IsProduction();

    public static bool IsEnvironment(IWebHostEnvironment env, string environmentName)
        => env.IsEnvironment(environmentName);

    public static bool IsInEnvironments(IWebHostEnvironment env, params string[] environments)
        => environments.Any(e => env.IsEnvironment(e));

    public static bool ShouldEnableFeature(string[] allowedEnvironments, string currentEnvironment)
    {
        if (allowedEnvironments == null || !allowedEnvironments.Any())
            return true;

        return allowedEnvironments.Any(e =>
            string.Equals(e, currentEnvironment, StringComparison.OrdinalIgnoreCase));
    }
}
