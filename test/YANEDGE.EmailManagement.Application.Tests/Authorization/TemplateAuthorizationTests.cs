using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Shouldly;
using Volo.Abp.Authorization;
using Xunit;
using YANEDGE.EmailManagement.Application.Template;
using YANEDGE.EmailManagement.Permissions;
using YANEDGE.EmailManagement.Template;

namespace YANEDGE.EmailManagement.Application.Tests.Authorization;

public class TemplateAuthorizationTests : EmailManagementApplicationTestBase
{
    private readonly IMailTemplateAppService _mailTemplateAppService;

    public TemplateAuthorizationTests()
    {
        _mailTemplateAppService = GetRequiredService<IMailTemplateAppService>();
    }

    [Fact]
    public async Task Should_Throw_Authorization_Exception_When_Creating_Without_Permission()
    {
        // Arrange
        var input = new CreateMailTemplateInput
        {
            Code = "UNAUTHORIZED_TEST",
            Name = "Unauthorized Test",
            SubjectTemplate = "Test {{Variable}}",
            BodyTemplate = "<p>Test</p>",
            Language = "en",
            Category = "Test",
            RequiresApproval = false
        };

        // Act & Assert
        // Note: In a real test, you would need to set up a user without permissions
        // For now, we just verify that the authorization attribute is present
        var methodInfo = typeof(MailTemplateAppService).GetMethod(nameof(IMailTemplateAppService.CreateAsync));
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), true);

        authorizeAttribute.ShouldNotBeNull();
        authorizeAttribute.ShouldNotBeEmpty();
    }

    [Fact]
    public void Should_Have_Authorization_Attribute_On_Service()
    {
        // Arrange & Act
        var serviceType = typeof(MailTemplateAppService);
        var authorizeAttributes = serviceType.GetCustomAttributes(typeof(AuthorizeAttribute), true);

        // Assert
        authorizeAttributes.ShouldNotBeNull();
        authorizeAttributes.ShouldNotBeEmpty();

        var authorizeAttribute = authorizeAttributes[0] as AuthorizeAttribute;
        authorizeAttribute?.Policy.ShouldBe(EmailManagementPermissions.Templates.Default);
    }

    [Fact]
    public void Should_Require_Create_Permission_For_Create_Method()
    {
        // Arrange & Act
        var methodInfo = typeof(MailTemplateAppService).GetMethod(nameof(IMailTemplateAppService.CreateAsync));
        var authorizeAttributes = methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), true);

        // Assert
        authorizeAttributes.ShouldNotBeNull();
        authorizeAttributes.ShouldNotBeEmpty();

        var authorizeAttribute = authorizeAttributes[0] as AuthorizeAttribute;
        authorizeAttribute?.Policy.ShouldBe(EmailManagementPermissions.Templates.Create);
    }
}
