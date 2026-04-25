using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Accounts;
using YANEDGE.EmailManagement.MailAccounts;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.MailAccounts;

[Authorize(EmailManagementPermissions.MailAccounts.Default)]
public class MailAccountAppService : ApplicationService, IMailAccountAppService
{
    private readonly IRepository<MailAccount, Guid> _repository;

    public MailAccountAppService(IRepository<MailAccount, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MailAccountDto>> GetListAsync(MailAccountListRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!) || x.EmailAddress.Contains(input.Filter!))
            .WhereIf(input.AccountType.HasValue, x => x.AccountType == input.AccountType!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.OwnerUserId.HasValue, x => x.OwnerUserId == input.OwnerUserId!.Value);

        var total = query.Count();
        var items = query
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<MailAccountDto>(
            total,
            ObjectMapper.Map<List<MailAccount>, List<MailAccountDto>>(items)
        );
    }

    public async Task<MailAccountDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<MailAccount, MailAccountDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Create)]
    public async Task<MailAccountDto> CreateAsync(CreateMailAccountInput input)
    {
        var entity = new MailAccount(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.Name,
            input.EmailAddress,
            input.AccountType,
            input.InboundProtocol,
            input.OutboundProtocol
        );

        entity.DisplayName = input.DisplayName;
        entity.ImapHost = input.ImapHost;
        entity.ImapPort = input.ImapPort;
        entity.SmtpHost = input.SmtpHost;
        entity.SmtpPort = input.SmtpPort;
        entity.UseSsl = input.UseSsl;
        entity.SyncIntervalSeconds = input.SyncIntervalSeconds;
        entity.Description = input.Description;

        if (input.OwnerType.HasValue)
        {
            entity.ChangeOwner(input.OwnerType.Value, input.OwnerUserId, null, null);
        }

        await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailAccount, MailAccountDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Update)]
    public async Task<MailAccountDto> UpdateAsync(Guid id, UpdateMailAccountInput input)
    {
        var entity = await _repository.GetAsync(id);
        entity.DisplayName = input.DisplayName;
        entity.ImapHost = input.ImapHost;
        entity.ImapPort = input.ImapPort;
        entity.SmtpHost = input.SmtpHost;
        entity.SmtpPort = input.SmtpPort;
        entity.UseSsl = input.UseSsl;
        entity.SyncIntervalSeconds = input.SyncIntervalSeconds;
        entity.Description = input.Description;

        await _repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailAccount, MailAccountDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Update)]
    public async Task EnableSyncAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.EnableSync();
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Update)]
    public async Task DisableSyncAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.DisableSync();
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Update)]
    public async Task EnableSendAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.EnableSend();
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailAccounts.Update)]
    public async Task DisableSendAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.DisableSend();
        await _repository.UpdateAsync(entity, autoSave: true);
    }
}
