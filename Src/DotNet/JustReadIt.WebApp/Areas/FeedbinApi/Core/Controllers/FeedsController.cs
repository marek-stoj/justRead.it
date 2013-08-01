﻿using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Utils;
using JustReadIt.WebApp.Core.App;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class FeedsController : FeedbinApiController {

    private const int _MaxEntriesForGetEntriesCount = 100;

    private readonly IFeedRepository _feedRepository;
    private readonly IFeedItemRepository _feedItemRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public FeedsController(IFeedRepository feedRepository, IFeedItemRepository feedItemRepository, ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(feedRepository, "feedRepository");
      Guard.ArgNotNull(feedItemRepository, "feedItemRepository");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _feedRepository = feedRepository;
      _feedItemRepository = feedItemRepository;
      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public FeedsController()
      : this(CommonIoC.GetFeedRepository(), CommonIoC.GetFeedItemRepository(), CommonIoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    /// <remarks>
    /// This behaves differently than in the spec.
    ///   - We won't return feeds with just any id - only those that are subscribed to by the current user.
    /// </remarks>
    [HttpGet]
    public JsonModel.Feed Get(int id) {
      int userAccountId = CurrentUserAccountId;
      Feed feed;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        feed = _feedRepository.FindById(id);

        if (feed == null) {
          throw HttpNotFound();
        }

        if (!_subscriptionRepository.IsSubscribedToFeed(userAccountId, id)) {
          ts.Complete();

          throw HttpNotFound();
        }

        ts.Complete();
      }

      JsonModel.Feed feedModel =
        _domainToJsonModelMapper.CreateFeed(feed);

      return feedModel;
    }

    /// <remarks>
    /// This behaves differently than in the spec.
    ///   - We're limiting number of returned entries to 100 if 'per_page' param is not given.
    ///   - If 'per_page' param is greater than 100, we return 400 - Bad Request.
    /// </remarks>
    [HttpGet]
    // TODO IMM HI: analyze doc and implement return codes for various situations
    public IEnumerable<JsonModel.Entry> GetEntries(int id, int? per_page = null, int? page = null, string since = null, bool? read = null, bool? starred = null) {
      if (per_page.HasValue && per_page.Value > _MaxEntriesForGetEntriesCount) {
        throw HttpBadRequest();
      }

      int userAccountId = CurrentUserAccountId;
      IEnumerable<FeedItem> feedItems;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {


        int maxCount;

        FeedItemFilter feedItemFilter =
          QueryUtils.CreateFeedItemQueryParams(
            out maxCount,
            feedId: id,
            perPage: per_page,
            page: page,
            since: since,
            read: read,
            starred: starred,
            ids: null);

        feedItems =
          _feedItemRepository.Query(
            userAccountId,
            maxCount,
            feedItemFilter);

        ts.Complete();
      }

      List<JsonModel.Entry> entriesModel =
        feedItems.Select(_domainToJsonModelMapper.CreateEntry)
          .ToList();

      if (entriesModel.Count == 0) {
        throw HttpNotFound();
      }

      return entriesModel;
    }

  }

}
