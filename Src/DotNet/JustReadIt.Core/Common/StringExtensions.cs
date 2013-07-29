using System;
using System.Text.RegularExpressions;

namespace JustReadIt.Core.Common {

  public static class StringExtensions {

    public static string TrimmedOrNull(this string s) {
      if (s == null) {
        return null;
      }

      string sTrimmed = s.Trim();

      if (sTrimmed.Length == 0) {
        return null;
      }

      return sTrimmed;
    }

    public static string ReplaceVariables(this string s, object variablesMapping) {
      if (s == null) {
        return null;
      }

      var regex = new Regex(@"\$\{(?<variable>[^\}]+)\}");

      string replaced = regex.Replace(s,
        match => {
          var variableName = match.Groups["variable"].Value;
          var property = variablesMapping.GetType().GetProperty(variableName);

          if (property == null) {
            throw new ArgumentException("No value specified for variable " + variableName + ".");
          }

          return property.GetValue(variablesMapping, null).ToString();
        });

      return replaced;
    }

    public static bool EqualsOrdinalIgnoreCase(this string s, string other = null) {
      Guard.ArgNotNull(s, "s");

      return s.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsNullOrEmpty(this string s) {
      return string.IsNullOrEmpty(s);
    }

    public static bool ContainsOrdinalIgnoreCase(this string s, string value) {
      Guard.ArgNotNull(s, "s");
      Guard.ArgNotNullNorEmpty(value, "value");

      return s.IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1;
    }

  }

}
