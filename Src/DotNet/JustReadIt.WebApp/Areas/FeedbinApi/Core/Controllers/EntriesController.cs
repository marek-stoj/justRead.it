using System.Collections.Generic;
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

  public class EntriesController : FeedbinApiController {

    private readonly IFeedItemRepository _feedItemRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public EntriesController(IFeedItemRepository feedItemRepository, ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(feedItemRepository, "feedItemRepository");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _feedItemRepository = feedItemRepository;
      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public EntriesController()
      : this(CommonIoC.GetFeedItemRepository(), CommonIoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    /// <remarks>
    /// This behaves differently than in the spec.
    ///   - We're limiting number of returned entries to 100 if 'per_page' param is not given.
    ///   - If 'per_page' param is greater than 100, we return 400 - Bad Request.
    /// </remarks>
    // TODO IMM HI: analyze doc and implement return codes for various situations
    [HttpGet]
    public IEnumerable<JsonModel.Entry> GetAll(int? per_page = null, int? page = null, string since = null, bool? read = null, bool? starred = null, string ids = null) {
      int userAccountId = CurrentUserAccountId;
      IEnumerable<FeedItem> feedItems;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        int maxCount;

        FeedItemFilter feedItemFilter =
          QueryUtils.CreateFeedItemQueryParams(
            out maxCount,
            feedId: null,
            perPage: per_page,
            page: page,
            since: since,
            read: read,
            starred: starred,
            ids: ids);

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

    [HttpGet]
    // TODO IMM HI: analyze doc and implement return codes for various situations
    public JsonModel.Entry Get(int id) {
      int userAccountId = CurrentUserAccountId;
      FeedItem feedItem;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        feedItem =
          _feedItemRepository.FindById(id);

        if (feedItem != null
         && !_subscriptionRepository.IsSubscribedToFeed(userAccountId, feedItem.FeedId)) {
          ts.Complete();

          throw HttpForbidden();
        }

        ts.Complete();
      }

      if (feedItem == null) {
        throw HttpNotFound();
      }

      JsonModel.Entry entryModel =
        _domainToJsonModelMapper.CreateEntry(feedItem);

      return entryModel;
    }

  }

}
