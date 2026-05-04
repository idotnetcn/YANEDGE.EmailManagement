using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Template;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.HttpApi.Tests.Controllers.Template;

public class MailTemplateControllerTests : EmailManagementHttpApiTestBase
{
    private readonly IRepository<MailTemplate, Guid> _templateRepository;

    public MailTemplateControllerTests()
    {
        _templateRepository = GetRequiredService<IRepository<MailTemplate, Guid>>();
    }

    [Fact]
    public async Task Should_Get_Template_By_Id()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        var response = await Client.GetAsync($"/api/mail-management/v1/templates/{template.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MailTemplateDto>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(template.Id);
        result.Code.ShouldBe(template.Code);
    }

    [Fact]
    public async Task Should_Get_Template_By_Code()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        var response = await Client.GetAsync($"/api/mail-management/v1/templates/by-code/{template.Code}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MailTemplateDto>();
        result.ShouldNotBeNull();
        result.Code.ShouldBe(template.Code);
    }

    [Fact]
    public async Task Should_Create_Template()
    {
        // Arrange
        var input = new CreateMailTemplateInput
        {
            Code = "API_TEST_TEMPLATE",
            Name = "API Test Template",
            SubjectTemplate = "Test Subject {{Variable}}",
            BodyTemplate = "<p>Test Body {{Content}}</p>",
            Language = "en",
            Category = "Test",
            RequiresApproval = false,
            Description = "API test template"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/mail-management/v1/templates", input);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MailTemplateDto>();
        result.ShouldNotBeNull();
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
    }

    [Fact]
    public async Task Should_Update_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();
        var input = new UpdateMailTemplateInput
        {
            Name = "Updated Template Name",
            SubjectTemplate = "Updated Subject",
            BodyTemplate = "<p>Updated Body</p>",
            Category = "Updated Category"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/mail-management/v1/templates/{template.Id}", input);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MailTemplateDto>();
        result.ShouldNotBeNull();
        result.Name.ShouldBe(input.Name);
    }

    [Fact]
    public async Task Should_Activate_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        var response = await Client.PostAsync($"/api/mail-management/v1/templates/{template.Id}/activate", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_Deactivate_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();
        await ActivateTemplateAsync(template.Id);

        // Act
        var response = await Client.PostAsync($"/api/mail-management/v1/templates/{template.Id}/deactivate", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_Delete_Template()
    {
        // Arrange
        var template = await CreateTestTemplateAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/mail-management/v1/templates/{template.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_Get_Template_List()
    {
        // Arrange
        await CreateTestTemplateAsync("TEMPLATE_1", "Template 1");
        await CreateTestTemplateAsync("TEMPLATE_2", "Template 2");

        // Act
        var response = await Client.GetAsync("/api/mail-management/v1/templates?MaxResultCount=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Volo.Abp.Application.Dtos.PagedResultDto<MailTemplateDto>>();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_Template()
    {
        // Act
        var response = await Client.GetAsync($"/api/mail-management/v1/templates/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task<MailTemplate> CreateTestTemplateAsync(
        string code = "TEST_TEMPLATE",
        string name = "Test Template")
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
                "Test Category",
                false,
                "Test Description"
            );

            await _templateRepository.InsertAsync(template);
        });

        return template!;
    }

    private async Task ActivateTemplateAsync(Guid templateId)
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var template = await _templateRepository.GetAsync(templateId);
            template.Activate();
            await _templateRepository.UpdateAsync(template);
        });
    }
}
