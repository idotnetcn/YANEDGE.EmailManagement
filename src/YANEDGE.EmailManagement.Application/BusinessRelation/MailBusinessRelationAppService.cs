using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.BusinessRelation;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.BusinessRelation;

[Authorize(EmailManagementPermissions.BusinessRelations.Default)]
public class MailBusinessRelationAppService : ApplicationService, IMailBusinessRelationAppService
{
    private readonly IRepository<Domain.BusinessRelation.MailBusinessRelation, Guid> _businessRelationRepository;

    public MailBusinessRelationAppService(
        IRepository<Domain.BusinessRelation.MailBusinessRelation, Guid> businessRelationRepository)
    {
        _businessRelationRepository = businessRelationRepository;
    }

    public async Task<PagedResultDto<MailBusinessRelationDto>> GetListAsync(GetBusinessRelationListInput input)
    {
        var query = await _businessRelationRepository.GetQueryableAsync();

        if (input.MailMessageId.HasValue)
        {
            query = query.Where(x => x.MailMessageId == input.MailMessageId.Value);
        }

        if (input.ThreadId.HasValue)
        {
            query = query.Where(x => x.ThreadId == input.ThreadId.Value);
        }

        if (input.BusinessObjectType.HasValue)
        {
            query = query.Where(x => x.BusinessObjectType == input.BusinessObjectType.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.BusinessObjectId))
        {
            query = query.Where(x => x.BusinessObjectId == input.BusinessObjectId);
        }

        if (!string.IsNullOrWhiteSpace(input.RelationSource))
        {
            query = query.Where(x => x.RelationSource == input.RelationSource);
        }

        if (input.IsPrimary.HasValue)
        {
            query = query.Where(x => x.IsPrimary == input.IsPrimary.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.IsPrimary).ThenByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailBusinessRelationDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.BusinessRelation.MailBusinessRelation>, System.Collections.Generic.List<MailBusinessRelationDto>>(items)
        );
    }

    public async Task<MailBusinessRelationDto> GetAsync(Guid id)
    {
        var businessRelation = await _businessRelationRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.BusinessRelation.MailBusinessRelation, MailBusinessRelationDto>(businessRelation);
    }

    [Authorize(EmailManagementPermissions.BusinessRelations.Create)]
    public async Task<MailBusinessRelationDto> CreateAsync(CreateMailBusinessRelationInput input)
    {
        var businessRelation = new Domain.BusinessRelation.MailBusinessRelation(
            GuidGenerator.Create(),
            input.MailMessageId,
            input.ThreadId,
            input.BusinessObjectType,
            input.BusinessObjectId,
            input.RelationSource,
            input.BusinessObjectName,
            input.BusinessObjectCode,
            input.IsPrimary,
            input.ExternalSystem,
            input.Notes
        );

        await _businessRelationRepository.InsertAsync(businessRelation);

        return ObjectMapper.Map<Domain.BusinessRelation.MailBusinessRelation, MailBusinessRelationDto>(businessRelation);
    }

    [Authorize(EmailManagementPermissions.BusinessRelations.Update)]
    public async Task<MailBusinessRelationDto> UpdateAsync(Guid id, UpdateMailBusinessRelationInput input)
    {
        var businessRelation = await _businessRelationRepository.GetAsync(id);

        businessRelation.Update(
            input.BusinessObjectName,
            input.BusinessObjectCode,
            input.IsPrimary,
            input.Notes
        );

        await _businessRelationRepository.UpdateAsync(businessRelation);

        return ObjectMapper.Map<Domain.BusinessRelation.MailBusinessRelation, MailBusinessRelationDto>(businessRelation);
    }

    [Authorize(EmailManagementPermissions.BusinessRelations.Update)]
    public async Task SetAsPrimaryAsync(Guid id)
    {
        var businessRelation = await _businessRelationRepository.GetAsync(id);
        businessRelation.SetAsPrimary();
        await _businessRelationRepository.UpdateAsync(businessRelation);
    }

    [Authorize(EmailManagementPermissions.BusinessRelations.Update)]
    public async Task UnsetAsPrimaryAsync(Guid id)
    {
        var businessRelation = await _businessRelationRepository.GetAsync(id);
        businessRelation.UnsetAsPrimary();
        await _businessRelationRepository.UpdateAsync(businessRelation);
    }

    [Authorize(EmailManagementPermissions.BusinessRelations.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _businessRelationRepository.DeleteAsync(id);
    }
}
