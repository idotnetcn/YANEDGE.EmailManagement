using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Rule;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Rule;

[Authorize(EmailManagementPermissions.Rules.Default)]
public class MailRuleAppService : ApplicationService, IMailRuleAppService
{
    private readonly IRepository<Domain.Rule.MailRule, Guid> _ruleRepository;

    public MailRuleAppService(
        IRepository<Domain.Rule.MailRule, Guid> ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    public async Task<PagedResultDto<MailRuleDto>> GetListAsync(GetRuleListInput input)
    {
        var query = await _ruleRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.Name.Contains(input.Keyword) || (x.Description != null && x.Description.Contains(input.Keyword)));
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == input.IsActive.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.Priority).ThenBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailRuleDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Rule.MailRule>, System.Collections.Generic.List<MailRuleDto>>(items)
        );
    }

    public async Task<MailRuleDto> GetAsync(Guid id)
    {
        var rule = await _ruleRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Rule.MailRule, MailRuleDto>(rule);
    }

    [Authorize(EmailManagementPermissions.Rules.Create)]
    public async Task<MailRuleDto> CreateAsync(CreateMailRuleInput input)
    {
        var rule = new Domain.Rule.MailRule(
            GuidGenerator.Create(),
            input.Name,
            input.Priority,
            input.Description
        );

        rule.SetApplicableMailAccounts(input.ApplicableMailAccountIds);

        foreach (var condition in input.Conditions)
        {
            rule.AddCondition(condition.ConditionType, condition.Value);
        }

        foreach (var action in input.Actions)
        {
            rule.AddAction(action.ActionType, action.Parameters);
        }

        await _ruleRepository.InsertAsync(rule);

        return ObjectMapper.Map<Domain.Rule.MailRule, MailRuleDto>(rule);
    }

    [Authorize(EmailManagementPermissions.Rules.Update)]
    public async Task<MailRuleDto> UpdateAsync(Guid id, UpdateMailRuleInput input)
    {
        var rule = await _ruleRepository.GetAsync(id);

        rule.Update(input.Name, input.Priority, input.Description);
        rule.SetApplicableMailAccounts(input.ApplicableMailAccountIds);

        rule.ClearConditions();
        foreach (var condition in input.Conditions)
        {
            rule.AddCondition(condition.ConditionType, condition.Value);
        }

        rule.ClearActions();
        foreach (var action in input.Actions)
        {
            rule.AddAction(action.ActionType, action.Parameters);
        }

        await _ruleRepository.UpdateAsync(rule);

        return ObjectMapper.Map<Domain.Rule.MailRule, MailRuleDto>(rule);
    }

    [Authorize(EmailManagementPermissions.Rules.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var rule = await _ruleRepository.GetAsync(id);
        rule.Activate();
        await _ruleRepository.UpdateAsync(rule);
    }

    [Authorize(EmailManagementPermissions.Rules.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var rule = await _ruleRepository.GetAsync(id);
        rule.Deactivate();
        await _ruleRepository.UpdateAsync(rule);
    }

    [Authorize(EmailManagementPermissions.Rules.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _ruleRepository.DeleteAsync(id);
    }
}
