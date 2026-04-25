using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.BusinessRelations;
using YANEDGE.EmailManagement.Contacts;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Contacts;

[Authorize(EmailManagementPermissions.Contacts.Default)]
public class ContactAppService : ApplicationService, IContactAppService
{
    private readonly IRepository<Contact, Guid> _repository;

    public ContactAppService(IRepository<Contact, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ContactDto>> GetListAsync(ContactListRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!) || x.EmailAddress.Contains(input.Filter!))
            .WhereIf(input.ContactType.HasValue, x => x.ContactType == input.ContactType!.Value)
            .WhereIf(input.IsEnabled.HasValue, x => x.IsEnabled == input.IsEnabled!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(input.SourceSystem),
                x => x.SourceSystem == input.SourceSystem);

        var total = query.Count();
        var items = query
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<ContactDto>(
            total,
            ObjectMapper.Map<List<Contact>, List<ContactDto>>(items)
        );
    }

    public async Task<ContactDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<Contact, ContactDto>(entity);
    }

    [Authorize(EmailManagementPermissions.Contacts.Create)]
    public async Task<ContactDto> CreateAsync(CreateUpdateContactInput input)
    {
        var entity = new Contact(GuidGenerator.Create(), input.Name, input.EmailAddress, input.ContactType)
        {
            Mobile = input.Mobile,
            CompanyName = input.CompanyName,
            ExternalId = input.ExternalId,
            SourceSystem = input.SourceSystem,
            DepartmentName = input.DepartmentName,
            Title = input.Title,
            Remark = input.Remark
        };

        await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Contact, ContactDto>(entity);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task<ContactDto> UpdateAsync(Guid id, CreateUpdateContactInput input)
    {
        var entity = await _repository.GetAsync(id);
        entity.Name = input.Name;
        entity.EmailAddress = input.EmailAddress;
        entity.ContactType = input.ContactType;
        entity.Mobile = input.Mobile;
        entity.CompanyName = input.CompanyName;
        entity.ExternalId = input.ExternalId;
        entity.SourceSystem = input.SourceSystem;
        entity.DepartmentName = input.DepartmentName;
        entity.Title = input.Title;
        entity.Remark = input.Remark;

        await _repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<Contact, ContactDto>(entity);
    }

    [Authorize(EmailManagementPermissions.Contacts.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id, autoSave: true);
    }
}
