// Copyright (c) 2008 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text.RegularExpressions;

namespace JustReadIt.Core.Common
{
  /// <summary>
  /// Converts provided HTML-formatted string to text format.
  /// </summary>
  internal static class HtmlToText
  {
    // regular expression to match html line breaks or paragraph tags and adjacent whitespace
    private static readonly Regex htmlNewlinePattern = new Regex("\\s*<(br|/?p|/?h[1-6])[^>\\n\\r]*?>\\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // regular expression to match list tags and adjacent whitespace
    private static readonly Regex htmlListPattern = new Regex("\\s*<li>\\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // regular expression to match any remaining html tags
    private static readonly Regex htmlTagPattern = new Regex("</?([^<]*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // regular expression to match empty lines
    private static readonly Regex doubleEmptyLinesPattern = new Regex("\\n\\n\\n+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #region Public methods

    /// <summary>
    /// Convert provided html string to plain text preserving the formatting as much as possible. Ensure line wrapping to 72 chars as default.
    /// NOTE: add support for more HTML tags here.
    /// For the present, convert &lt;br&gt; to '\n'
    ///                  convert &lt;p&gt; and &lt;/p&gt; to '\n'
    ///                  convert &lt;li&gt; to "\n- "
    /// </summary>
    public static string htmlToPlainText(string html, int? maxLineWidth)
    {
      if (string.IsNullOrEmpty(html))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "html");
      }

      // Clear any html indentation and incidental whitespace
      string text = StringUtil.StripAndCollapse(html);

      // Replace <br> and <p> tags with new line characters.
      // Replace <li> tags (HTML bullets) with dashes.
      // Remove any remaining HTML tags not supported yet.
      // Replace any HTML escape string with appropriate character.
      // Finally collapse empty lines.
      text = htmlNewlinePattern.Replace(text, "\n");
      text = htmlListPattern.Replace(text, "\n- ");
      text = htmlTagPattern.Replace(text, "");
      text = StringUtil.UnescapeHTML(text).Trim();
      text = doubleEmptyLinesPattern.Replace(text, "\n\n");

      return
        maxLineWidth.HasValue
          ? StringUtil.FixedWidth(text.Split(new[] { '\n' }, StringSplitOptions.None), maxLineWidth.Value)
          : text;
    }

    #endregion
  }
}
