using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AcademicPlanner.Helpers;

public static class ValidationHelper
{
    public static bool IsRequired(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool DatesAreValid(DateTime start, DateTime end)
    {
        return start <= end;
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(
            email.Trim(),
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }
}