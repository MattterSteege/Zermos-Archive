using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Utils;

[AttributeUsage(AttributeTargets.Property)]
public class SettingAttribute : Attribute
{
    public string[] Options { get; }

    public SettingAttribute(params string[] options)
    {
        Options = options;
    }

    public bool IsValid(string value)
    {
        if (Options == null || Options.Length == 0) return true; // No options means any value is valid
        if (string.IsNullOrEmpty(value) || value == string.Empty) return true; // Null values are always valid
        if (Options.Length == 1) return Regex.IsMatch(value, Options[0]); // Single option is treated as a regex pattern
        return Options.Any(option => string.Equals(option, value, StringComparison.OrdinalIgnoreCase)); // Multiple options are treated as a case-insensitive match
    }
}