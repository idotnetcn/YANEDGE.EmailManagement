using YANEDGE.EmailManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace YANEDGE.EmailManagement.Permissions;

public class EmailManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(EmailManagementPermissions.GroupName, L("EmailManagement"));

        var mailAccounts = group.AddPermission(EmailManagementPermissions.MailAccounts.Default, L("MailAccounts"));
        mailAccounts.AddChild(EmailManagementPermissions.MailAccounts.Create, L("Permission:Create"));
        mailAccounts.AddChild(EmailManagementPermissions.MailAccounts.Update, L("Permission:Update"));
        mailAccounts.AddChild(EmailManagementPermissions.MailAccounts.Delete, L("Permission:Delete"));
        mailAccounts.AddChild(EmailManagementPermissions.MailAccounts.ManagePermissions, L("Permission:ManagePermissions"));

        var mailThreads = group.AddPermission(EmailManagementPermissions.MailThreads.Default, L("MailThreads"));
        mailThreads.AddChild(EmailManagementPermissions.MailThreads.Assign, L("Permission:Assign"));
        mailThreads.AddChild(EmailManagementPermissions.MailThreads.Archive, L("Permission:Archive"));

        var mailMessages = group.AddPermission(EmailManagementPermissions.MailMessages.Default, L("MailMessages"));
        mailMessages.AddChild(EmailManagementPermissions.MailMessages.Create, L("Permission:Create"));
        mailMessages.AddChild(EmailManagementPermissions.MailMessages.Send, L("Permission:Send"));
        mailMessages.AddChild(EmailManagementPermissions.MailMessages.Delete, L("Permission:Delete"));
        mailMessages.AddChild(EmailManagementPermissions.MailMessages.ViewSensitive, L("Permission:ViewSensitive"));
        mailMessages.AddChild(EmailManagementPermissions.MailMessages.DownloadAttachment, L("Permission:DownloadAttachment"));

        var templates = group.AddPermission(EmailManagementPermissions.MailTemplates.Default, L("MailTemplates"));
        templates.AddChild(EmailManagementPermissions.MailTemplates.Create, L("Permission:Create"));
        templates.AddChild(EmailManagementPermissions.MailTemplates.Update, L("Permission:Update"));
        templates.AddChild(EmailManagementPermissions.MailTemplates.Delete, L("Permission:Delete"));

        var rules = group.AddPermission(EmailManagementPermissions.MailRules.Default, L("MailRules"));
        rules.AddChild(EmailManagementPermissions.MailRules.Create, L("Permission:Create"));
        rules.AddChild(EmailManagementPermissions.MailRules.Update, L("Permission:Update"));
        rules.AddChild(EmailManagementPermissions.MailRules.Delete, L("Permission:Delete"));

        var contacts = group.AddPermission(EmailManagementPermissions.Contacts.Default, L("Contacts"));
        contacts.AddChild(EmailManagementPermissions.Contacts.Create, L("Permission:Create"));
        contacts.AddChild(EmailManagementPermissions.Contacts.Update, L("Permission:Update"));
        contacts.AddChild(EmailManagementPermissions.Contacts.Delete, L("Permission:Delete"));

        var integrationApps = group.AddPermission(EmailManagementPermissions.IntegrationApps.Default, L("IntegrationApps"));
        integrationApps.AddChild(EmailManagementPermissions.IntegrationApps.Create, L("Permission:Create"));
        integrationApps.AddChild(EmailManagementPermissions.IntegrationApps.Update, L("Permission:Update"));
        integrationApps.AddChild(EmailManagementPermissions.IntegrationApps.Delete, L("Permission:Delete"));

        group.AddPermission(EmailManagementPermissions.AuditLogs.Default, L("AuditLogs"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create<EmailManagementResource>(name);
}
