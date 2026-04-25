using Volo.Abp.Reflection;

namespace YANEDGE.EmailManagement.Permissions;

public static class EmailManagementPermissions
{
    public const string GroupName = "EmailManagement";

    public static class Emails
    {
        public const string Default = GroupName + ".Emails";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Send = Default + ".Send";
    }

    public static class Templates
    {
        public const string Default = GroupName + ".Templates";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(EmailManagementPermissions));
    }
}
