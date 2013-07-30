﻿using System;
using System.Collections.Generic;
using System.Linq;
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

  // TODO IMM HI: 404 instead of 403? check existence?
  public class SubscriptionsController : FeedbinApiController {

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;
    private readonly Feeds.IFeedFetcher _feedFetcher;
    private readonly Feeds.IFeedParser _feedParser;

    public SubscriptionsController(ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper, Feeds.IFeedFetcher feedFetcher, Feeds.IFeedParser feedParser) {
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");
      Guard.ArgNotNull(feedFetcher, "feedFetcher");
      Guard.ArgNotNull(feedParser, "feedParser");

      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
      _feedFetcher = feedFetcher;
      _feedParser = feedParser;
    }

    public SubscriptionsController()
      : this(IoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper(), IoC.GetFeedFetcher(), IoC.GetFeedParser()) {
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
      Subscription subscription;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        subscription = _subscriptionRepository.FindById(userAccountId, id);

        ts.Complete();
      }

      if (subscription == null) {
        throw HttpNotFound();
      }

      JsonModel.Subscription subscriptionModel =
        _domainToJsonModelMapper.CreateSubscription(subscription);

      return subscriptionModel;
    }

    [HttpPost]
    public void Create(CreateInputModel input) {
      if (input == null) {
        throw HttpBadRequest();
      }

      string feedUrl = input.feed_url;

      if (string.IsNullOrEmpty(feedUrl)) {
        throw HttpBadRequest();
      }

      Feeds.FetchFeedResult fetchFeedResult = _feedFetcher.FetchFeed(feedUrl);

      if (fetchFeedResult.ContentType.IsNullOrEmpty()) {
        throw HttpNotFound();
      }

      if (!IsRssContentType(fetchFeedResult.ContentType)) {
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

        string feedContent = fetchFeedResult.FeedContent;

        Feeds.Feed feed = _feedParser.Parse(feedContent);
        string feedTitle = (!string.IsNullOrEmpty(feed.Title) ? feed.Title : CommonResources.UntitledFeedTitle);

        var subscription =
          new Subscription {
            UserAccountId = userAccountId,
            CustomTitle = null,
            Feed =
              new Feed {
                FeedUrl = feedUrl,
                SiteUrl = feed.SiteUrl ?? feedUrl,
                Title = feedTitle,
              },
          };

        _subscriptionRepository.Add(subscription);

        apiFeedUrl = Routes.CreateApiUrlForGetSubscription(Url, subscription.Id);

        ts.Complete();

        throw HttpCreated(new Dictionary<string, string> { { "Location", apiFeedUrl }, });
      }
    }

    [HttpDelete]
    public void Delete(int id) {
      int userAccountId = CurrentUserAccountId;
      bool deleted;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        deleted = _subscriptionRepository.Delete(userAccountId, id);

        ts.Complete();
      }

      if (!deleted) {
        throw HttpNotFound();
      }

      throw HttpNoContent();
    }

    [HttpPatch]
    public void UpdateViaPatch(int id, UpdateInputModel input) {
      DoUpdate(id, input);
    }

    [HttpPost]
    public void UpdateViaPost(int id, UpdateInputModel input) {
      DoUpdate(id, input);
    }

    private void DoUpdate(int id, UpdateInputModel input) {
      if (input == null) {
        throw HttpBadRequest();
      }

      int userAccountId = CurrentUserAccountId;
      bool updated;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        updated =
          _subscriptionRepository.UpdateTitle(
            userAccountId,
            id,
            input.title);

        ts.Complete();
      }

      if (!updated) {
        throw HttpNotFound();
      }

      throw HttpOk();
    }

    private static bool IsRssContentType(string contentType) {
      return contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) > -1
             || contentType.IndexOf("rss", StringComparison.OrdinalIgnoreCase) > -1;
    }

  }

}
