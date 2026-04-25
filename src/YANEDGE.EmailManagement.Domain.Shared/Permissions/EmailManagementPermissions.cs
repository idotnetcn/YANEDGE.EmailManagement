namespace YANEDGE.EmailManagement.Permissions;

public static class EmailManagementPermissions
{
    public const string GroupName = "EmailManagement";

    public static class MailAccounts
    {
        public const string Default = GroupName + ".MailAccounts";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }

    public static class MailThreads
    {
        public const string Default = GroupName + ".MailThreads";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Assign = Default + ".Assign";
        public const string Archive = Default + ".Archive";
    }

    public static class MailMessages
    {
        public const string Default = GroupName + ".MailMessages";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Send = Default + ".Send";
        public const string ViewSensitive = Default + ".ViewSensitive";
        public const string DownloadAttachment = Default + ".DownloadAttachment";
    }

    public static class MailTemplates
    {
        public const string Default = GroupName + ".MailTemplates";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class MailRules
    {
        public const string Default = GroupName + ".MailRules";
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

    public static class MailApprovals
    {
        public const string Default = GroupName + ".MailApprovals";
        public const string Process = Default + ".Process";
    }

    public static class IntegrationApps
    {
        public const string Default = GroupName + ".IntegrationApps";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
    }
}
