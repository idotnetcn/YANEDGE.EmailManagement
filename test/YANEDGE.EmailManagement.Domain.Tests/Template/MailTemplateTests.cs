using Shouldly;
using Xunit;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Tests.Template;

public class MailTemplateTests : EmailManagementDomainTestBase
{
    [Fact]
    public void Should_Create_MailTemplate_With_Valid_Data()
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = "WELCOME_EMAIL";
        var name = "Welcome Email Template";
        var subjectTemplate = "Welcome to {{AppName}}";
        var bodyTemplate = "<p>Hello {{UserName}}, welcome to our application!</p>";
        var language = "en";
        var category = "Marketing";
        var requiresApproval = true;
        var description = "Template for welcoming new users";

        // Act
        var template = new MailTemplate(
            id,
            code,
            name,
            subjectTemplate,
            bodyTemplate,
            language,
            category,
            requiresApproval,
            description
        );

        // Assert
        template.ShouldNotBeNull();
        template.Id.ShouldBe(id);
        template.Code.ShouldBe(code);
        template.Name.ShouldBe(name);
        template.SubjectTemplate.ShouldBe(subjectTemplate);
        template.BodyTemplate.ShouldBe(bodyTemplate);
        template.Language.ShouldBe(language);
        template.Category.ShouldBe(category);
        template.RequiresApproval.ShouldBe(requiresApproval);
        template.Description.ShouldBe(description);
        template.Status.ShouldBe(TemplateStatus.Draft);
        template.Version.ShouldBe(1);
        template.IsDefault.ShouldBeFalse();
        template.SortOrder.ShouldBe(0);
    }

    [Fact]
    public void Should_Update_Template_And_Increment_Version()
    {
        // Arrange
        var template = CreateTestTemplate();
        var initialVersion = template.Version;
        var newName = "Updated Template Name";
        var newSubject = "Updated Subject";
        var newBody = "<p>Updated body content</p>";
        var newCategory = "Updated Category";
        var newDescription = "Updated description";

        // Act
        template.Update(
            newName,
            newSubject,
            newBody,
            newCategory,
            null,
            newDescription
        );

        // Assert
        template.Name.ShouldBe(newName);
        template.SubjectTemplate.ShouldBe(newSubject);
        template.BodyTemplate.ShouldBe(newBody);
        template.Category.ShouldBe(newCategory);
        template.Description.ShouldBe(newDescription);
        template.Version.ShouldBe(initialVersion + 1);
    }

    [Fact]
    public void Should_Activate_Draft_Template()
    {
        // Arrange
        var template = CreateTestTemplate();

        // Act
        template.Activate();

        // Assert
        template.Status.ShouldBe(TemplateStatus.Active);
    }

    [Fact]
    public void Should_Throw_When_Activating_Already_Active_Template()
    {
        // Arrange
        var template = CreateTestTemplate();
        template.Activate();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => template.Activate())
            .Message.ShouldBe("Template is already active");
    }

    [Fact]
    public void Should_Deactivate_Active_Template()
    {
        // Arrange
        var template = CreateTestTemplate();
        template.Activate();

        // Act
        template.Deactivate();

        // Assert
        template.Status.ShouldBe(TemplateStatus.Inactive);
    }

    [Fact]
    public void Should_Throw_When_Deactivating_Non_Active_Template()
    {
        // Arrange
        var template = CreateTestTemplate();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => template.Deactivate())
            .Message.ShouldBe("Only active templates can be deactivated");
    }

    [Fact]
    public void Should_Archive_Template()
    {
        // Arrange
        var template = CreateTestTemplate();

        // Act
        template.Archive();

        // Assert
        template.Status.ShouldBe(TemplateStatus.Archived);
    }

    [Fact]
    public void Should_Throw_When_Archiving_Already_Archived_Template()
    {
        // Arrange
        var template = CreateTestTemplate();
        template.Archive();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => template.Archive())
            .Message.ShouldBe("Template is already archived");
    }

    [Fact]
    public void Should_Set_As_Default()
    {
        // Arrange
        var template = CreateTestTemplate();

        // Act
        template.SetAsDefault();

        // Assert
        template.IsDefault.ShouldBeTrue();
    }

    [Fact]
    public void Should_Unset_As_Default()
    {
        // Arrange
        var template = CreateTestTemplate();
        template.SetAsDefault();

        // Act
        template.UnsetAsDefault();

        // Assert
        template.IsDefault.ShouldBeFalse();
    }

    [Fact]
    public void Should_Set_Sort_Order()
    {
        // Arrange
        var template = CreateTestTemplate();
        var sortOrder = 10;

        // Act
        template.SetSortOrder(sortOrder);

        // Assert
        template.SortOrder.ShouldBe(sortOrder);
    }

    private MailTemplate CreateTestTemplate()
    {
        return new MailTemplate(
            Guid.NewGuid(),
            "TEST_TEMPLATE",
            "Test Template",
            "Test Subject {{Variable}}",
            "<p>Test Body {{Content}}</p>",
            "zh-CN",
            "Test Category",
            false,
            "Test Description"
        );
    }
}
