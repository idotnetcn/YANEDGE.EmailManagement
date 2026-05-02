using System;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Contact;

public class MailContactDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? CompanyName { get; set; }

    public string? JobTitle { get; set; }

    public string? CustomerId { get; set; }

    public string? SupplierId { get; set; }

    public string Source { get; set; } = null!;

    public string? ExternalId { get; set; }

    public DateTime? LastContactedAt { get; set; }

    public int MailCount { get; set; }

    public bool IsVerified { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }
}
