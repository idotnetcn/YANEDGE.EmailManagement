using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YANEDGE.EmailManagement.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachmentAccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentAccessLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    IncomingProtocol = table.Column<int>(type: "integer", nullable: false),
                    IncomingHost = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IncomingPort = table.Column<int>(type: "integer", nullable: false),
                    IncomingSslEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OutgoingProtocol = table.Column<int>(type: "integer", nullable: false),
                    OutgoingHost = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OutgoingPort = table.Column<int>(type: "integer", nullable: false),
                    OutgoingSslEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EncryptedPassword = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SyncEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    SendEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSendAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HealthStatus = table.Column<int>(type: "integer", nullable: false),
                    OwnerOrgId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessSnapshot = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContentId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsInline = table.Column<bool>(type: "boolean", nullable: false),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false),
                    LastDownloadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsScanned = table.Column<bool>(type: "boolean", nullable: false),
                    IsSafe = table.Column<bool>(type: "boolean", nullable: false),
                    ScanResult = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailBusinessRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessObjectType = table.Column<int>(type: "integer", nullable: false),
                    BusinessObjectId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BusinessObjectName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BusinessObjectCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    RelationSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalSystem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailBusinessRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    SupplierId = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LastContactedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MailCount = table.Column<int>(type: "integer", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailLabels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystemLabel = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLabels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailMessageLabels",
                columns: table => new
                {
                    MailMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessageLabels", x => new { x.MailMessageId, x.LabelId });
                });

            migrationBuilder.CreateTable(
                name: "MailMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternetMessageId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    FromAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FromDisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReceivedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasAttachment = table.Column<bool>(type: "boolean", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    Importance = table.Column<int>(type: "integer", nullable: false),
                    SanitizedHtmlBody = table.Column<string>(type: "text", nullable: true),
                    TextBody = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    ApplicableMailAccountIds = table.Column<string>(type: "jsonb", nullable: false),
                    Conditions = table.Column<string>(type: "jsonb", nullable: false),
                    Actions = table.Column<string>(type: "jsonb", nullable: false),
                    ExecutionCount = table.Column<int>(type: "integer", nullable: false),
                    LastExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailSendTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BodyHtml = table.Column<string>(type: "text", nullable: true),
                    BodyText = table.Column<string>(type: "text", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    SignatureId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    NeedApproval = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uuid", nullable: true),
                    ScheduledSendTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualSentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    MaxRetryCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ExternalBizRef = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSendTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailSignatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PlainTextContent = table.Column<string>(type: "text", nullable: true),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailSignatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubjectTemplate = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BodyTemplate = table.Column<string>(type: "text", nullable: false),
                    PlainTextTemplate = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NormalizedSubject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentAssigneeType = table.Column<int>(type: "integer", nullable: true),
                    CurrentAssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    LatestMessageTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MessageCount = table.Column<int>(type: "integer", nullable: false),
                    HasAttachment = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MailMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsMatched = table.Column<bool>(type: "boolean", nullable: false),
                    ExecutionResult = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExecutionTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreadAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromAssigneeType = table.Column<int>(type: "integer", nullable: true),
                    FromAssigneeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToAssigneeType = table.Column<int>(type: "integer", nullable: false),
                    ToAssigneeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadAssignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAccessLogs_AttachmentId",
                table: "AttachmentAccessLogs",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAccessLogs_CreationTime",
                table: "AttachmentAccessLogs",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentAccessLogs_UserId",
                table: "AttachmentAccessLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalNotes_CreatedByUserId",
                table: "InternalNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalNotes_IsPinned",
                table: "InternalNotes",
                column: "IsPinned");

            migrationBuilder.CreateIndex(
                name: "IX_InternalNotes_ThreadId",
                table: "InternalNotes",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MailAccounts_EmailAddress",
                table: "MailAccounts",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_MailApprovals_BusinessType_BusinessId",
                table: "MailApprovals",
                columns: new[] { "BusinessType", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_MailApprovals_CurrentApproverId",
                table: "MailApprovals",
                column: "CurrentApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_MailApprovals_Status",
                table: "MailApprovals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MailAttachments_FileHash",
                table: "MailAttachments",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_MailAttachments_IsScanned",
                table: "MailAttachments",
                column: "IsScanned");

            migrationBuilder.CreateIndex(
                name: "IX_MailAttachments_IsSensitive",
                table: "MailAttachments",
                column: "IsSensitive");

            migrationBuilder.CreateIndex(
                name: "IX_MailAttachments_MailMessageId",
                table: "MailAttachments",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBusinessRelations_BusinessObjectType_BusinessObjectId",
                table: "MailBusinessRelations",
                columns: new[] { "BusinessObjectType", "BusinessObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_MailBusinessRelations_IsPrimary",
                table: "MailBusinessRelations",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_MailBusinessRelations_MailMessageId",
                table: "MailBusinessRelations",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailBusinessRelations_ThreadId",
                table: "MailBusinessRelations",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MailContacts_CustomerId",
                table: "MailContacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MailContacts_EmailAddress",
                table: "MailContacts",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_MailContacts_ExternalId",
                table: "MailContacts",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_MailContacts_IsActive",
                table: "MailContacts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MailContacts_SupplierId",
                table: "MailContacts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_MailLabels_IsSystemLabel",
                table: "MailLabels",
                column: "IsSystemLabel");

            migrationBuilder.CreateIndex(
                name: "IX_MailLabels_Name",
                table: "MailLabels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MailLabels_OwnerOrganizationId",
                table: "MailLabels",
                column: "OwnerOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MailLabels_OwnerUserId",
                table: "MailLabels",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageLabels_LabelId",
                table: "MailMessageLabels",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageLabels_MailMessageId",
                table: "MailMessageLabels",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_InternetMessageId",
                table: "MailMessages",
                column: "InternetMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_MailAccountId",
                table: "MailMessages",
                column: "MailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_ReceivedTime",
                table: "MailMessages",
                column: "ReceivedTime");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_ThreadId",
                table: "MailMessages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRules_IsActive",
                table: "MailRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MailRules_Priority",
                table: "MailRules",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_CreatedByUserId",
                table: "MailSendTasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_ExternalBizRef",
                table: "MailSendTasks",
                column: "ExternalBizRef");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_IdempotencyKey",
                table: "MailSendTasks",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_MailAccountId",
                table: "MailSendTasks",
                column: "MailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_Status",
                table: "MailSendTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MailSendTasks_ThreadId",
                table: "MailSendTasks",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_MailSignatures_IsDefault",
                table: "MailSignatures",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_MailSignatures_OwnerOrganizationId",
                table: "MailSignatures",
                column: "OwnerOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MailSignatures_OwnerUserId",
                table: "MailSignatures",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MailSignatures_Scope",
                table: "MailSignatures",
                column: "Scope");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_Category",
                table: "MailTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_Code",
                table: "MailTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_IsDefault",
                table: "MailTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_MailTemplates_Status",
                table: "MailTemplates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MailThreads_CurrentAssigneeType_CurrentAssigneeId",
                table: "MailThreads",
                columns: new[] { "CurrentAssigneeType", "CurrentAssigneeId" });

            migrationBuilder.CreateIndex(
                name: "IX_MailThreads_LatestMessageTime",
                table: "MailThreads",
                column: "LatestMessageTime");

            migrationBuilder.CreateIndex(
                name: "IX_MailThreads_MailAccountId",
                table: "MailThreads",
                column: "MailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MailThreads_Status",
                table: "MailThreads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_CreationTime",
                table: "RuleExecutionLogs",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_MailMessageId",
                table: "RuleExecutionLogs",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_RuleId",
                table: "RuleExecutionLogs",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleExecutionLogs_ThreadId",
                table: "RuleExecutionLogs",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadAssignments_AssignedAt",
                table: "ThreadAssignments",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadAssignments_ThreadId",
                table: "ThreadAssignments",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadAssignments_ToAssigneeType_ToAssigneeId",
                table: "ThreadAssignments",
                columns: new[] { "ToAssigneeType", "ToAssigneeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentAccessLogs");

            migrationBuilder.DropTable(
                name: "InternalNotes");

            migrationBuilder.DropTable(
                name: "MailAccounts");

            migrationBuilder.DropTable(
                name: "MailApprovals");

            migrationBuilder.DropTable(
                name: "MailAttachments");

            migrationBuilder.DropTable(
                name: "MailBusinessRelations");

            migrationBuilder.DropTable(
                name: "MailContacts");

            migrationBuilder.DropTable(
                name: "MailLabels");

            migrationBuilder.DropTable(
                name: "MailMessageLabels");

            migrationBuilder.DropTable(
                name: "MailMessages");

            migrationBuilder.DropTable(
                name: "MailRules");

            migrationBuilder.DropTable(
                name: "MailSendTasks");

            migrationBuilder.DropTable(
                name: "MailSignatures");

            migrationBuilder.DropTable(
                name: "MailTemplates");

            migrationBuilder.DropTable(
                name: "MailThreads");

            migrationBuilder.DropTable(
                name: "RuleExecutionLogs");

            migrationBuilder.DropTable(
                name: "ThreadAssignments");
        }
    }
}
