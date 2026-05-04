using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Enums;
using DomainMailAccount = YANEDGE.EmailManagement.Domain.MailAccount.MailAccount;

namespace YANEDGE.EmailManagement.Application.Tests.Integration;

/// <summary>
/// 集成测试：测试完整的邮件发送工作流
/// </summary>
public class MailSendWorkflowIntegrationTests : EmailManagementApplicationTestBase
{
    private readonly IRepository<MailTemplate, Guid> _templateRepository;
    private readonly IRepository<DomainMailAccount, Guid> _mailAccountRepository;
    private readonly IRepository<MailSendTask, Guid> _sendTaskRepository;

    public MailSendWorkflowIntegrationTests()
    {
        _templateRepository = GetRequiredService<IRepository<MailTemplate, Guid>>();
        _mailAccountRepository = GetRequiredService<IRepository<DomainMailAccount, Guid>>();
        _sendTaskRepository = GetRequiredService<IRepository<MailSendTask, Guid>>();
    }

    [Fact]
    public async Task Should_Create_Complete_Mail_Send_Workflow()
    {
        // 1. Create a mail account
        var mailAccount = new DomainMailAccount(
            Guid.NewGuid(),
            "Integration Test Account",
            "integration@test.com",
            "Integration Tester",
            MailAccountType.Service,
            MailProtocol.IMAP,
            "imap.test.com",
            993,
            true,
            MailProtocol.IMAP,
            "smtp.test.com",
            587,
            true,
            "testuser",
            "encrypted_password",
            Guid.NewGuid()
        );

        await _mailAccountRepository.InsertAsync(mailAccount);

        // 2. Create a template
        var template = new MailTemplate(
            Guid.NewGuid(),
            "INTEGRATION_TEST",
            "Integration Test Template",
            "Test: {{Subject}}",
            "<p>Hello {{Name}}, this is a test.</p>",
            "en",
            "Testing",
            false,
            "Template for integration testing"
        );

        await _templateRepository.InsertAsync(template);

        // 3. Create a send task using the template
        var sendTask = new MailSendTask(
            Guid.NewGuid(),
            mailAccount.Id,
            "Integration Test Email",
            Guid.NewGuid(), // userId
            false
        );

        sendTask.SetBody("<p>Test body</p>", "Test body");
        sendTask.SetTemplate(template.Id);

        await _sendTaskRepository.InsertAsync(sendTask);

        // Assert
        var savedAccount = await _mailAccountRepository.GetAsync(mailAccount.Id);
        savedAccount.ShouldNotBeNull();
        savedAccount.EmailAddress.ShouldBe("integration@test.com");

        var savedTemplate = await _templateRepository.GetAsync(template.Id);
        savedTemplate.ShouldNotBeNull();
        savedTemplate.Code.ShouldBe("INTEGRATION_TEST");

        var savedTask = await _sendTaskRepository.GetAsync(sendTask.Id);
        savedTask.ShouldNotBeNull();
        savedTask.TemplateId.ShouldBe(template.Id);
        savedTask.MailAccountId.ShouldBe(mailAccount.Id);
        savedTask.Status.ShouldBe(SendTaskStatus.Draft);
    }

    [Fact]
    public async Task Should_Create_Template_And_Activate_It()
    {
        // Arrange
        var template = new MailTemplate(
            Guid.NewGuid(),
            "WORKFLOW_TEST",
            "Workflow Test Template",
            "Subject: {{Topic}}",
            "<p>Content: {{Body}}</p>",
            "en",
            "Workflow",
            false,
            "Testing workflow"
        );

        // Act
        await _templateRepository.InsertAsync(template);
        template.Activate();
        await _templateRepository.UpdateAsync(template);

        // Assert
        var savedTemplate = await _templateRepository.GetAsync(template.Id);
        savedTemplate.Status.ShouldBe(TemplateStatus.Active);
    }
}
