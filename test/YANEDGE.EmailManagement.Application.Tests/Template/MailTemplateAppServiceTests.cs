using System;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Template;
using YANEDGE.EmailManagement.Enums;
using YANEDGE.EmailManagement.Application.Template;

namespace YANEDGE.EmailManagement.Application.Tests.Template;

public class MailTemplateAppServiceTests : EmailManagementApplicationTestBase
{
    private readonly IMailTemplateAppService _mailTemplateAppService;
    private readonly IRepository<MailTemplate, Guid> _templateRepository;

    public MailTemplateAppServiceTests()
    {
        _mailTemplateAppService = GetRequiredService<IMailTemplateAppService>();
        _templateRepository = GetRequiredService<IRepository<MailTemplate, Guid>>();
    }

    [Fact]
    public async Task Should_Create_Template()
    {
        // Arrange
        var input = new CreateMailTemplateInput
        {
            Code = "TEST_WELCOME",
            Name = "Welcome Email",
            SubjectTemplate = "Welcome {{UserName}}",
            BodyTemplate = "<p>Hello {{UserName}}, welcome!</p>",
            Language = "en",
            Category = "Onboarding",
            RequiresApproval = false,
            Description = "Welcome template for new users"
        };

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _mailTemplateAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(input.Code);
            result.Name.ShouldBe(input.Name);
            result.SubjectTemplate.ShouldBe(input.SubjectTemplate);
            result.BodyTemplate.ShouldBe(input.BodyTemplate);
            result.Status.ShouldBe(TemplateStatus.Draft);
        });
    }

    [Fact]
    public async Task Should_Get_Template_By_Id()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _mailTemplateAppService.GetAsync(template.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(template.Id);
            result.Code.ShouldBe(template.Code);
            result.Name.ShouldBe(template.Name);
        });
    }

    [Fact]
    public async Task Should_Get_Template_By_Code()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _mailTemplateAppService.GetByCodeAsync(template.Code);

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(template.Code);
            result.Name.ShouldBe(template.Name);
        });
    }

    [Fact]
    public async Task Should_Update_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();
        var input = new UpdateMailTemplateInput
        {
            Name = "Updated Template Name",
            SubjectTemplate = "Updated Subject {{Variable}}",
            BodyTemplate = "<p>Updated body {{Content}}</p>",
            Category = "Updated Category",
            Description = "Updated description"
        };

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _mailTemplateAppService.UpdateAsync(template.Id, input);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(input.Name);
            result.SubjectTemplate.ShouldBe(input.SubjectTemplate);
            result.BodyTemplate.ShouldBe(input.BodyTemplate);
            result.Category.ShouldBe(input.Category);
            result.Description.ShouldBe(input.Description);
        });
    }

    [Fact]
    public async Task Should_Delete_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _mailTemplateAppService.DeleteAsync(template.Id);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var deletedTemplate = await _templateRepository.FindAsync(template.Id);
            deletedTemplate.ShouldBeNull();
        });
    }

    [Fact]
    public async Task Should_Activate_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _mailTemplateAppService.ActivateAsync(template.Id);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedTemplate = await _templateRepository.GetAsync(template.Id);
            updatedTemplate.Status.ShouldBe(TemplateStatus.Active);
        });
    }

    [Fact]
    public async Task Should_Get_List_With_Filters()
    {
        // Arrange
        await CreateTestTemplateAsync("TEMPLATE_1", "Test Template 1", "Category1");
        await CreateTestTemplateAsync("TEMPLATE_2", "Test Template 2", "Category2");
        await CreateTestTemplateAsync("TEMPLATE_3", "Another Template", "Category1");

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            var input = new GetTemplateListInput
            {
                Category = "Category1",
                MaxResultCount = 10
            };

            var result = await _mailTemplateAppService.GetListAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            result.Items.ShouldAllBe(x => x.Category == "Category1");
        });
    }

    private async Task<MailTemplate> CreateTestTemplateAsync(
        string code = "TEST_TEMPLATE",
        string name = "Test Template",
        string category = "Test Category")
    {
        MailTemplate? template = null;

        await WithUnitOfWorkAsync(async () =>
        {
            template = new MailTemplate(
                Guid.NewGuid(),
                code,
                name,
                "Test Subject {{Variable}}",
                "<p>Test Body {{Content}}</p>",
                "zh-CN",
                category,
                false,
                "Test Description"
            );

            await _templateRepository.InsertAsync(template);
        });

        return template!;
    }
}
