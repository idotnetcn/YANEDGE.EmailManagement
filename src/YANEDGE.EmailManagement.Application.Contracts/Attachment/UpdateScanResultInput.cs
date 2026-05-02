using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Attachment;

public class UpdateScanResultInput
{
    [Required]
    public bool IsSafe { get; set; }

    [StringLength(1000)]
    public string? ScanResult { get; set; }
}
