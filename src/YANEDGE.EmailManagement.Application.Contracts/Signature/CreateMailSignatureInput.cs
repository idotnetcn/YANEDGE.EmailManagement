using System;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Signature;

public class CreateMailSignatureInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    public string? PlainTextContent { get; set; }

    [Required]
    public SignatureScope Scope { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }

    public bool IsDefault { get; set; }
}
