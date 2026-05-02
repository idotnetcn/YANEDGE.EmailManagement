namespace YANEDGE.EmailManagement.Permissions;

/// <summary>
/// 邮件管理系统权限定义
/// </summary>
public static class EmailManagementPermissions
{
    public const string GroupName = "MailManagement";

    public static class MailAccounts
    {
        public const string Default = GroupName + ".MailAccounts";
        public const string Manage = Default + ".Manage";
        public const string Sync = Default + ".Sync";
    }

    public static class Messages
    {
        public const string Default = GroupName + ".Messages";
        public const string View = Default + ".View";
    }

    public static class Threads
    {
        public const string Default = GroupName + ".Threads";
        public const string View = Default + ".View";
        public const string Assign = Default + ".Assign";
        public const string Claim = Default + ".Claim";
        public const string Archive = Default + ".Archive";
    }

    public static class SendTasks
    {
        public const string Default = GroupName + ".SendTasks";
        public const string Create = Default + ".Create";
        public const string Send = Default + ".Send";
        public const string Approve = Default + ".Approve";
    }

    public static class Attachments
    {
        public const string Default = GroupName + ".Attachments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Download = Default + ".Download";
    }

    public static class Rules
    {
        public const string Default = GroupName + ".Rules";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class Templates
    {
        public const string Default = GroupName + ".Templates";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class Labels
    {
        public const string Default = GroupName + ".Labels";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Contacts
    {
        public const string Default = GroupName + ".Contacts";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class BusinessRelations
    {
        public const string Default = GroupName + ".BusinessRelations";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Integration
    {
        public const string Default = GroupName + ".Integration";
        public const string Manage = Default + ".Manage";
    }

    public static class Statistics
    {
        public const string Default = GroupName + ".Statistics";
        public const string View = Default + ".View";
    }
}
