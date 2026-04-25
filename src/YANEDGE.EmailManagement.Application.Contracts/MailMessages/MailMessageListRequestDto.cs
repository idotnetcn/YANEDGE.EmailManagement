using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailMessages;

public class MailMessageListRequestDto : PagedAndSortedResultRequestDto
{
    public Guid? MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public FolderType? FolderType { get; set; }
    public MailDirection? MailDirection { get; set; }
    public ProcessingStatus? ProcessingStatus { get; set; }
    public string? Filter { get; set; }
    public string? FromAddress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
