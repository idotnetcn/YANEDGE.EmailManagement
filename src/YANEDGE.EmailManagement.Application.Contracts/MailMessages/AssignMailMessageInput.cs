using System;

namespace YANEDGE.EmailManagement.MailMessages;

public class AssignMailMessageInput
{
    public Guid? ToUserId { get; set; }
    public Guid? ToRoleId { get; set; }
    public Guid? ToOrganizationUnitId { get; set; }
    public string? Comment { get; set; }
}
