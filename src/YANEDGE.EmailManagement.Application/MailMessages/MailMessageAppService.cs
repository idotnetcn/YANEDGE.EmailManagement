using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Conversations;
using YANEDGE.EmailManagement.Enums;
using YANEDGE.EmailManagement.MailMessages;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.MailMessages;

[Authorize(EmailManagementPermissions.MailMessages.Default)]
public class MailMessageAppService : ApplicationService, IMailMessageAppService
{
    private readonly IRepository<MailMessage, Guid> _messageRepository;
    private readonly IRepository<MailMessageBody, Guid> _bodyRepository;

    public MailMessageAppService(
        IRepository<MailMessage, Guid> messageRepository,
        IRepository<MailMessageBody, Guid> bodyRepository)
    {
        _messageRepository = messageRepository;
        _bodyRepository = bodyRepository;
    }

    public async Task<PagedResultDto<MailMessageDto>> GetListAsync(MailMessageListRequestDto input)
    {
        var query = await _messageRepository.GetQueryableAsync();

        query = query
            .WhereIf(input.MailAccountId.HasValue, x => x.MailAccountId == input.MailAccountId!.Value)
            .WhereIf(input.ThreadId.HasValue, x => x.ThreadId == input.ThreadId!.Value)
            .WhereIf(input.FolderType.HasValue, x => x.FolderType == input.FolderType!.Value)
            .WhereIf(input.MailDirection.HasValue, x => x.MailDirection == input.MailDirection!.Value)
            .WhereIf(input.ProcessingStatus.HasValue, x => x.ProcessingStatus == input.ProcessingStatus!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => (x.Subject != null && x.Subject.Contains(input.Filter!)) ||
                     (x.FromAddress != null && x.FromAddress.Contains(input.Filter!)))
            .WhereIf(!string.IsNullOrWhiteSpace(input.FromAddress),
                x => x.FromAddress != null && x.FromAddress.Contains(input.FromAddress!))
            .WhereIf(input.StartTime.HasValue, x => x.ReceivedTime >= input.StartTime)
            .WhereIf(input.EndTime.HasValue, x => x.ReceivedTime <= input.EndTime);

        var total = query.Count();
        var items = query
            .OrderByDescending(x => x.ReceivedTime ?? x.SentTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<MailMessageDto>(
            total,
            ObjectMapper.Map<List<MailMessage>, List<MailMessageDto>>(items)
        );
    }

    public async Task<MailMessageDto> GetAsync(Guid id)
    {
        var entity = await _messageRepository.GetAsync(id);
        return ObjectMapper.Map<MailMessage, MailMessageDto>(entity);
    }

    public async Task<MailMessageBodyDto> GetBodyAsync(Guid id)
    {
        var query = await _bodyRepository.GetQueryableAsync();
        var body = query.FirstOrDefault(x => x.MailMessageId == id);
        if (body == null) return new MailMessageBodyDto { MailMessageId = id };
        return ObjectMapper.Map<MailMessageBody, MailMessageBodyDto>(body);
    }

    [Authorize(EmailManagementPermissions.MailMessages.Create)]
    public async Task<MailMessageDto> CreateDraftAsync(CreateMailMessageInput input)
    {
        var message = new MailMessage(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.MailAccountId,
            MailDirection.Outbound,
            FolderType.Draft
        );

        message.Subject = input.Subject;
        message.BodyFormat = input.BodyFormat;
        message.Importance = input.Importance;
        message.SecurityLevel = input.SecurityLevel;
        message.CreatedByUserId = CurrentUser.Id;
        message.ApprovalStatus = input.NeedApproval ? ApprovalStatus.Pending : ApprovalStatus.NotRequired;

        foreach (var r in input.ToRecipients)
        {
            message.Recipients.Add(new MailRecipient(GuidGenerator.Create(), message.Id, RecipientType.To, r.EmailAddress)
            {
                DisplayName = r.DisplayName
            });
        }
        foreach (var r in input.CcRecipients)
        {
            message.Recipients.Add(new MailRecipient(GuidGenerator.Create(), message.Id, RecipientType.Cc, r.EmailAddress)
            {
                DisplayName = r.DisplayName
            });
        }
        foreach (var r in input.BccRecipients)
        {
            message.Recipients.Add(new MailRecipient(GuidGenerator.Create(), message.Id, RecipientType.Bcc, r.EmailAddress)
            {
                DisplayName = r.DisplayName
            });
        }

        if (!string.IsNullOrEmpty(input.BodyHtml) || !string.IsNullOrEmpty(input.BodyText))
        {
            message.Body = new MailMessageBody(GuidGenerator.Create(), message.Id)
            {
                BodyHtmlRaw = input.BodyHtml,
                BodyText = input.BodyText
            };
        }

        await _messageRepository.InsertAsync(message, autoSave: true);
        return ObjectMapper.Map<MailMessage, MailMessageDto>(message);
    }

    [Authorize(EmailManagementPermissions.MailMessages.Send)]
    public async Task SendAsync(Guid id)
    {
        var entity = await _messageRepository.GetAsync(id);
        entity.ProcessingStatus = ProcessingStatus.Processing;
        entity.FolderType = FolderType.Sent;
        entity.SentByUserId = CurrentUser.Id;
        entity.SentTime = DateTime.UtcNow;
        await _messageRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailMessages.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _messageRepository.DeleteAsync(id, autoSave: true);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await Task.CompletedTask;
    }

    public async Task MarkAsUnreadAsync(Guid id)
    {
        await Task.CompletedTask;
    }

    [Authorize(EmailManagementPermissions.MailThreads.Archive)]
    public async Task ArchiveAsync(Guid id)
    {
        var entity = await _messageRepository.GetAsync(id);
        entity.FolderType = FolderType.Archive;
        entity.ProcessingStatus = ProcessingStatus.Archived;
        await _messageRepository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailThreads.Assign)]
    public async Task AssignAsync(Guid id, AssignMailMessageInput input)
    {
        var entity = await _messageRepository.GetAsync(id);
        entity.ProcessingStatus = ProcessingStatus.PendingProcess;
        entity.LastOperatorUserId = CurrentUser.Id;
        await _messageRepository.UpdateAsync(entity, autoSave: true);
    }
}
