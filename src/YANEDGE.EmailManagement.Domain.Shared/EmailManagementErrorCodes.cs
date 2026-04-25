namespace YANEDGE.EmailManagement;

public static class EmailManagementErrorCodes
{
    public const string MailAccountNotFound = "EmailManagement:00001";
    public const string MailAccountAlreadyExists = "EmailManagement:00002";
    public const string MailAccountSendDisabled = "EmailManagement:00003";
    public const string MailAccountSyncDisabled = "EmailManagement:00004";
    public const string MailAccountPermissionDenied = "EmailManagement:00005";
    public const string MailAccountDuplicateUserPermission = "EmailManagement:00006";

    public const string MailThreadNotFound = "EmailManagement:01001";
    public const string MailMessageNotFound = "EmailManagement:01002";
    public const string MailMessageAlreadyLocked = "EmailManagement:01003";

    public const string MailTemplateNotFound = "EmailManagement:02001";
    public const string MailTemplateDuplicateCode = "EmailManagement:02002";

    public const string MailRuleNotFound = "EmailManagement:03001";

    public const string ContactNotFound = "EmailManagement:04001";
    public const string ContactDuplicateEmail = "EmailManagement:04002";

    public const string SendTaskNotFound = "EmailManagement:05001";
    public const string SendTaskAlreadyCancelled = "EmailManagement:05002";

    public const string IntegrationAppNotFound = "EmailManagement:06001";
    public const string IntegrationAppDuplicateAppId = "EmailManagement:06002";
}
