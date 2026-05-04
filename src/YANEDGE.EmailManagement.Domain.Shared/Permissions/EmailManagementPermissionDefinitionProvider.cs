using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using YANEDGE.EmailManagement.Localization;

namespace YANEDGE.EmailManagement.Permissions;

/// <summary>
/// 邮件管理系统权限定义提供者
/// </summary>
public class EmailManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var mailManagementGroup = context.AddGroup(
            EmailManagementPermissions.GroupName,
            L("Permission:MailManagement"));

        // 邮箱账号权限
        var mailAccountsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.MailAccounts.Default,
            L("Permission:MailAccounts"));

        mailAccountsPermission.AddChild(
            EmailManagementPermissions.MailAccounts.Manage,
            L("Permission:MailAccounts.Manage"));

        mailAccountsPermission.AddChild(
            EmailManagementPermissions.MailAccounts.Sync,
            L("Permission:MailAccounts.Sync"));

        // 邮件权限
        var messagesPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Messages.Default,
            L("Permission:Messages"));

        messagesPermission.AddChild(
            EmailManagementPermissions.Messages.View,
            L("Permission:Messages.View"));

        // 邮件线程权限
        var threadsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Threads.Default,
            L("Permission:Threads"));

        threadsPermission.AddChild(
            EmailManagementPermissions.Threads.View,
            L("Permission:Threads.View"));

        threadsPermission.AddChild(
            EmailManagementPermissions.Threads.Assign,
            L("Permission:Threads.Assign"));

        threadsPermission.AddChild(
            EmailManagementPermissions.Threads.Claim,
            L("Permission:Threads.Claim"));

        threadsPermission.AddChild(
            EmailManagementPermissions.Threads.Archive,
            L("Permission:Threads.Archive"));

        // 发件任务权限
        var sendTasksPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.SendTasks.Default,
            L("Permission:SendTasks"));

        sendTasksPermission.AddChild(
            EmailManagementPermissions.SendTasks.Create,
            L("Permission:SendTasks.Create"));

        sendTasksPermission.AddChild(
            EmailManagementPermissions.SendTasks.Send,
            L("Permission:SendTasks.Send"));

        sendTasksPermission.AddChild(
            EmailManagementPermissions.SendTasks.Approve,
            L("Permission:SendTasks.Approve"));

        // 附件权限
        var attachmentsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Attachments.Default,
            L("Permission:Attachments"));

        attachmentsPermission.AddChild(
            EmailManagementPermissions.Attachments.Create,
            L("Permission:Attachments.Create"));

        attachmentsPermission.AddChild(
            EmailManagementPermissions.Attachments.Update,
            L("Permission:Attachments.Update"));

        attachmentsPermission.AddChild(
            EmailManagementPermissions.Attachments.Delete,
            L("Permission:Attachments.Delete"));

        attachmentsPermission.AddChild(
            EmailManagementPermissions.Attachments.Download,
            L("Permission:Attachments.Download"));

        // 规则权限
        var rulesPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Rules.Default,
            L("Permission:Rules"));

        rulesPermission.AddChild(
            EmailManagementPermissions.Rules.Create,
            L("Permission:Rules.Create"));

        rulesPermission.AddChild(
            EmailManagementPermissions.Rules.Update,
            L("Permission:Rules.Update"));

        rulesPermission.AddChild(
            EmailManagementPermissions.Rules.Delete,
            L("Permission:Rules.Delete"));

        rulesPermission.AddChild(
            EmailManagementPermissions.Rules.Manage,
            L("Permission:Rules.Manage"));

        // 模板权限
        var templatesPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Templates.Default,
            L("Permission:Templates"));

        templatesPermission.AddChild(
            EmailManagementPermissions.Templates.Create,
            L("Permission:Templates.Create"));

        templatesPermission.AddChild(
            EmailManagementPermissions.Templates.Update,
            L("Permission:Templates.Update"));

        templatesPermission.AddChild(
            EmailManagementPermissions.Templates.Delete,
            L("Permission:Templates.Delete"));

        templatesPermission.AddChild(
            EmailManagementPermissions.Templates.Manage,
            L("Permission:Templates.Manage"));

        // 标签权限
        var labelsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Labels.Default,
            L("Permission:Labels"));

        labelsPermission.AddChild(
            EmailManagementPermissions.Labels.Create,
            L("Permission:Labels.Create"));

        labelsPermission.AddChild(
            EmailManagementPermissions.Labels.Update,
            L("Permission:Labels.Update"));

        labelsPermission.AddChild(
            EmailManagementPermissions.Labels.Delete,
            L("Permission:Labels.Delete"));

        // 联系人权限
        var contactsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Contacts.Default,
            L("Permission:Contacts"));

        contactsPermission.AddChild(
            EmailManagementPermissions.Contacts.Create,
            L("Permission:Contacts.Create"));

        contactsPermission.AddChild(
            EmailManagementPermissions.Contacts.Update,
            L("Permission:Contacts.Update"));

        contactsPermission.AddChild(
            EmailManagementPermissions.Contacts.Delete,
            L("Permission:Contacts.Delete"));

        // 商务关系权限
        var businessRelationsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.BusinessRelations.Default,
            L("Permission:BusinessRelations"));

        businessRelationsPermission.AddChild(
            EmailManagementPermissions.BusinessRelations.Create,
            L("Permission:BusinessRelations.Create"));

        businessRelationsPermission.AddChild(
            EmailManagementPermissions.BusinessRelations.Update,
            L("Permission:BusinessRelations.Update"));

        businessRelationsPermission.AddChild(
            EmailManagementPermissions.BusinessRelations.Delete,
            L("Permission:BusinessRelations.Delete"));

        // 集成权限
        var integrationPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Integration.Default,
            L("Permission:Integration"));

        integrationPermission.AddChild(
            EmailManagementPermissions.Integration.Manage,
            L("Permission:Integration.Manage"));

        // 统计权限
        var statisticsPermission = mailManagementGroup.AddPermission(
            EmailManagementPermissions.Statistics.Default,
            L("Permission:Statistics"));

        statisticsPermission.AddChild(
            EmailManagementPermissions.Statistics.View,
            L("Permission:Statistics.View"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EmailManagementResource>(name);
    }
}
