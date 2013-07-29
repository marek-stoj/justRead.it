using System.Net;
using ImmRafSoft.Net;
using JustReadIt.Core.Common;

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

      using (IWebClient webClient = _webClientFactory.CreateWebClient()) {
        feedContent = webClient.DownloadString(feedUrl, out responseHeaders);
      }

      return
        new FetchFeedResult {
          ContentType = responseHeaders["Content-Type2"],
          FeedContent = feedContent,
        };
    }

  }

}
