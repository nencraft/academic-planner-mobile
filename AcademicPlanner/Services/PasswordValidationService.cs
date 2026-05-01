using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Services;

public static class PasswordValidationService
{
    public static string? ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Password is required.";

        if (password.Length < 8)
            return "Password must be at least 8 characters.";

        if (!password.Any(char.IsLetter))
            return "Password must include at least one letter.";

        if (!password.Any(char.IsDigit))
            return "Password must include at least one number.";

        return null;
    }
}
