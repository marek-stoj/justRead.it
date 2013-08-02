using System.Net;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;
using JustReadIt.Core.Services.Feeds.Exceptions;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedFetcher : IFeedFetcher {

    private readonly IWebClientFactory _webClientFactory;

    public FeedFetcher(IWebClientFactory webClientFactory) {
      Guard.ArgNotNull(webClientFactory, "webClientFactory");

      _webClientFactory = webClientFactory;
    }

    public FetchFeedResult FetchFeed(string feedUrl) {
      Guard.ArgNotNullNorEmpty(feedUrl, "feedUrl");

      string feedContent;
      WebHeaderCollection responseHeaders;

      try {
        using (IWebClient webClient = _webClientFactory.CreateWebClient()) {
          feedContent = webClient.DownloadString(feedUrl, out responseHeaders);
        }
      }
      catch (WebException exc) {
        HttpWebResponse httpWebResponse = exc.Response as HttpWebResponse;

        if (httpWebResponse != null) {
          if (httpWebResponse.StatusCode == HttpStatusCode.NotFound) {
            throw new FeedNotFoundException(feedUrl);
          }
        }

        throw;
      }

      return
        new FetchFeedResult {
          ContentType = responseHeaders["Content-Type"],
          FeedContent = feedContent,
        };
    }

  }

}
