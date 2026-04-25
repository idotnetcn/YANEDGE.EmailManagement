using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace YANEDGE.EmailManagement.Emails;

public class EmailManager : DomainService
{
    private readonly IEmailRepository _emailRepository;
    private readonly IGuidGenerator _guidGenerator;

    public EmailManager(IEmailRepository emailRepository, IGuidGenerator guidGenerator)
    {
        _emailRepository = emailRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task<Email> CreateAsync(
        string subject,
        string body,
        string fromAddress,
        bool isBodyHtml = true,
        EmailPriority priority = EmailPriority.Normal,
        string fromDisplayName = "")
    {
        var email = new Email(
            _guidGenerator.Create(),
            subject,
            body,
            fromAddress,
            isBodyHtml,
            priority,
            fromDisplayName
        );
        return await _emailRepository.InsertAsync(email);
    }

    public async Task<Email> QueueAsync(Guid emailId, DateTime? scheduledAt = null)
    {
        var email = await _emailRepository.GetAsync(emailId);
        email.Queue(scheduledAt);
        return await _emailRepository.UpdateAsync(email);
    }

    public async Task<Email> MarkAsSentAsync(Guid emailId)
    {
        var email = await _emailRepository.GetAsync(emailId);
        email.MarkAsSent();
        return await _emailRepository.UpdateAsync(email);
    }

    public async Task<Email> MarkAsFailedAsync(Guid emailId, string errorMessage)
    {
        var email = await _emailRepository.GetAsync(emailId);
        email.MarkAsFailed(errorMessage);
        return await _emailRepository.UpdateAsync(email);
    }
}
