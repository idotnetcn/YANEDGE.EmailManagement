using System;

namespace YANEDGE.EmailManagement.MailThreads;

public class AssignThreadInput
{
    public Guid? ToUserId { get; set; }
    public Guid? ToRoleId { get; set; }
    public Guid? ToOrganizationUnitId { get; set; }
    public string? Comment { get; set; }
}
