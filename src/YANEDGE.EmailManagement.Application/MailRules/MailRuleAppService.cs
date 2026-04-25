using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Rules;
using YANEDGE.EmailManagement.MailRules;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.MailRules;

[Authorize(EmailManagementPermissions.MailRules.Default)]
public class MailRuleAppService : ApplicationService, IMailRuleAppService
{
    private readonly IRepository<MailRule, Guid> _repository;

    public MailRuleAppService(IRepository<MailRule, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MailRuleDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();
        var total = query.Count();
        var items = query
            .OrderBy(x => x.Priority)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<MailRuleDto>(
            total,
            ObjectMapper.Map<List<MailRule>, List<MailRuleDto>>(items)
        );
    }

    public async Task<MailRuleDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<MailRule, MailRuleDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailRules.Create)]
    public async Task<MailRuleDto> CreateAsync(CreateUpdateMailRuleInput input)
    {
        var entity = new MailRule(GuidGenerator.Create(), input.Name, input.RuleType)
        {
            Priority = input.Priority,
            IsEnabled = input.IsEnabled,
            StopOnMatch = input.StopOnMatch,
            AppliesToAccountId = input.AppliesToAccountId,
            AppliesToFolderType = input.AppliesToFolderType,
            Description = input.Description
        };

        foreach (var c in input.Conditions)
        {
            entity.Conditions.Add(new MailRuleCondition(GuidGenerator.Create(), entity.Id, c.FieldName, c.Operator, c.CompareValue)
            {
                LogicalOperator = c.LogicalOperator,
                SortOrder = c.SortOrder
            });
        }

        foreach (var a in input.Actions)
        {
            entity.Actions.Add(new MailRuleAction(GuidGenerator.Create(), entity.Id, a.ActionType)
            {
                ActionValue = a.ActionValue,
                ActionOrder = a.ActionOrder
            });
        }

        await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailRule, MailRuleDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailRules.Update)]
    public async Task<MailRuleDto> UpdateAsync(Guid id, CreateUpdateMailRuleInput input)
    {
        var entity = await _repository.GetAsync(id);
        entity.Name = input.Name;
        entity.Priority = input.Priority;
        entity.IsEnabled = input.IsEnabled;
        entity.StopOnMatch = input.StopOnMatch;
        entity.AppliesToAccountId = input.AppliesToAccountId;
        entity.AppliesToFolderType = input.AppliesToFolderType;
        entity.Description = input.Description;

        await _repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailRule, MailRuleDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailRules.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id, autoSave: true);
    }
}
