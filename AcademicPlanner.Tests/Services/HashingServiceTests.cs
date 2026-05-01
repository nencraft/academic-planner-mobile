using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Services;

namespace AcademicPlanner.Tests.Services;

public class HashingServiceTests
{
    [Fact]
    public void HashPassword_ReturnsHashAndSalt()
    {
        var service = new HashingService();

        var result = service.HashPassword("Password123");

        Assert.False(string.IsNullOrWhiteSpace(result.Hash));
        Assert.False(string.IsNullOrWhiteSpace(result.Salt));
    }

    [Fact]
    public void HashPassword_UsesDifferentSaltEachTime()
    {
        var service = new HashingService();

        var first = service.HashPassword("Password123");
        var second = service.HashPassword("Password123");

        Assert.NotEqual(first.Salt, second.Salt);
        Assert.NotEqual(first.Hash, second.Hash);
    }

    [Fact]
    public void VerifyPassword_ReturnsTrue_ForCorrectPassword()
    {
        var service = new HashingService();
        var stored = service.HashPassword("Password123");

        bool result = service.VerifyPassword("Password123", stored.Hash, stored.Salt);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_ForIncorrectPassword()
    {
        var service = new HashingService();
        var stored = service.HashPassword("Password123");

        bool result = service.VerifyPassword("WrongPassword123", stored.Hash, stored.Salt);

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_WhenHashDoesNotMatchSalt()
    {
        var service = new HashingService();

        var first = service.HashPassword("Password123");
        var second = service.HashPassword("Password123");

        bool result = service.VerifyPassword("Password123", first.Hash, second.Salt);

        Assert.False(result);
    }
}
