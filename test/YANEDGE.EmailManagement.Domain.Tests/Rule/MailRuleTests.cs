using Shouldly;
using Xunit;
using YANEDGE.EmailManagement.Domain.Rule;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Tests.Rule;

public class MailRuleTests : EmailManagementDomainTestBase
{
    [Fact]
    public void Should_Create_MailRule_With_Valid_Data()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Auto Label Important Emails";
        var priority = 100;
        var description = "Automatically label emails from important senders";

        // Act
        var rule = new MailRule(id, name, priority, description);

        // Assert
        rule.ShouldNotBeNull();
        rule.Id.ShouldBe(id);
        rule.Name.ShouldBe(name);
        rule.Priority.ShouldBe(priority);
        rule.Description.ShouldBe(description);
        rule.IsActive.ShouldBeTrue();
        rule.ApplicableMailAccountIds.ShouldNotBeNull();
        rule.ApplicableMailAccountIds.ShouldBeEmpty();
        rule.Conditions.ShouldNotBeNull();
        rule.Conditions.ShouldBeEmpty();
        rule.Actions.ShouldNotBeNull();
        rule.Actions.ShouldBeEmpty();
        rule.ExecutionCount.ShouldBe(0);
        rule.LastExecutedAt.ShouldBeNull();
    }

    [Fact]
    public void Should_Update_Rule()
    {
        // Arrange
        var rule = CreateTestRule();
        var newName = "Updated Rule Name";
        var newPriority = 200;
        var newDescription = "Updated description";

        // Act
        rule.Update(newName, newPriority, newDescription);

        // Assert
        rule.Name.ShouldBe(newName);
        rule.Priority.ShouldBe(newPriority);
        rule.Description.ShouldBe(newDescription);
    }

    [Fact]
    public void Should_Activate_Rule()
    {
        // Arrange
        var rule = CreateTestRule();
        rule.Deactivate();

        // Act
        rule.Activate();

        // Assert
        rule.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Should_Deactivate_Rule()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.Deactivate();

        // Assert
        rule.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Should_Add_Condition()
    {
        // Arrange
        var rule = CreateTestRule();
        var conditionType = RuleConditionType.SenderAddress;
        var value = "important@example.com";

        // Act
        rule.AddCondition(conditionType, value);

        // Assert
        rule.Conditions.ShouldNotBeEmpty();
        rule.Conditions.Count.ShouldBe(1);
        rule.Conditions[0].ConditionType.ShouldBe(conditionType);
        rule.Conditions[0].Value.ShouldBe(value);
    }

    [Fact]
    public void Should_Add_Multiple_Conditions()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.AddCondition(RuleConditionType.SenderAddress, "sender@example.com");
        rule.AddCondition(RuleConditionType.SubjectContains, "urgent");
        rule.AddCondition(RuleConditionType.BodyContains, "important");

        // Assert
        rule.Conditions.Count.ShouldBe(3);
    }

    [Fact]
    public void Should_Clear_Conditions()
    {
        // Arrange
        var rule = CreateTestRule();
        rule.AddCondition(RuleConditionType.SenderAddress, "test@example.com");
        rule.AddCondition(RuleConditionType.SubjectContains, "test");

        // Act
        rule.ClearConditions();

        // Assert
        rule.Conditions.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Add_Action()
    {
        // Arrange
        var rule = CreateTestRule();
        var actionType = RuleActionType.AddLabel;
        var parameters = "{\"labelId\":\"123\"}";

        // Act
        rule.AddAction(actionType, parameters);

        // Assert
        rule.Actions.ShouldNotBeEmpty();
        rule.Actions.Count.ShouldBe(1);
        rule.Actions[0].ActionType.ShouldBe(actionType);
        rule.Actions[0].Parameters.ShouldBe(parameters);
    }

    [Fact]
    public void Should_Add_Multiple_Actions()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.AddAction(RuleActionType.AddLabel, "{\"labelId\":\"1\"}");
        rule.AddAction(RuleActionType.MarkImportant, null);
        rule.AddAction(RuleActionType.AutoAssign, "{\"to\":\"forward@example.com\"}");

        // Assert
        rule.Actions.Count.ShouldBe(3);
    }

    [Fact]
    public void Should_Clear_Actions()
    {
        // Arrange
        var rule = CreateTestRule();
        rule.AddAction(RuleActionType.AddLabel, "{\"labelId\":\"1\"}");
        rule.AddAction(RuleActionType.MarkImportant, null);

        // Act
        rule.ClearActions();

        // Assert
        rule.Actions.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Record_Execution()
    {
        // Arrange
        var rule = CreateTestRule();
        var initialCount = rule.ExecutionCount;
        var beforeExecution = DateTime.UtcNow;

        // Act
        rule.RecordExecution();
        var afterExecution = DateTime.UtcNow;

        // Assert
        rule.ExecutionCount.ShouldBe(initialCount + 1);
        rule.LastExecutedAt.ShouldNotBeNull();
        rule.LastExecutedAt.Value.ShouldBeGreaterThanOrEqualTo(beforeExecution);
        rule.LastExecutedAt.Value.ShouldBeLessThanOrEqualTo(afterExecution);
    }

    [Fact]
    public void Should_Increment_Execution_Count_On_Multiple_Executions()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.RecordExecution();
        rule.RecordExecution();
        rule.RecordExecution();

        // Assert
        rule.ExecutionCount.ShouldBe(3);
    }

    [Fact]
    public void Should_Set_Applicable_Mail_Accounts()
    {
        // Arrange
        var rule = CreateTestRule();
        var mailAccountIds = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        // Act
        rule.SetApplicableMailAccounts(mailAccountIds);

        // Assert
        rule.ApplicableMailAccountIds.ShouldNotBeEmpty();
        rule.ApplicableMailAccountIds.Count.ShouldBe(3);
        rule.ApplicableMailAccountIds.ShouldBe(mailAccountIds);
    }

    [Fact]
    public void Should_Handle_Null_Mail_Account_Ids()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.SetApplicableMailAccounts(null!);

        // Assert
        rule.ApplicableMailAccountIds.ShouldNotBeNull();
        rule.ApplicableMailAccountIds.ShouldBeEmpty();
    }

    private MailRule CreateTestRule()
    {
        return new MailRule(
            Guid.NewGuid(),
            "Test Rule",
            50,
            "Test rule description"
        );
    }
}
