using System;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Signature;

public class MailSignatureDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? PlainTextContent { get; set; }

    public SignatureScope Scope { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

    public int SortOrder { get; set; }
}
