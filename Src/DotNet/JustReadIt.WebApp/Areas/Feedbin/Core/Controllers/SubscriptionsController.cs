﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Resources;
using JustReadIt.Core.Services;
using JustReadIt.Core.Services.Feeds.Exceptions;
using JustReadIt.WebApp.Areas.Feedbin.Core.Models.Subscriptions;
using JustReadIt.WebApp.Areas.Feedbin.Core.Services;
using JustReadIt.WebApp.Areas.Feedbin.Core.Utils;
using JustReadIt.WebApp.Core.Security;
using Feed = JustReadIt.Core.Domain.Feed;
using Subscription = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Subscription;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Controllers {

  public class SubscriptionsController : FeedbinApiController {

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;
    private readonly JustReadIt.Core.Services.Feeds.IFeedFetcher _feedFetcher;
    private readonly JustReadIt.Core.Services.Feeds.IFeedParser _feedParser;

    public SubscriptionsController(ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper, JustReadIt.Core.Services.Feeds.IFeedFetcher feedFetcher, JustReadIt.Core.Services.Feeds.IFeedParser feedParser) {
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
      : this(CommonIoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper(), CommonIoC.GetFeedFetcher(), CommonIoC.GetFeedParser()) {
    }

    [HttpGet]
    public IEnumerable<Subscription> GetAll(string since = null) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      DateTime? sinceDate =
        !string.IsNullOrEmpty(since)
          ? ModelUtils.ParseFeedbinDateTime(since)
          : null;

      IEnumerable<JustReadIt.Core.Domain.Subscription> subscriptions;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionRepository.Query(userAccountId, sinceDate);

        ts.Complete();
      }

      List<Subscription> subscriptionsModel =
        subscriptions.Select(_domainToJsonModelMapper.CreateSubscription)
          .ToList();

      return subscriptionsModel;
    }

    [HttpGet]
    public Subscription Get(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      JustReadIt.Core.Domain.Subscription subscription;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        subscription = _subscriptionRepository.FindById(id);

        ts.Complete();
      }

      if (subscription == null || subscription.UserAccountId != userAccountId) {
        throw HttpForbidden();
      }

      Subscription subscriptionModel =
        _domainToJsonModelMapper.CreateSubscription(subscription);

      return subscriptionModel;
    }

    [HttpPost]
    public void Create(CreateInputModel input) {
      if (input == null) {
        throw HttpBadRequest();
      }

      string feedUrl = input.FeedUrl;

      if (string.IsNullOrEmpty(feedUrl)) {
        throw HttpBadRequest();
      }

      JustReadIt.Core.Services.Feeds.FetchFeedResult fetchFeedResult;

      try {
        fetchFeedResult = _feedFetcher.FetchFeed(feedUrl);
      }
      catch (FeedNotFoundException) {
        throw HttpNotFound();
      }

      if (fetchFeedResult.ContentType.IsNullOrEmpty()) {
        throw HttpNotFound();
      }

      if (!IsRssContentType(fetchFeedResult.ContentType)) {
        // TODO IMM HI: parse the web page searching for feeds? implement response 300 - multiple choices; see: https://github.com/feedbin/feedbin-api/blob/master/content/subscriptions.md
        throw HttpUnsupportedMediaType();
      }

      int userAccountId = SecurityUtils.CurrentUserAccountId;

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

        JustReadIt.Core.Services.Feeds.Feed feed = _feedParser.Parse(feedContent);
        string feedTitle = (!string.IsNullOrEmpty(feed.Title) ? feed.Title : CommonResources.UntitledFeedTitle);

        var subscription =
          new JustReadIt.Core.Domain.Subscription {
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

        Debug.Assert(subscription.Id > 0);

        apiFeedUrl = Routes.CreateApiUrlForGetSubscription(Url, subscription.Id);

        ts.Complete();

        throw HttpCreated(new Dictionary<string, string> { { "Location", apiFeedUrl }, });
      }
    }

    [HttpDelete]
    public void Delete(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        if (!_subscriptionRepository.Exists(userAccountId, id)) {
          ts.Complete();

          throw HttpForbidden();
        }

        _subscriptionRepository.Delete(userAccountId, id);

        ts.Complete();
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

      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        if (!_subscriptionRepository.Exists(userAccountId, id)) {
          ts.Complete();

          throw HttpForbidden();
        }

        _subscriptionRepository.UpdateTitle(
          userAccountId,
          id,
          input.Title);

        ts.Complete();
      }

      throw HttpOk();
    }

    private static bool IsRssContentType(string contentType) {
      return contentType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) > -1
             || contentType.IndexOf("rss", StringComparison.OrdinalIgnoreCase) > -1;
    }

  }

}
