namespace YANEDGE.EmailManagement.Enums;
public enum ProcessingStatus : byte
{
    PendingAssignment = 1,
    PendingProcess = 2,
    Processing = 3,
    PendingApproval = 4,
    Completed = 5,
    Archived = 6,
    Closed = 7
}
