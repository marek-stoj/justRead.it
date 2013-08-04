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

    /// <summary>
    /// Returns a new character sequence that is a subsequence of this sequence. The subsequence starts with the character at the specified index and ends with the character at index end - 1. The length of the returned sequence is end - start, so if start == end then an empty sequence is returned.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="start">the start index, inclusive</param>
    /// <param name="end">the end index, exclusive</param>
    /// <returns>the specified subsequence</returns>
    /// <exception cref="IndexOutOfRangeException"> if start or end are negative, if end is greater than length(), or if start is greater than end</exception>
    public static string SubSequence(this string s, int start, int end) {
      if (s == null) {
        return null;
      }

      if (start < 0) throw new ArgumentOutOfRangeException("start", "Argument must not be negative.");
      if (end < 0) throw new ArgumentOutOfRangeException("end", "Argument must not be negative.");
      if (end > s.Length) throw new ArgumentOutOfRangeException("end", "Argument must not be greater than the input string's length.");
      if (start > end) throw new ArgumentOutOfRangeException("start", "Argument must not be greater than the 'end' argument.");

      return s.Substring(start, end - start);
    }


    public static string StripHtml(this string s, int? maxLineWidth) {
      if (s == null) {
        return null;
      }

      int bodyStartIndex = s.IndexOf("<body>", StringComparison.OrdinalIgnoreCase);

      if (bodyStartIndex != -1) {
        int bodyEndIndex = s.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

        if (bodyEndIndex == -1) {
          bodyEndIndex = s.Length;
        }

        s = s.SubSequence(bodyStartIndex + "<body>".Length, bodyEndIndex);
      }

      return HtmlToText.htmlToPlainText(s, maxLineWidth);
    }

    public static string StripHtml(this string s) {
      return StripHtml(s, null);
    }

  }

}
