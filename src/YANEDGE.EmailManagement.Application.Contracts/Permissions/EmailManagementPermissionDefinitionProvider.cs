using YANEDGE.EmailManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace YANEDGE.EmailManagement.Permissions;

public class EmailManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var emailManagementGroup = context.AddGroup(
            EmailManagementPermissions.GroupName,
            L("Permission:EmailManagement"));

        var emailsPermission = emailManagementGroup.AddPermission(
            EmailManagementPermissions.Emails.Default,
            L("Permission:Emails"));
        emailsPermission.AddChild(EmailManagementPermissions.Emails.Create, L("Permission:Emails.Create"));
        emailsPermission.AddChild(EmailManagementPermissions.Emails.Edit, L("Permission:Emails.Edit"));
        emailsPermission.AddChild(EmailManagementPermissions.Emails.Delete, L("Permission:Emails.Delete"));
        emailsPermission.AddChild(EmailManagementPermissions.Emails.Send, L("Permission:Emails.Send"));

        var templatesPermission = emailManagementGroup.AddPermission(
            EmailManagementPermissions.Templates.Default,
            L("Permission:Templates"));
        templatesPermission.AddChild(EmailManagementPermissions.Templates.Create, L("Permission:Templates.Create"));
        templatesPermission.AddChild(EmailManagementPermissions.Templates.Edit, L("Permission:Templates.Edit"));
        templatesPermission.AddChild(EmailManagementPermissions.Templates.Delete, L("Permission:Templates.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EmailManagementResource>(name);
    }
}
