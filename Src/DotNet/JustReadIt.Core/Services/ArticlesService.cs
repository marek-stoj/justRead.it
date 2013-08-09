using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JustReadIt.Core.Common;
using NReadability;

namespace JustReadIt.Core.Services {

  public class ArticlesService : IArticlesService {

    private readonly NReadabilityWebTranscoder _nReadabilityWebTranscoder;

    public ArticlesService(IUrlFetcher urlFetcher) {
      Guard.ArgNotNull(urlFetcher, "urlFetcher");

      _nReadabilityWebTranscoder =
        new NReadabilityWebTranscoder(
          new NReadabilityTranscoder(),
          urlFetcher);
    }

    public ArticlesService()
      : this(CommonIoC.GetUrlFetcher()) {
    }

    public string GetArticleContentHtml(string articleUrl) {
      Guard.ArgNotNullNorEmpty(articleUrl, "articleUrl");

      var webTranscodingInput = new WebTranscodingInput(articleUrl);

      WebTranscodingResult webTranscodingResult =
        _nReadabilityWebTranscoder.Transcode(
          webTranscodingInput);

      if (!webTranscodingResult.ContentExtracted) {
        return null;
      }

      // extract body and return it
      XDocument xDocument =
        new SgmlDomBuilder()
          .BuildDocument(webTranscodingResult.ExtractedContent);

      XElement readInnerDivElement = xDocument.GetElementById("readInner");
      XElement h1Element = readInnerDivElement.GetChildrenByTagName("h1").FirstOrDefault();

      if (h1Element != null) {
        h1Element.Remove();
      }

      List<string> removedHyperlinkUrls;

      ArticleContentProcessor.ReplaceHyperlinksWithSpans(
        readInnerDivElement,
        out removedHyperlinkUrls);

      // TODO IMM HI: rethink
      if (removedHyperlinkUrls.Count > 0) {
        readInnerDivElement.Add(
          XElement.Parse(
            "<div><hr /><h5>Links</h5><ol>" +
            string.Join("", removedHyperlinkUrls.Distinct().Select(url => string.Format("<li><a href=\"{0}\" target=\"_blank\">{0}</a></li>", url)))
            + "</ol></div>"));
      }

      // TODO IMM HI: remove alphabet
      string contentHtml = readInnerDivElement.GetInnerHtml() + "<p>abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcd</p>";

      return contentHtml;
    }

  }

}
