using AutoMapper;
using YANEDGE.EmailManagement.Application.Contracts.MailAccount;
using YANEDGE.EmailManagement.Application.Contracts.MailThread;
using YANEDGE.EmailManagement.Application.Contracts.MailMessage;
using YANEDGE.EmailManagement.Application.Contracts.MailCompose;
using YANEDGE.EmailManagement.Application.Contracts.Approval;
using YANEDGE.EmailManagement.Template;
using YANEDGE.EmailManagement.Signature;
using YANEDGE.EmailManagement.Label;
using YANEDGE.EmailManagement.Rule;
using YANEDGE.EmailManagement.Attachment;
using YANEDGE.EmailManagement.Contact;
using YANEDGE.EmailManagement.BusinessRelation;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Domain.Label;
using YANEDGE.EmailManagement.Domain.Rule;
using YANEDGE.EmailManagement.Domain.Attachment;
using YANEDGE.EmailManagement.Domain.Contact;
using YANEDGE.EmailManagement.Domain.BusinessRelation;
using YANEDGE.EmailManagement.Domain.Webhook;
using YANEDGE.EmailManagement.Webhook;

namespace YANEDGE.EmailManagement;

public class EmailManagementApplicationAutoMapperProfile : Profile
{
    public EmailManagementApplicationAutoMapperProfile()
    {
        // MailAccount mappings
        CreateMap<MailAccount, MailAccountDto>();
        CreateMap<CreateMailAccountInput, MailAccount>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // MailThread mappings
        CreateMap<MailThread, MailThreadDto>();

        // MailMessage mappings
        CreateMap<MailMessage, MailMessageDto>();

        // MailSendTask mappings
        CreateMap<MailSendTask, MailSendTaskDto>();
        CreateMap<CreateSendTaskInput, MailSendTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        // MailApproval mappings
        CreateMap<MailApproval, MailApprovalDto>();

        // MailTemplate mappings
        CreateMap<MailTemplate, MailTemplateDto>();
        CreateMap<CreateMailTemplateInput, MailTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Version, opt => opt.Ignore())
            .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailTemplateInput, MailTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Version, opt => opt.Ignore())
            .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifierId, opt => opt.Ignore());

        // MailSignature mappings
        CreateMap<MailSignature, MailSignatureDto>();
        CreateMap<CreateMailSignatureInput, MailSignature>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailSignatureInput, MailSignature>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDefault, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifierId, opt => opt.Ignore());

        // MailLabel mappings
        CreateMap<MailLabel, MailLabelDto>();
        CreateMap<CreateMailLabelInput, MailLabel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsSystemLabel, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailLabelInput, MailLabel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsSystemLabel, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifierId, opt => opt.Ignore());

        // MailRule mappings
        CreateMap<MailRule, MailRuleDto>();
        CreateMap<RuleCondition, RuleConditionDto>();
        CreateMap<RuleAction, RuleActionDto>();
        CreateMap<CreateMailRuleInput, MailRule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExecutionCount, opt => opt.Ignore())
            .ForMember(dest => dest.LastExecutedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailRuleInput, MailRule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExecutionCount, opt => opt.Ignore())
            .ForMember(dest => dest.LastExecutedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifierId, opt => opt.Ignore());

        // MailAttachment mappings
        CreateMap<MailAttachment, MailAttachmentDto>();
        CreateMap<CreateMailAttachmentInput, MailAttachment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DownloadCount, opt => opt.Ignore())
            .ForMember(dest => dest.LastDownloadedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());

        // MailContact mappings
        CreateMap<MailContact, MailContactDto>();
        CreateMap<CreateMailContactInput, MailContact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LastContactedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MailCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailContactInput, MailContact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LastContactedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MailCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.LastModificationTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifierId, opt => opt.Ignore());

        // MailBusinessRelation mappings
        CreateMap<MailBusinessRelation, MailBusinessRelationDto>();
        CreateMap<CreateMailBusinessRelationInput, MailBusinessRelation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());
        CreateMap<UpdateMailBusinessRelationInput, MailBusinessRelation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore());

        // WebhookSubscription mappings
        CreateMap<WebhookSubscription, WebhookSubscriptionDto>();
    }
}
