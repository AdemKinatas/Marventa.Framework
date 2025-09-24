namespace Marventa.Framework.Core.Constants;

public static class CacheKeys
{
    public const string USER_PREFIX = "user:";
    public const string PERMISSIONS_PREFIX = "permissions:";
    public const string SETTINGS_PREFIX = "settings:";
    public const string LOOKUP_PREFIX = "lookup:";

    public static string GetUserKey(string userId) => $"{USER_PREFIX}{userId}";
    public static string GetPermissionsKey(string userId) => $"{PERMISSIONS_PREFIX}{userId}";
    public static string GetSettingsKey(string key) => $"{SETTINGS_PREFIX}{key}";
    public static string GetLookupKey(string category) => $"{LOOKUP_PREFIX}{category}";
}