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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JustReadIt.Core.Common
{
  /// <summary>
  /// Some common string manipulation utilities.
  /// </summary>
  internal static class StringUtil
  {
    // \u3000 is the double-byte space character in UTF-8
    // \u00A0 is the non-breaking space character (&nbsp;)
    // \u2007 is the figure space character (&#8199;)
    // \u202F is the narrow non-breaking space character (&#8239;)
    private const string _WhiteSpaces = " \r\n\t\u3000\u00A0\u2007\u202F";

    private static readonly Dictionary<string, char> _escapeStrings;

    #region Constructor(s)

    static StringUtil()
    {
      // HTML character entity references as defined in HTML 4, see http://www.w3.org/TR/REC-html40/sgml/entities.html
      _escapeStrings =
        new Dictionary<string, char>(252)
          {
            #region Entity references

            { "&nbsp;", '\u00A0' },
            { "&iexcl;", '\u00A1' },
            { "&cent;", '\u00A2' },
            { "&pound;", '\u00A3' },
            { "&curren;", '\u00A4' },
            { "&yen;", '\u00A5' },
            { "&brvbar;", '\u00A6' },
            { "&sect;", '\u00A7' },
            { "&uml;", '\u00A8' },
            { "&copy;", '\u00A9' },
            { "&ordf;", '\u00AA' },
            { "&laquo;", '\u00AB' },
            { "&not;", '\u00AC' },
            { "&shy;", '\u00AD' },
            { "&reg;", '\u00AE' },
            { "&macr;", '\u00AF' },
            { "&deg;", '\u00B0' },
            { "&plusmn;", '\u00B1' },
            { "&sup2;", '\u00B2' },
            { "&sup3;", '\u00B3' },
            { "&acute;", '\u00B4' },
            { "&micro;", '\u00B5' },
            { "&para;", '\u00B6' },
            { "&middot;", '\u00B7' },
            { "&cedil;", '\u00B8' },
            { "&sup1;", '\u00B9' },
            { "&ordm;", '\u00BA' },
            { "&raquo;", '\u00BB' },
            { "&frac14;", '\u00BC' },
            { "&frac12;", '\u00BD' },
            { "&frac34;", '\u00BE' },
            { "&iquest;", '\u00BF' },
            { "&Agrave;", '\u00C0' },
            { "&Aacute;", '\u00C1' },
            { "&Acirc;", '\u00C2' },
            { "&Atilde;", '\u00C3' },
            { "&Auml;", '\u00C4' },
            { "&Aring;", '\u00C5' },
            { "&AElig;", '\u00C6' },
            { "&Ccedil;", '\u00C7' },
            { "&Egrave;", '\u00C8' },
            { "&Eacute;", '\u00C9' },
            { "&Ecirc;", '\u00CA' },
            { "&Euml;", '\u00CB' },
            { "&Igrave;", '\u00CC' },
            { "&Iacute;", '\u00CD' },
            { "&Icirc;", '\u00CE' },
            { "&Iuml;", '\u00CF' },
            { "&ETH;", '\u00D0' },
            { "&Ntilde;", '\u00D1' },
            { "&Ograve;", '\u00D2' },
            { "&Oacute;", '\u00D3' },
            { "&Ocirc;", '\u00D4' },
            { "&Otilde;", '\u00D5' },
            { "&Ouml;", '\u00D6' },
            { "&times;", '\u00D7' },
            { "&Oslash;", '\u00D8' },
            { "&Ugrave;", '\u00D9' },
            { "&Uacute;", '\u00DA' },
            { "&Ucirc;", '\u00DB' },
            { "&Uuml;", '\u00DC' },
            { "&Yacute;", '\u00DD' },
            { "&THORN;", '\u00DE' },
            { "&szlig;", '\u00DF' },
            { "&agrave;", '\u00E0' },
            { "&aacute;", '\u00E1' },
            { "&acirc;", '\u00E2' },
            { "&atilde;", '\u00E3' },
            { "&auml;", '\u00E4' },
            { "&aring;", '\u00E5' },
            { "&aelig;", '\u00E6' },
            { "&ccedil;", '\u00E7' },
            { "&egrave;", '\u00E8' },
            { "&eacute;", '\u00E9' },
            { "&ecirc;", '\u00EA' },
            { "&euml;", '\u00EB' },
            { "&igrave;", '\u00EC' },
            { "&iacute;", '\u00ED' },
            { "&icirc;", '\u00EE' },
            { "&iuml;", '\u00EF' },
            { "&eth;", '\u00F0' },
            { "&ntilde;", '\u00F1' },
            { "&ograve;", '\u00F2' },
            { "&oacute;", '\u00F3' },
            { "&ocirc;", '\u00F4' },
            { "&otilde;", '\u00F5' },
            { "&ouml;", '\u00F6' },
            { "&divide;", '\u00F7' },
            { "&oslash;", '\u00F8' },
            { "&ugrave;", '\u00F9' },
            { "&uacute;", '\u00FA' },
            { "&ucirc;", '\u00FB' },
            { "&uuml;", '\u00FC' },
            { "&yacute;", '\u00FD' },
            { "&thorn;", '\u00FE' },
            { "&yuml;", '\u00FF' },
            { "&fnof;", '\u0192' },
            { "&Alpha;", '\u0391' },
            { "&Beta;", '\u0392' },
            { "&Gamma;", '\u0393' },
            { "&Delta;", '\u0394' },
            { "&Epsilon;", '\u0395' },
            { "&Zeta;", '\u0396' },
            { "&Eta;", '\u0397' },
            { "&Theta;", '\u0398' },
            { "&Iota;", '\u0399' },
            { "&Kappa;", '\u039A' },
            { "&Lambda;", '\u039B' },
            { "&Mu;", '\u039C' },
            { "&Nu;", '\u039D' },
            { "&Xi;", '\u039E' },
            { "&Omicron;", '\u039F' },
            { "&Pi;", '\u03A0' },
            { "&Rho;", '\u03A1' },
            { "&Sigma;", '\u03A3' },
            { "&Tau;", '\u03A4' },
            { "&Upsilon;", '\u03A5' },
            { "&Phi;", '\u03A6' },
            { "&Chi;", '\u03A7' },
            { "&Psi;", '\u03A8' },
            { "&Omega;", '\u03A9' },
            { "&alpha;", '\u03B1' },
            { "&beta;", '\u03B2' },
            { "&gamma;", '\u03B3' },
            { "&delta;", '\u03B4' },
            { "&epsilon;", '\u03B5' },
            { "&zeta;", '\u03B6' },
            { "&eta;", '\u03B7' },
            { "&theta;", '\u03B8' },
            { "&iota;", '\u03B9' },
            { "&kappa;", '\u03BA' },
            { "&lambda;", '\u03BB' },
            { "&mu;", '\u03BC' },
            { "&nu;", '\u03BD' },
            { "&xi;", '\u03BE' },
            { "&omicron;", '\u03BF' },
            { "&pi;", '\u03C0' },
            { "&rho;", '\u03C1' },
            { "&sigmaf;", '\u03C2' },
            { "&sigma;", '\u03C3' },
            { "&tau;", '\u03C4' },
            { "&upsilon;", '\u03C5' },
            { "&phi;", '\u03C6' },
            { "&chi;", '\u03C7' },
            { "&psi;", '\u03C8' },
            { "&omega;", '\u03C9' },
            { "&thetasym;", '\u03D1' },
            { "&upsih;", '\u03D2' },
            { "&piv;", '\u03D6' },
            { "&bull;", '\u2022' },
            { "&hellip;", '\u2026' },
            { "&prime;", '\u2032' },
            { "&Prime;", '\u2033' },
            { "&oline;", '\u203E' },
            { "&frasl;", '\u2044' },
            { "&weierp;", '\u2118' },
            { "&image;", '\u2111' },
            { "&real;", '\u211C' },
            { "&trade;", '\u2122' },
            { "&alefsym;", '\u2135' },
            { "&larr;", '\u2190' },
            { "&uarr;", '\u2191' },
            { "&rarr;", '\u2192' },
            { "&darr;", '\u2193' },
            { "&harr;", '\u2194' },
            { "&crarr;", '\u21B5' },
            { "&lArr;", '\u21D0' },
            { "&uArr;", '\u21D1' },
            { "&rArr;", '\u21D2' },
            { "&dArr;", '\u21D3' },
            { "&hArr;", '\u21D4' },
            { "&forall;", '\u2200' },
            { "&part;", '\u2202' },
            { "&exist;", '\u2203' },
            { "&empty;", '\u2205' },
            { "&nabla;", '\u2207' },
            { "&isin;", '\u2208' },
            { "&notin;", '\u2209' },
            { "&ni;", '\u220B' },
            { "&prod;", '\u220F' },
            { "&sum;", '\u2211' },
            { "&minus;", '\u2212' },
            { "&lowast;", '\u2217' },
            { "&radic;", '\u221A' },
            { "&prop;", '\u221D' },
            { "&infin;", '\u221E' },
            { "&ang;", '\u2220' },
            { "&and;", '\u2227' },
            { "&or;", '\u2228' },
            { "&cap;", '\u2229' },
            { "&cup;", '\u222A' },
            { "&int;", '\u222B' },
            { "&there4;", '\u2234' },
            { "&sim;", '\u223C' },
            { "&cong;", '\u2245' },
            { "&asymp;", '\u2248' },
            { "&ne;", '\u2260' },
            { "&equiv;", '\u2261' },
            { "&le;", '\u2264' },
            { "&ge;", '\u2265' },
            { "&sub;", '\u2282' },
            { "&sup;", '\u2283' },
            { "&nsub;", '\u2284' },
            { "&sube;", '\u2286' },
            { "&supe;", '\u2287' },
            { "&oplus;", '\u2295' },
            { "&otimes;", '\u2297' },
            { "&perp;", '\u22A5' },
            { "&sdot;", '\u22C5' },
            { "&lceil;", '\u2308' },
            { "&rceil;", '\u2309' },
            { "&lfloor;", '\u230A' },
            { "&rfloor;", '\u230B' },
            { "&lang;", '\u2329' },
            { "&rang;", '\u232A' },
            { "&loz;", '\u25CA' },
            { "&spades;", '\u2660' },
            { "&clubs;", '\u2663' },
            { "&hearts;", '\u2665' },
            { "&diams;", '\u2666' },
            { "&quot;", '\u0022' },
            { "&amp;", '\u0026' },
            { "&lt;", '\u003C' },
            { "&gt;", '\u003E' },
            { "&OElig;", '\u0152' },
            { "&oelig;", '\u0153' },
            { "&Scaron;", '\u0160' },
            { "&scaron;", '\u0161' },
            { "&Yuml;", '\u0178' },
            { "&circ;", '\u02C6' },
            { "&tilde;", '\u02DC' },
            { "&ensp;", '\u2002' },
            { "&emsp;", '\u2003' },
            { "&thinsp;", '\u2009' },
            { "&zwnj;", '\u200C' },
            { "&zwj;", '\u200D' },
            { "&lrm;", '\u200E' },
            { "&rlm;", '\u200F' },
            { "&ndash;", '\u2013' },
            { "&mdash;", '\u2014' },
            { "&lsquo;", '\u2018' },
            { "&rsquo;", '\u2019' },
            { "&sbquo;", '\u201A' },
            { "&ldquo;", '\u201C' },
            { "&rdquo;", '\u201D' },
            { "&bdquo;", '\u201E' },
            { "&dagger;", '\u2020' },
            { "&Dagger;", '\u2021' },
            { "&permil;", '\u2030' },
            { "&lsaquo;", '\u2039' },
            { "&rsaquo;", '\u203A' },
            { "&euro;", '\u20AC' },

            #endregion
          };
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Replace all the occurences of HTML escape strings with the respective characters.
    /// </summary>
    public static string UnescapeHTML(string str)
    {
      char[] chars = str.ToCharArray();
      char[] escaped = new char[chars.Length];

      // Note: escaped[pos] = end of the escaped char array.
      int pos = 0;

      for (int i = 0; i < chars.Length; )
      {
        if (chars[i] != '&')
        {
          escaped[pos++] = chars[i++];
          continue;
        }

        // Allow e.g. &#123;
        int j = i + 1;

        if (j < chars.Length && chars[j] == '#')
        {
          j++;
        }

        // Scan until we find a char that is not letter or digit.
        for (; j < chars.Length; j++)
        {
          if (!char.IsLetterOrDigit(chars[j]))
          {
            break;
          }
        }

        bool replaced = false;

        if (j < chars.Length && chars[j] == ';')
        {
          if (str[i + 1] == '#') // Check for &#D; and &#xD; pattern
          {
            try
            {
              long charCode = 0;
              char ch = str[i + 2];

              if (ch == 'x' || ch == 'X')
              {
                charCode = long.Parse(new string(chars, i + 3, j - i - 3), NumberStyles.HexNumber);
              }
              else if (char.IsDigit(ch))
              {
                charCode = long.Parse(new string(chars, i + 2, j - i - 2));
              }

              if (charCode > 0 && charCode < 65536)
              {
                escaped[pos++] = (char)charCode;
                replaced = true;
              }
            }
            catch (FormatException)
            {
              // Failed, not replaced.
            }
          }
          else
          {
            string key = new string(chars, i, j - i + 1);
            char repl;

            if (_escapeStrings.TryGetValue(key, out repl))
            {
              escaped[pos++] = repl;
              replaced = true;
            }
          }

          j++; // Skip over ';'
        }

        if (!replaced)
        {
          // Not a recognized escape sequence, leave as-is
          Array.Copy(chars, i, escaped, pos, j - i);
          pos += j - i;
        }

        i = j;
      }

      return new string(escaped, 0, pos);
    }

    /// <summary>
    /// Strip white spaces from both end, and collapse white spaces in the middle.
    /// </summary>
    public static string StripAndCollapse(string str)
    {
      return CollapseWhitespace(Strip(str));
    }

    /// <summary>
    /// Reformats the given array of lines to a fixed width by inserting carriage returns and trimming unnecessary whitespace.
    /// </summary>
    public static string FixedWidth(string[] lines, int width)
    {
      var formatStr = new StringBuilder();

      for (int i = 0; i < lines.Length; i++)
      {
        int curWidth = 0;

        if (i != 0)
        {
          formatStr.Append("\n");
        }

        // a small optimization
        if (lines[i].Length <= width)
        {
          formatStr.Append(lines[i]);
          continue;
        }

        IEnumerable<string> words = SplitAndTrim(lines[i], _WhiteSpaces);

        foreach (string word in words)
        {
          if (curWidth == 0 || (curWidth + word.Length) < width)
          {
            // add a space if we're not at the beginning of a line
            if (curWidth != 0)
            {
              formatStr.Append(" ");
              curWidth += 1;
            }

            curWidth += word.Length;
            formatStr.Append(word);
          }
          else
          {
            formatStr.Append("\n");
            curWidth = word.Length;
            formatStr.Append(word);
          }
        }
      }

      return formatStr.ToString();
    }

    #endregion

    #region Private helper methods

    /// <summary>
    /// Replaces any string of adjacent whitespace characters with the whitespace character " ".
    /// </summary>
    private static string CollapseWhitespace(string str)
    {
      return Collapse(str, _WhiteSpaces, " ");
    }

    /// <summary>
    /// Replaces any string of matched characters with the supplied string.
    /// This is a more general version of CollapseWhitespace.
    /// E.g. Collapse("hello     world", " ", "::") will return the following string: "hello::world"
    /// </summary>
    private static string Collapse(string str, string chars, string replacement)
    {
      if (str == null)
      {
        return null;
      }

      StringBuilder newStr = new StringBuilder();

      bool prevCharMatched = false;

      foreach (char ch in str)
      {
        if (chars.IndexOf(ch) != -1)
        {
          // this character is matched
          if (prevCharMatched)
          {
            // apparently a string of matched chars, so don't Append anything
            // to the string
            continue;
          }

          prevCharMatched = true;
          newStr.Append(replacement);
        }
        else
        {
          prevCharMatched = false;
          newStr.Append(ch);
        }
      }

      return newStr.ToString();
    }

    /// <summary>
    /// Strip - strips both ways
    /// </summary>
    private static string Strip(string str)
    {
      return MegaStrip(str, true, true, _WhiteSpaces);
    }

    /// <summary>
    /// This is a both way strip.
    /// </summary>
    private static string MegaStrip(string str, bool left, bool right, string what)
    {
      if (str == null)
      {
        return null;
      }

      int limitLeft = 0;
      int limitRight = str.Length - 1;

      while (left && limitLeft <= limitRight && what.IndexOf(str[limitLeft]) >= 0)
      {
        limitLeft++;
      }

      while (right && limitRight >= limitLeft && what.IndexOf(str[limitRight]) >= 0)
      {
        limitRight--;
      }

      return str.SubSequence(limitLeft, limitRight + 1);
    }

    /// <summary>
    /// Split "str" into tokens by delimiters and optionally remove white spaces  from the splitted tokens.
    /// </summary>
    private static IEnumerable<string> Split(string str, string delims, bool trimTokens)
    {
      string[] tokens = str.Split(delims.ToCharArray());

      if (!trimTokens)
      {
        return tokens;
      }

      return
        tokens
          .Select(t => t.Trim())
          .ToArray();
    }

    /// <summary>
    /// Shorthand for <code>Split(str, delims, true)</code>.
    /// </summary>
    private static IEnumerable<string> SplitAndTrim(string str, string delims)
    {
      return Split(str, delims, true);
    }

    #endregion
  }
}
