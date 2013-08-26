using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using NReadability;

namespace JustReadIt.Core.Services {

  public class ArticlesService : IArticlesService {

    private readonly IWebClientFactory _webClientFactory;

    private readonly NReadabilityWebTranscoder _nReadabilityWebTranscoder;
    private readonly NReadabilityTranscoder _nReadabilityTranscoder;

    public ArticlesService(IUrlFetcher urlFetcher, IWebClientFactory webClientFactory) {
      _webClientFactory = webClientFactory;
      Guard.ArgNotNull(urlFetcher, "urlFetcher");

      _nReadabilityTranscoder =
        new NReadabilityTranscoder();

      _nReadabilityWebTranscoder =
        new NReadabilityWebTranscoder(_nReadabilityTranscoder, urlFetcher);
    }

    public ArticlesService()
      : this(CommonIoC.GetUrlFetcher(), CommonIoC.GetWebClientFactory()) {
    }

    public string GetArticleContentHtml(string articleUrl) {
      Guard.ArgNotNullNorEmpty(articleUrl, "articleUrl");

      var webTranscodingInput = new WebTranscodingInput(articleUrl);
      string extractedContent = null;

      WebTranscodingResult webTranscodingResult =
        _nReadabilityWebTranscoder.Transcode(
          webTranscodingInput);

      if (webTranscodingResult.ContentExtracted) {
        extractedContent = webTranscodingResult.ExtractedContent;
      }
      else {
        TranscodingResult transcodingResult =
          TranscodeWithInstapaper(articleUrl);

        if (transcodingResult.ContentExtracted) {
          extractedContent = transcodingResult.ExtractedContent;
        }
      }

      if (string.IsNullOrEmpty(extractedContent)) {
        return null;
      }

      // extract body and return it
      XDocument xDocument =
        new SgmlDomBuilder()
          .BuildDocument(extractedContent);

      XElement readInnerDivElement = xDocument.GetElementById("readInner");
      XElement h1Element = readInnerDivElement.GetChildrenByTagName("h1").FirstOrDefault();

      if (h1Element != null) {
        h1Element.Remove();
      }

      List<string> removedHyperlinkUrls;

      ArticleContentProcessor.ReplaceHyperlinksWithSpans(
        readInnerDivElement,
        out removedHyperlinkUrls);

      string contentHtml = readInnerDivElement.GetInnerHtml();

      // TODO IMM HI: rethink
      if (removedHyperlinkUrls.Count > 0)
      {
        string linksHtml =
          string.Join(
            "",
            removedHyperlinkUrls
              .Distinct()
              .Select(url => string.Format("<li><a href=\"{0}\" target=\"_blank\">{0}</a></li>", url)));

        contentHtml +=
          " <div class=\"links-list\">" +
          "   <hr />" +
          "   <h4 class=\"links-list-header\">" +
          "     Links" +
          "   </h4>" +
          "   <ol>" +
          linksHtml +
          "   </ol>" +
          " </div>";
      }

      contentHtml = ArticleContentProcessor.ProcessUsingSmartyPants(contentHtml);

      return contentHtml;
    }

    private TranscodingResult TranscodeWithInstapaper(string url) {
      string htmlContent;

      using (IWebClient webClient = _webClientFactory.CreateWebClient()) {
        htmlContent =
          webClient.DownloadString(
            string.Format(
              "http://www.instapaper.com/text?u={0}",
              Uri.EscapeDataString(url)));
      }

      var transcodingInput =
        new TranscodingInput(htmlContent)
        {
          Url = url,
        };

      TranscodingResult transcodingResult =
        _nReadabilityTranscoder.Transcode(transcodingInput);

      return transcodingResult;
    }

  }

}
