using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Attachment;

public interface IMailAttachmentAppService : IApplicationService
{
    Task<PagedResultDto<MailAttachmentDto>> GetListAsync(GetAttachmentListInput input);

    Task<MailAttachmentDto> GetAsync(Guid id);

    Task<MailAttachmentDto> CreateAsync(CreateMailAttachmentInput input);

    Task RecordDownloadAsync(Guid id);

    Task MarkAsSensitiveAsync(Guid id);

    Task UpdateScanResultAsync(Guid id, UpdateScanResultInput input);

    Task DeleteAsync(Guid id);
}
