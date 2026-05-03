using System;

namespace YANEDGE.EmailManagement.Exceptions;

/// <summary>
/// 邮箱账号不存在异常
/// </summary>
public class MailAccountNotFoundException : EmailManagementDomainException
{
    public MailAccountNotFoundException(Guid accountId)
        : base("MailAccount:NotFound", $"邮箱账号不存在: {accountId}")
    {
    }

    public MailAccountNotFoundException(string emailAddress)
        : base("MailAccount:NotFound", $"邮箱账号不存在: {emailAddress}")
    {
    }
}

/// <summary>
/// 邮件连接失败异常
/// </summary>
public class InvalidMailConnectionException : EmailManagementDomainException
{
    public InvalidMailConnectionException(string emailAddress, string reason, Exception? innerException = null)
        : base("MailAccount:ConnectionFailed",
            $"邮箱连接失败: {emailAddress}",
            reason,
            innerException)
    {
    }
}

/// <summary>
/// 发件任务未审批异常
/// </summary>
public class SendTaskNotApprovedException : EmailManagementDomainException
{
    public SendTaskNotApprovedException(Guid taskId)
        : base("SendTask:NotApproved",
            $"发件任务未审批，无法发送: {taskId}",
            "请先提交审批并等待审批通过后再发送")
    {
    }
}

/// <summary>
/// 发件任务状态无效异常
/// </summary>
public class InvalidSendTaskStateException : EmailManagementDomainException
{
    public InvalidSendTaskStateException(Guid taskId, string currentStatus, string expectedStatus)
        : base("SendTask:InvalidState",
            $"发件任务状态无效: {taskId}",
            $"当前状态: {currentStatus}, 期望状态: {expectedStatus}")
    {
    }
}

/// <summary>
/// 附件访问被拒绝异常
/// </summary>
public class AttachmentAccessDeniedException : EmailManagementDomainException
{
    public AttachmentAccessDeniedException(Guid attachmentId, string reason)
        : base("Attachment:AccessDenied",
            $"附件访问被拒绝: {attachmentId}",
            reason)
    {
    }
}

/// <summary>
/// 附件不存在异常
/// </summary>
public class AttachmentNotFoundException : EmailManagementDomainException
{
    public AttachmentNotFoundException(Guid attachmentId)
        : base("Attachment:NotFound", $"附件不存在: {attachmentId}")
    {
    }
}

/// <summary>
/// 规则执行失败异常
/// </summary>
public class RuleExecutionFailedException : EmailManagementDomainException
{
    public RuleExecutionFailedException(Guid ruleId, string reason, Exception? innerException = null)
        : base("Rule:ExecutionFailed",
            $"规则执行失败: {ruleId}",
            reason,
            innerException)
    {
    }
}

/// <summary>
/// 邮件线程不存在异常
/// </summary>
public class MailThreadNotFoundException : EmailManagementDomainException
{
    public MailThreadNotFoundException(Guid threadId)
        : base("MailThread:NotFound", $"邮件线程不存在: {threadId}")
    {
    }
}

/// <summary>
/// 模板不存在异常
/// </summary>
public class MailTemplateNotFoundException : EmailManagementDomainException
{
    public MailTemplateNotFoundException(Guid templateId)
        : base("MailTemplate:NotFound", $"邮件模板不存在: {templateId}")
    {
    }

    public MailTemplateNotFoundException(string templateCode)
        : base("MailTemplate:NotFound", $"邮件模板不存在: {templateCode}")
    {
    }
}

/// <summary>
/// 模板渲染失败异常
/// </summary>
public class TemplateRenderFailedException : EmailManagementDomainException
{
    public TemplateRenderFailedException(Guid templateId, string reason, Exception? innerException = null)
        : base("MailTemplate:RenderFailed",
            $"模板渲染失败: {templateId}",
            reason,
            innerException)
    {
    }
}

/// <summary>
/// 审批不存在异常
/// </summary>
public class ApprovalNotFoundException : EmailManagementDomainException
{
    public ApprovalNotFoundException(Guid approvalId)
        : base("Approval:NotFound", $"审批不存在: {approvalId}")
    {
    }
}

/// <summary>
/// 审批状态无效异常
/// </summary>
public class InvalidApprovalStateException : EmailManagementDomainException
{
    public InvalidApprovalStateException(Guid approvalId, string currentStatus, string expectedStatus)
        : base("Approval:InvalidState",
            $"审批状态无效: {approvalId}",
            $"当前状态: {currentStatus}, 期望状态: {expectedStatus}")
    {
    }
}
