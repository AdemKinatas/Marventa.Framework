namespace Marventa.Framework.Core.Interfaces.MultiTenancy;

public static class TenantPermissions
{
    public const string Read = "tenant:read";
    public const string Write = "tenant:write";
    public const string Delete = "tenant:delete";
    public const string Admin = "tenant:admin";
    public const string ManageUsers = "tenant:users:manage";
    public const string ManageSettings = "tenant:settings:manage";
    public const string ViewReports = "tenant:reports:view";
    public const string ManageBilling = "tenant:billing:manage";
}