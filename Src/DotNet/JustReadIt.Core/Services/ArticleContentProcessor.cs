using System.Collections.Generic;
using System.Xml.Linq;
using JustReadIt.Core.Common;
using NReadability;

namespace JustReadIt.Core.Services {

  // TODO IMM HI: review http://practicaltypography.com/summary-of-key-rules.html
  public static class ArticleContentProcessor {

    public static void ReplaceHyperlinksWithSpans(XElement rootElement, out List<string> removedHyperlinkUrls) {
      Guard.ArgNotNull(rootElement, "rootElement");

      var removedHyperlinkUrlsList = new List<string>();

      var elementsTraverser =
        new ElementsTraverser(
          element => {
            string elementName = element.Name.LocalName;

            if (!string.Equals("a", elementName)) {
              return;
            }

            element.Name = "span";

            string url = element.GetAttributeValue("href", null);

            if (!string.IsNullOrEmpty(url)) {
              removedHyperlinkUrlsList.Add(url);
            }
          });

      elementsTraverser.Traverse(rootElement);

      removedHyperlinkUrls = removedHyperlinkUrlsList;
    }

  }

}
