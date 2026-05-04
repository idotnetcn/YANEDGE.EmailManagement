using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Volo.Abp.Uow;
using Xunit;
using YANEDGE.EmailManagement.Application.BackgroundJobs;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.Tests.BackgroundJobs;

public class SendTaskProcessorJobTests : EmailManagementApplicationTestBase
{
    private readonly SendTaskProcessorJob _sendTaskProcessorJob;
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ILogger<SendTaskProcessorJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public SendTaskProcessorJobTests()
    {
        _sendTaskRepository = Substitute.For<IMailSendTaskRepository>();
        _mailSendService = Substitute.For<IMailSendService>();
        _logger = Substitute.For<ILogger<SendTaskProcessorJob>>();
        _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();

        _sendTaskProcessorJob = new SendTaskProcessorJob(
            _sendTaskRepository,
            _mailSendService,
            _logger,
            _unitOfWorkManager);
    }

    [Fact]
    public async Task Should_Execute_Without_Error_When_No_Pending_Tasks()
    {
        // Arrange
        _sendTaskRepository.GetPendingSendTasksAsync()
            .Returns(Task.FromResult(new List<MailSendTask>()));

        // Act
        await _sendTaskProcessorJob.ExecuteAsync();

        // Assert
        await _sendTaskRepository.Received(1).GetPendingSendTasksAsync();
        await _mailSendService.DidNotReceive().ExecuteSendTaskAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Should_Process_All_Pending_Tasks()
    {
        // Arrange
        var task1 = CreateTestSendTask();
        var task2 = CreateTestSendTask();
        var tasks = new List<MailSendTask> { task1, task2 };

        _sendTaskRepository.GetPendingSendTasksAsync()
            .Returns(Task.FromResult(tasks));

        _mailSendService.ExecuteSendTaskAsync(Arg.Any<Guid>())
            .Returns(Task.CompletedTask);

        // Act
        await _sendTaskProcessorJob.ExecuteAsync();

        // Assert
        await _sendTaskRepository.Received(1).GetPendingSendTasksAsync();
        await _mailSendService.Received(1).ExecuteSendTaskAsync(task1.Id);
        await _mailSendService.Received(1).ExecuteSendTaskAsync(task2.Id);
    }

    [Fact]
    public async Task Should_Continue_Processing_When_One_Task_Fails()
    {
        // Arrange
        var task1 = CreateTestSendTask();
        var task2 = CreateTestSendTask();
        var tasks = new List<MailSendTask> { task1, task2 };

        _sendTaskRepository.GetPendingSendTasksAsync()
            .Returns(Task.FromResult(tasks));

        _mailSendService.ExecuteSendTaskAsync(task1.Id)
            .Returns(Task.FromException(new Exception("Send failed")));

        _mailSendService.ExecuteSendTaskAsync(task2.Id)
            .Returns(Task.CompletedTask);

        // Act
        await _sendTaskProcessorJob.ExecuteAsync();

        // Assert - should still process the second task
        await _sendTaskRepository.Received(1).GetPendingSendTasksAsync();
        await _mailSendService.Received(1).ExecuteSendTaskAsync(task1.Id);
        await _mailSendService.Received(1).ExecuteSendTaskAsync(task2.Id);
    }

    [Fact]
    public async Task Should_Limit_Batch_Size_When_Too_Many_Tasks()
    {
        // Arrange - Create more than MaxBatchSize (50) tasks
        var tasks = Enumerable.Range(0, 60)
            .Select(_ => CreateTestSendTask())
            .ToList();

        _sendTaskRepository.GetPendingSendTasksAsync()
            .Returns(Task.FromResult(tasks));

        _mailSendService.ExecuteSendTaskAsync(Arg.Any<Guid>())
            .Returns(Task.CompletedTask);

        // Act
        await _sendTaskProcessorJob.ExecuteAsync();

        // Assert - should only process first 50 tasks
        await _sendTaskRepository.Received(1).GetPendingSendTasksAsync();
        await _mailSendService.Received(50).ExecuteSendTaskAsync(Arg.Any<Guid>());
    }

    private MailSendTask CreateTestSendTask()
    {
        var task = new MailSendTask(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Subject",
            Guid.NewGuid(),
            false
        );

        task.SetBody("<p>Test body</p>", "Test body");
        task.SetRecipients("test@example.com", null, null);

        return task;
    }
}
