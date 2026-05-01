using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Helpers;
using AcademicPlanner.Models;

namespace AcademicPlanner.Tests.Helpers;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("Term 1", true)]
    [InlineData("  Course A  ", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void IsRequired_ValidatesRequiredText(string value, bool expected)
    {
        bool result = ValidationHelper.IsRequired(value);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DatesAreValid_ReturnsTrue_WhenStartDateIsBeforeEndDate()
    {
        DateTime start = new(2026, 1, 1);
        DateTime end = new(2026, 6, 30);

        bool result = ValidationHelper.DatesAreValid(start, end);

        Assert.True(result);
    }

    [Fact]
    public void DatesAreValid_ReturnsTrue_WhenStartDateEqualsEndDate()
    {
        DateTime start = new(2026, 1, 1);
        DateTime end = new(2026, 1, 1);

        bool result = ValidationHelper.DatesAreValid(start, end);

        Assert.True(result);
    }

    [Fact]
    public void DatesAreValid_ReturnsFalse_WhenStartDateIsAfterEndDate()
    {
        DateTime start = new(2026, 6, 30);
        DateTime end = new(2026, 1, 1);

        bool result = ValidationHelper.DatesAreValid(start, end);

        Assert.False(result);
    }

    [Fact]
    public void DateRangeIsWithinParent_ReturnsTrue_WhenChildRangeIsInsideParentRange()
    {
        bool result = ValidationHelper.DateRangeIsWithinParent(
            childStart: new DateTime(2026, 2, 1),
            childEnd: new DateTime(2026, 3, 1),
            parentStart: new DateTime(2026, 1, 1),
            parentEnd: new DateTime(2026, 6, 30));

        Assert.True(result);
    }

    [Fact]
    public void DateRangeIsWithinParent_ReturnsTrue_WhenChildRangeMatchesParentRange()
    {
        bool result = ValidationHelper.DateRangeIsWithinParent(
            childStart: new DateTime(2026, 1, 1),
            childEnd: new DateTime(2026, 6, 30),
            parentStart: new DateTime(2026, 1, 1),
            parentEnd: new DateTime(2026, 6, 30));

        Assert.True(result);
    }

    [Fact]
    public void DateRangeIsWithinParent_ReturnsFalse_WhenChildStartsBeforeParent()
    {
        bool result = ValidationHelper.DateRangeIsWithinParent(
            childStart: new DateTime(2025, 12, 31),
            childEnd: new DateTime(2026, 3, 1),
            parentStart: new DateTime(2026, 1, 1),
            parentEnd: new DateTime(2026, 6, 30));

        Assert.False(result);
    }

    [Fact]
    public void DateRangeIsWithinParent_ReturnsFalse_WhenChildEndsAfterParent()
    {
        bool result = ValidationHelper.DateRangeIsWithinParent(
            childStart: new DateTime(2026, 2, 1),
            childEnd: new DateTime(2026, 7, 1),
            parentStart: new DateTime(2026, 1, 1),
            parentEnd: new DateTime(2026, 6, 30));

        Assert.False(result);
    }

    [Theory]
    [InlineData("instructor@example.com", true)]
    [InlineData("first.last@example.edu", true)]
    [InlineData("not-an-email", false)]
    [InlineData("missing-domain@", false)]
    [InlineData("@missing-name.com", false)]
    [InlineData("", false)]
    public void IsValidEmail_ValidatesEmailFormat(string email, bool expected)
    {
        bool result = ValidationHelper.IsValidEmail(email);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("(555) 123-4567", true)]
    [InlineData("555-123-4567", true)]
    [InlineData("+1 555 123 4567", true)]
    [InlineData("5551234567", true)]
    [InlineData("abc-def-ghij", false)]
    [InlineData("123", false)]
    [InlineData("", false)]
    public void IsValidPhone_ValidatesPhoneFormat(string phone, bool expected)
    {
        bool result = ValidationHelper.IsValidPhone(phone);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanAddAnotherAssessment_ReturnsTrue_WhenCourseHasFewerThanTwoAssessments()
    {
        var assessments = new List<Assessment>
        {
            new() { Id = 1, Type = "Objective" }
        };

        bool result = ValidationHelper.CanAddAnotherAssessment(assessments);

        Assert.True(result);
    }

    [Fact]
    public void CanAddAnotherAssessment_ReturnsFalse_WhenCourseAlreadyHasTwoAssessments()
    {
        var assessments = new List<Assessment>
        {
            new() { Id = 1, Type = "Objective" },
            new() { Id = 2, Type = "Performance" }
        };

        bool result = ValidationHelper.CanAddAnotherAssessment(assessments);

        Assert.False(result);
    }

    [Fact]
    public void HasDuplicateAssessmentType_ReturnsTrue_WhenAssessmentTypeAlreadyExists()
    {
        var assessments = new List<Assessment>
        {
            new() { Id = 1, Type = "Objective" }
        };

        bool result = ValidationHelper.HasDuplicateAssessmentType(
            assessments,
            "Objective");

        Assert.True(result);
    }

    [Fact]
    public void HasDuplicateAssessmentType_ReturnsFalse_WhenEditingSameAssessment()
    {
        var assessments = new List<Assessment>
        {
            new() { Id = 1, Type = "Objective" }
        };

        bool result = ValidationHelper.HasDuplicateAssessmentType(
            assessments,
            "Objective",
            currentAssessmentId: 1);

        Assert.False(result);
    }

    [Fact]
    public void HasDuplicateAssessmentType_IgnoresCase()
    {
        var assessments = new List<Assessment>
        {
            new() { Id = 1, Type = "Objective" }
        };

        bool result = ValidationHelper.HasDuplicateAssessmentType(
            assessments,
            "objective");

        Assert.True(result);
    }
}
