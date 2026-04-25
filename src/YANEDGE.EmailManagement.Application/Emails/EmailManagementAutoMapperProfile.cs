using AutoMapper;

namespace YANEDGE.EmailManagement.Emails;

public class EmailManagementAutoMapperProfile : Profile
{
    public EmailManagementAutoMapperProfile()
    {
        CreateMap<Email, EmailDto>();
        CreateMap<EmailRecipient, EmailRecipientDto>();
        CreateMap<EmailAttachment, EmailAttachmentDto>();
        CreateMap<EmailTemplate, EmailTemplateDto>();
    }
}
