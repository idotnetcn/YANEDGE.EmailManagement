using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Signature;

public interface IMailSignatureAppService : IApplicationService
{
    Task<PagedResultDto<MailSignatureDto>> GetListAsync(GetSignatureListInput input);

    Task<MailSignatureDto> GetAsync(Guid id);

    Task<MailSignatureDto?> GetDefaultByUserIdAsync(Guid userId);

    Task<MailSignatureDto> CreateAsync(CreateMailSignatureInput input);

    Task<MailSignatureDto> UpdateAsync(Guid id, UpdateMailSignatureInput input);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task SetAsDefaultAsync(Guid id);

    Task UnsetAsDefaultAsync(Guid id);

    Task DeleteAsync(Guid id);
}
