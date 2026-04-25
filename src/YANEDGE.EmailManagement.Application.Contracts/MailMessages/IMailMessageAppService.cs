using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.MailMessages;

public interface IMailMessageAppService : IApplicationService
{
    Task<PagedResultDto<MailMessageDto>> GetListAsync(MailMessageListRequestDto input);
    Task<MailMessageDto> GetAsync(Guid id);
    Task<MailMessageBodyDto> GetBodyAsync(Guid id);
    Task<MailMessageDto> CreateDraftAsync(CreateMailMessageInput input);
    Task SendAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task MarkAsReadAsync(Guid id);
    Task MarkAsUnreadAsync(Guid id);
    Task ArchiveAsync(Guid id);
    Task AssignAsync(Guid id, AssignMailMessageInput input);
}
