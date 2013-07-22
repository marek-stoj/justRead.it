using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.Mvc {

  public static class HtmlHelperExtensions {

    /// <param name="button"></param>
    /// <param name="text">Can be null.</param>
    /// <param name="htmlHelper"></param>
    /// <param name="htmlAttributes"></param>
    public static MvcHtmlString Button<T>(this HtmlHelper htmlHelper, T button, string text, object htmlAttributes)
      where T : struct {
      if (!typeof(T).IsEnum) {
        throw new InvalidOperationException("Type parameter T must denote an enum type.");
      }

      if (text == null) {
        throw new ArgumentNullException("text");
      }

      TagBuilder inputTagBuilder = new TagBuilder("input");

      inputTagBuilder.MergeAttribute("type", "submit");
      inputTagBuilder.MergeAttribute("name", typeof(T).Name + "_" + button);
      inputTagBuilder.MergeAttribute("value", text);

      if (htmlAttributes != null) {
        inputTagBuilder.MergeAttributes(ObjectToCaseSensitiveDictionary(htmlAttributes));
      }

      return MvcHtmlString.Create(inputTagBuilder.ToString());
    }

    public static MvcHtmlString Button<T>(this HtmlHelper htmlHelper, T button, string text)
      where T : struct {
      return htmlHelper.Button(button, text, null);
    }

    private static Dictionary<string, object> ObjectToCaseSensitiveDictionary(object values) {
      var dict = new Dictionary<string, object>(StringComparer.Ordinal);

      if (values != null) {
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(values)) {
          object val = prop.GetValue(values);

          dict[prop.Name] = val;
        }
      }

      return dict;
    }

  }

}
