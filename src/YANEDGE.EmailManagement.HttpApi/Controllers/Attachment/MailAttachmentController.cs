using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Attachment;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Attachment;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/attachments")]
public class MailAttachmentController : AbpControllerBase
{
    private readonly IMailAttachmentAppService _attachmentAppService;

    public MailAttachmentController(IMailAttachmentAppService attachmentAppService)
    {
        _attachmentAppService = attachmentAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailAttachmentDto>> GetListAsync([FromQuery] GetAttachmentListInput input)
    {
        return _attachmentAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailAttachmentDto> GetAsync(Guid id)
    {
        return _attachmentAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<MailAttachmentDto> CreateAsync([FromBody] CreateMailAttachmentInput input)
    {
        return _attachmentAppService.CreateAsync(input);
    }

    [HttpPost]
    [Route("{id}/record-download")]
    public virtual Task RecordDownloadAsync(Guid id)
    {
        return _attachmentAppService.RecordDownloadAsync(id);
    }

    [HttpPost]
    [Route("{id}/mark-sensitive")]
    public virtual Task MarkAsSensitiveAsync(Guid id)
    {
        return _attachmentAppService.MarkAsSensitiveAsync(id);
    }

    [HttpPost]
    [Route("{id}/scan-result")]
    public virtual Task UpdateScanResultAsync(Guid id, [FromBody] UpdateScanResultInput input)
    {
        return _attachmentAppService.UpdateScanResultAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _attachmentAppService.DeleteAsync(id);
    }
}
