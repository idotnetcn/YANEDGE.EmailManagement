using System;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Signature;

public class UpdateMailSignatureInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    public string? PlainTextContent { get; set; }
}
