using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Templates;

public class MailTemplateVariable : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public string VariableCode { get; private set; } = null!;
    public string DisplayName { get; set; } = null!;
    public byte SourceType { get; set; }
    public string ValueType { get; set; } = null!;
    public string? Description { get; set; }
    public string? ExampleValue { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailTemplateVariable() { }

    public MailTemplateVariable(Guid id, string variableCode, string displayName, string valueType)
    {
        Id = id;
        VariableCode = variableCode;
        DisplayName = displayName;
        ValueType = valueType;
        CreationTime = DateTime.UtcNow;
    }
}
