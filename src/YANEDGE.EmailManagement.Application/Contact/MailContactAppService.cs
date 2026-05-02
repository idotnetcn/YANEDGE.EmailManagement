using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Contact;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Contact;

[Authorize(EmailManagementPermissions.Contacts.Default)]
public class MailContactAppService : ApplicationService, IMailContactAppService
{
    private readonly IRepository<Domain.Contact.MailContact, Guid> _contactRepository;

    public MailContactAppService(
        IRepository<Domain.Contact.MailContact, Guid> contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<PagedResultDto<MailContactDto>> GetListAsync(GetContactListInput input)
    {
        var query = await _contactRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.Name.Contains(input.Keyword)
                || x.EmailAddress.Contains(input.Keyword)
                || (x.CompanyName != null && x.CompanyName.Contains(input.Keyword)));
        }

        if (!string.IsNullOrWhiteSpace(input.Source))
        {
            query = query.Where(x => x.Source == input.Source);
        }

        if (input.IsVerified.HasValue)
        {
            query = query.Where(x => x.IsVerified == input.IsVerified.Value);
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == input.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.CustomerId))
        {
            query = query.Where(x => x.CustomerId == input.CustomerId);
        }

        if (!string.IsNullOrWhiteSpace(input.SupplierId))
        {
            query = query.Where(x => x.SupplierId == input.SupplierId);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.LastContactedAt).ThenBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailContactDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Contact.MailContact>, System.Collections.Generic.List<MailContactDto>>(items)
        );
    }

    public async Task<MailContactDto> GetAsync(Guid id)
    {
        var contact = await _contactRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Contact.MailContact, MailContactDto>(contact);
    }

    public async Task<MailContactDto> GetByEmailAsync(string emailAddress)
    {
        var query = await _contactRepository.GetQueryableAsync();
        var contact = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.EmailAddress == emailAddress));

        if (contact == null)
        {
            throw new Volo.Abp.BusinessException("Contact:NotFound")
                .WithData("EmailAddress", emailAddress);
        }

        return ObjectMapper.Map<Domain.Contact.MailContact, MailContactDto>(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Create)]
    public async Task<MailContactDto> CreateAsync(CreateMailContactInput input)
    {
        var query = await _contactRepository.GetQueryableAsync();
        var existingContact = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.EmailAddress == input.EmailAddress));

        if (existingContact != null)
        {
            throw new Volo.Abp.BusinessException("Contact:EmailAddressAlreadyExists")
                .WithData("EmailAddress", input.EmailAddress);
        }

        var contact = new Domain.Contact.MailContact(
            GuidGenerator.Create(),
            input.Name,
            input.EmailAddress,
            input.Source,
            input.PhoneNumber,
            input.CompanyName,
            input.JobTitle,
            input.CustomerId,
            input.SupplierId,
            input.ExternalId,
            input.Notes
        );

        await _contactRepository.InsertAsync(contact);

        return ObjectMapper.Map<Domain.Contact.MailContact, MailContactDto>(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task<MailContactDto> UpdateAsync(Guid id, UpdateMailContactInput input)
    {
        var contact = await _contactRepository.GetAsync(id);

        contact.Update(
            input.Name,
            input.PhoneNumber,
            input.CompanyName,
            input.JobTitle,
            input.Notes
        );

        await _contactRepository.UpdateAsync(contact);

        return ObjectMapper.Map<Domain.Contact.MailContact, MailContactDto>(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task VerifyAsync(Guid id)
    {
        var contact = await _contactRepository.GetAsync(id);
        contact.Verify();
        await _contactRepository.UpdateAsync(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var contact = await _contactRepository.GetAsync(id);
        contact.Activate();
        await _contactRepository.UpdateAsync(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var contact = await _contactRepository.GetAsync(id);
        contact.Deactivate();
        await _contactRepository.UpdateAsync(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task LinkToCustomerAsync(Guid id, string customerId)
    {
        var contact = await _contactRepository.GetAsync(id);
        contact.LinkToCustomer(customerId);
        await _contactRepository.UpdateAsync(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Update)]
    public async Task LinkToSupplierAsync(Guid id, string supplierId)
    {
        var contact = await _contactRepository.GetAsync(id);
        contact.LinkToSupplier(supplierId);
        await _contactRepository.UpdateAsync(contact);
    }

    [Authorize(EmailManagementPermissions.Contacts.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _contactRepository.DeleteAsync(id);
    }
}
