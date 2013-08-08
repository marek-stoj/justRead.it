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

      XElement readInnerDiv = xDocument.GetElementById("readInner");
      XElement h1Element = readInnerDiv.GetChildrenByTagName("h1").FirstOrDefault();

      if (h1Element != null) {
        h1Element.Remove();
      }

      string contentHtml = readInnerDiv.GetInnerHtml();

      return contentHtml;
    }

  }

}
