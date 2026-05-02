using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Attachment;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Attachment;

[Authorize(EmailManagementPermissions.Attachments.Default)]
public class MailAttachmentAppService : ApplicationService, IMailAttachmentAppService
{
    private readonly IRepository<Domain.Attachment.MailAttachment, Guid> _attachmentRepository;

    public MailAttachmentAppService(
        IRepository<Domain.Attachment.MailAttachment, Guid> attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<PagedResultDto<MailAttachmentDto>> GetListAsync(GetAttachmentListInput input)
    {
        var query = await _attachmentRepository.GetQueryableAsync();

        if (input.MailMessageId.HasValue)
        {
            query = query.Where(x => x.MailMessageId == input.MailMessageId.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.FileName.Contains(input.Keyword));
        }

        if (input.IsSensitive.HasValue)
        {
            query = query.Where(x => x.IsSensitive == input.IsSensitive.Value);
        }

        if (input.IsScanned.HasValue)
        {
            query = query.Where(x => x.IsScanned == input.IsScanned.Value);
        }

        if (input.IsSafe.HasValue)
        {
            query = query.Where(x => x.IsSafe == input.IsSafe.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailAttachmentDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Attachment.MailAttachment>, System.Collections.Generic.List<MailAttachmentDto>>(items)
        );
    }

    public async Task<MailAttachmentDto> GetAsync(Guid id)
    {
        var attachment = await _attachmentRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Attachment.MailAttachment, MailAttachmentDto>(attachment);
    }

    [Authorize(EmailManagementPermissions.Attachments.Create)]
    public async Task<MailAttachmentDto> CreateAsync(CreateMailAttachmentInput input)
    {
        var attachment = new Domain.Attachment.MailAttachment(
            GuidGenerator.Create(),
            input.MailMessageId,
            input.FileName,
            input.ContentType,
            input.FileSize,
            input.StoragePath,
            input.FileHash,
            input.ContentId,
            input.IsInline,
            input.IsSensitive
        );

        await _attachmentRepository.InsertAsync(attachment);

        return ObjectMapper.Map<Domain.Attachment.MailAttachment, MailAttachmentDto>(attachment);
    }

    [Authorize(EmailManagementPermissions.Attachments.Download)]
    public async Task RecordDownloadAsync(Guid id)
    {
        var attachment = await _attachmentRepository.GetAsync(id);
        attachment.RecordDownload();
        await _attachmentRepository.UpdateAsync(attachment);
    }

    [Authorize(EmailManagementPermissions.Attachments.Update)]
    public async Task MarkAsSensitiveAsync(Guid id)
    {
        var attachment = await _attachmentRepository.GetAsync(id);
        attachment.MarkAsSensitive();
        await _attachmentRepository.UpdateAsync(attachment);
    }

    [Authorize(EmailManagementPermissions.Attachments.Update)]
    public async Task UpdateScanResultAsync(Guid id, UpdateScanResultInput input)
    {
        var attachment = await _attachmentRepository.GetAsync(id);
        attachment.UpdateScanResult(input.IsSafe, input.ScanResult);
        await _attachmentRepository.UpdateAsync(attachment);
    }

    [Authorize(EmailManagementPermissions.Attachments.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _attachmentRepository.DeleteAsync(id);
    }
}
