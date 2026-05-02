using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Signature;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Signature;

[Authorize(EmailManagementPermissions.Templates.Default)]
public class MailSignatureAppService : ApplicationService, IMailSignatureAppService
{
    private readonly IRepository<Domain.Signature.MailSignature, Guid> _signatureRepository;
    private readonly Domain.Signature.IMailSignatureRepository _mailSignatureRepository;

    public MailSignatureAppService(
        IRepository<Domain.Signature.MailSignature, Guid> signatureRepository,
        Domain.Signature.IMailSignatureRepository mailSignatureRepository)
    {
        _signatureRepository = signatureRepository;
        _mailSignatureRepository = mailSignatureRepository;
    }

    public async Task<PagedResultDto<MailSignatureDto>> GetListAsync(GetSignatureListInput input)
    {
        var query = await _signatureRepository.GetQueryableAsync();

        if (input.Scope.HasValue)
        {
            query = query.Where(x => x.Scope == input.Scope.Value);
        }

        if (input.OwnerUserId.HasValue)
        {
            query = query.Where(x => x.OwnerUserId == input.OwnerUserId.Value);
        }

        if (input.OwnerOrganizationId.HasValue)
        {
            query = query.Where(x => x.OwnerOrganizationId == input.OwnerOrganizationId.Value);
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == input.IsActive.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailSignatureDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Signature.MailSignature>, System.Collections.Generic.List<MailSignatureDto>>(items)
        );
    }

    public async Task<MailSignatureDto> GetAsync(Guid id)
    {
        var signature = await _signatureRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Signature.MailSignature, MailSignatureDto>(signature);
    }

    public async Task<MailSignatureDto?> GetDefaultByUserIdAsync(Guid userId)
    {
        var signature = await _mailSignatureRepository.GetDefaultSignatureByUserIdAsync(userId);
        if (signature == null)
        {
            return null;
        }
        return ObjectMapper.Map<Domain.Signature.MailSignature, MailSignatureDto>(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Create)]
    public async Task<MailSignatureDto> CreateAsync(CreateMailSignatureInput input)
    {
        var signature = new Domain.Signature.MailSignature(
            GuidGenerator.Create(),
            input.Name,
            input.Content,
            input.Scope,
            input.OwnerUserId,
            input.OwnerOrganizationId
        );

        signature.SetPlainTextContent(input.PlainTextContent);

        if (input.IsDefault)
        {
            signature.SetAsDefault();
        }

        await _signatureRepository.InsertAsync(signature);

        return ObjectMapper.Map<Domain.Signature.MailSignature, MailSignatureDto>(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task<MailSignatureDto> UpdateAsync(Guid id, UpdateMailSignatureInput input)
    {
        var signature = await _signatureRepository.GetAsync(id);

        signature.Update(input.Name, input.Content, input.PlainTextContent);

        await _signatureRepository.UpdateAsync(signature);

        return ObjectMapper.Map<Domain.Signature.MailSignature, MailSignatureDto>(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var signature = await _signatureRepository.GetAsync(id);
        signature.Activate();
        await _signatureRepository.UpdateAsync(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var signature = await _signatureRepository.GetAsync(id);
        signature.Deactivate();
        await _signatureRepository.UpdateAsync(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task SetAsDefaultAsync(Guid id)
    {
        var signature = await _signatureRepository.GetAsync(id);
        signature.SetAsDefault();
        await _signatureRepository.UpdateAsync(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task UnsetAsDefaultAsync(Guid id)
    {
        var signature = await _signatureRepository.GetAsync(id);
        signature.UnsetAsDefault();
        await _signatureRepository.UpdateAsync(signature);
    }

    [Authorize(EmailManagementPermissions.Templates.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _signatureRepository.DeleteAsync(id);
    }
}
