using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Services;

namespace AcademicPlanner.Tests.Services;

public class PasswordValidationServiceTests
{
    [Theory]
    [InlineData("", "Password is required.")]
    [InlineData("   ", "Password is required.")]
    [InlineData("short1", "Password must be at least 8 characters.")]
    [InlineData("password", "Password must include at least one number.")]
    [InlineData("12345678", "Password must include at least one letter.")]
    public void ValidatePassword_ReturnsError_ForInvalidPassword(
        string password,
        string expectedError)
    {
        string? result = PasswordValidationService.ValidatePassword(password);

        Assert.Equal(expectedError, result);
    }

    [Theory]
    [InlineData("Password123")]
    [InlineData("abc12345")]
    [InlineData("Planner2026")]
    public void ValidatePassword_ReturnsNull_ForValidPassword(string password)
    {
        string? result = PasswordValidationService.ValidatePassword(password);

        Assert.Null(result);
    }
}
