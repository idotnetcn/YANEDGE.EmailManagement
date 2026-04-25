using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Contacts;

public interface IContactAppService : IApplicationService
{
    Task<PagedResultDto<ContactDto>> GetListAsync(ContactListRequestDto input);
    Task<ContactDto> GetAsync(Guid id);
    Task<ContactDto> CreateAsync(CreateUpdateContactInput input);
    Task<ContactDto> UpdateAsync(Guid id, CreateUpdateContactInput input);
    Task DeleteAsync(Guid id);
}
