using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Contact;

public interface IMailContactAppService : IApplicationService
{
    Task<PagedResultDto<MailContactDto>> GetListAsync(GetContactListInput input);

    Task<MailContactDto> GetAsync(Guid id);

    Task<MailContactDto> GetByEmailAsync(string emailAddress);

    Task<MailContactDto> CreateAsync(CreateMailContactInput input);

    Task<MailContactDto> UpdateAsync(Guid id, UpdateMailContactInput input);

    Task VerifyAsync(Guid id);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task LinkToCustomerAsync(Guid id, string customerId);

    Task LinkToSupplierAsync(Guid id, string supplierId);

    Task DeleteAsync(Guid id);
}
