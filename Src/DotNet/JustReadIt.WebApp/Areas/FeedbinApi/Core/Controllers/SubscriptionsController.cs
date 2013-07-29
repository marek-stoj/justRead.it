using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Resources;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Subscriptions;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Core.App;
using Feeds = JustReadIt.Core.Services.Feeds;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class SubscriptionsController : FeedbinApiController {

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;
    private readonly Feeds.IFeedParser _feedParser;

    public SubscriptionsController(ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper, Feeds.IFeedParser feedParser) {
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");
      Guard.ArgNotNull(feedParser, "feedParser");

      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
      _feedParser = feedParser;
    }

    public SubscriptionsController()
      : this(IoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper(), IoC.GetFeedParser()) {
    }

    [HttpGet]
    public IEnumerable<JsonModel.Subscription> GetAll(string since = null) {
      int userAccountId = CurrentUserAccountId;

      DateTime? sinceDate =
        !string.IsNullOrEmpty(since)
          ? ParseFeedbinDateTime(since)
          : null;

      IEnumerable<Subscription> subscriptions;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionRepository.GetAll(userAccountId, sinceDate);

        ts.Complete();
      }

      List<JsonModel.Subscription> subscriptionsModel =
        subscriptions.Select(_domainToJsonModelMapper.CreateSubscription)
          .ToList();

      return subscriptionsModel;
    }

    [HttpGet]
    public JsonModel.Subscription Get(int id) {
      int userAccountId = CurrentUserAccountId;

      Subscription subscription =
        _subscriptionRepository.FindById(userAccountId, id);

      if (subscription == null) {
        throw HttpNotFound();
      }

      JsonModel.Subscription subscriptionModel =
        _domainToJsonModelMapper.CreateSubscription(subscription);

      return subscriptionModel;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> Create(CreateInputModel input) {
      string feedUrl = input.feed_url;

      if (string.IsNullOrEmpty(feedUrl)) {
        throw HttpBadRequest();
      }

      HttpClient httpClient = null;
      HttpResponseMessage feedResponseMessage = null;

      try {
        httpClient = new HttpClient();
        feedResponseMessage = await httpClient.GetAsync(feedUrl);

        IEnumerable<string> contentTypes;

        if (!feedResponseMessage.Content.Headers.TryGetValues("Content-Type", out contentTypes)) {
          throw HttpNotFound();
        }

        string contentType = contentTypes.FirstOrDefault();

        if (string.IsNullOrEmpty(contentType)) {
          throw HttpNotFound();
        }

        if (!IsRssContentType(contentType)) {
          // TODO IMM HI: parse the web page searching for feeds?
          throw HttpUnsupportedMediaType();
        }

        int userAccountId = CurrentUserAccountId;

        using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
          string apiFeedUrl;

          int? subscriptionId =
            _subscriptionRepository.FindIdByFeedUrl(userAccountId, feedUrl);

          if (subscriptionId.HasValue) {
            apiFeedUrl = Routes.CreateApiUrlForGetSubscription(Url, subscriptionId.Value);

            ts.Complete();

            throw HttpFound(new Dictionary<string, string> { { "Location", apiFeedUrl }, });
          }

          string feedContent = await feedResponseMessage.Content.ReadAsStringAsync();

          Feeds.Feed feed = _feedParser.Parse(feedContent);

          var subscription =
            new Subscription {
              UserAccountId = userAccountId,
              Feed =
                new Feed {
                  FeedUrl = feedUrl,
                  SiteUrl = feed.SiteUrl ?? feedUrl,
                  Title = feed.Title ?? CommonResources.UntitledFeedTitle,
                },
            };

          _subscriptionRepository.Add(subscription);

          apiFeedUrl = Routes.CreateApiUrlForGetSubscription(Url, subscription.Id);

          ts.Complete();

          throw HttpCreated(new Dictionary<string, string> { { "Location", apiFeedUrl }, });
        }
      }
      finally {
        try {
          if (feedResponseMessage != null) {
            feedResponseMessage.Dispose();
          }
        }
        finally {
          if (httpClient != null) {
            httpClient.Dispose();
          }
        }
      }
    }

    private static bool IsRssContentType(string contentType) {
      return contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) > -1
             || contentType.IndexOf("rss", StringComparison.OrdinalIgnoreCase) > -1;
    }

  }

}
