using System;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Signature;

public class GetSignatureListInput : PagedAndSortedResultRequestDto
{
    public SignatureScope? Scope { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? OwnerOrganizationId { get; set; }

    public bool? IsActive { get; set; }
}
