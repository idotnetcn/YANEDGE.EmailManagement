using AutoMapper;
using YANEDGE.EmailManagement.Accounts;
using YANEDGE.EmailManagement.MailAccounts;
using YANEDGE.EmailManagement.Conversations;
using YANEDGE.EmailManagement.MailMessages;
using YANEDGE.EmailManagement.MailThreads;
using YANEDGE.EmailManagement.Templates;
using YANEDGE.EmailManagement.MailTemplates;
using YANEDGE.EmailManagement.BusinessRelations;
using YANEDGE.EmailManagement.Contacts;
using YANEDGE.EmailManagement.Rules;
using YANEDGE.EmailManagement.MailRules;
using YANEDGE.EmailManagement.Integrations;
using YANEDGE.EmailManagement.IntegrationApps;

namespace YANEDGE.EmailManagement;

public class EmailManagementApplicationAutoMapperProfile : Profile
{
    public EmailManagementApplicationAutoMapperProfile()
    {
        CreateMap<MailAccount, MailAccountDto>();
        CreateMap<MailMessage, MailMessageDto>();
        CreateMap<MailRecipient, MailRecipientDto>();
        CreateMap<MailMessageBody, MailMessageBodyDto>();
        CreateMap<MailThread, MailThreadDto>();
        CreateMap<MailTemplate, MailTemplateDto>();
        CreateMap<Contact, ContactDto>();
        CreateMap<MailRule, MailRuleDto>();
        CreateMap<MailRuleCondition, MailRuleConditionDto>();
        CreateMap<MailRuleAction, MailRuleActionDto>();
        CreateMap<IntegrationApp, IntegrationAppDto>();
    }
}
