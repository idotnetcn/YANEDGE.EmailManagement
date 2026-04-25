using System;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Emails;

[Authorize(EmailManagementPermissions.Emails.Default)]
public class EmailAppService : ApplicationService, IEmailAppService
{
    private readonly IEmailRepository _emailRepository;
    private readonly EmailManager _emailManager;

    public EmailAppService(IEmailRepository emailRepository, EmailManager emailManager)
    {
        _emailRepository = emailRepository;
        _emailManager = emailManager;
    }

    public async Task<EmailDto> GetAsync(Guid id)
    {
        var email = await _emailRepository.GetAsync(id);
        return ObjectMapper.Map<Email, EmailDto>(email);
    }

    public async Task<PagedResultDto<EmailDto>> GetListAsync(GetEmailListDto input)
    {
        var totalCount = await _emailRepository.GetCountAsync(
            input.Status,
            input.Priority,
            input.FromAddress);

        var emails = await _emailRepository.GetListAsync(
            input.Status,
            input.Priority,
            input.FromAddress,
            input.ScheduledBefore,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<EmailDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Email>, System.Collections.Generic.List<EmailDto>>(emails));
    }

    [Authorize(EmailManagementPermissions.Emails.Create)]
    public async Task<EmailDto> CreateAsync(CreateEmailDto input)
    {
        var email = await _emailManager.CreateAsync(
            input.Subject,
            input.Body,
            input.FromAddress,
            input.IsBodyHtml,
            input.Priority,
            input.FromDisplayName);

        foreach (var recipientDto in input.Recipients)
        {
            email.Recipients.Add(new EmailRecipient(
                GuidGenerator.Create(),
                email.Id,
                recipientDto.Address,
                recipientDto.RecipientType,
                recipientDto.DisplayName));
        }

        await _emailRepository.UpdateAsync(email);
        return ObjectMapper.Map<Email, EmailDto>(email);
    }

    [Authorize(EmailManagementPermissions.Emails.Edit)]
    public async Task<EmailDto> UpdateAsync(Guid id, UpdateEmailDto input)
    {
        var email = await _emailRepository.GetAsync(id);
        email.UpdateContent(input.Subject, input.Body);
        await _emailRepository.UpdateAsync(email);
        return ObjectMapper.Map<Email, EmailDto>(email);
    }

    [Authorize(EmailManagementPermissions.Emails.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _emailRepository.DeleteAsync(id);
    }

    [Authorize(EmailManagementPermissions.Emails.Send)]
    public async Task<EmailDto> QueueAsync(Guid id, QueueEmailDto input)
    {
        var email = await _emailManager.QueueAsync(id, input.ScheduledAt);
        return ObjectMapper.Map<Email, EmailDto>(email);
    }

    [Authorize(EmailManagementPermissions.Emails.Edit)]
    public async Task<EmailDto> CancelAsync(Guid id)
    {
        var email = await _emailRepository.GetAsync(id);
        email.Cancel();
        await _emailRepository.UpdateAsync(email);
        return ObjectMapper.Map<Email, EmailDto>(email);
    }
}
