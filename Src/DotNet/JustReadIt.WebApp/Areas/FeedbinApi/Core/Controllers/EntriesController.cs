using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Entries;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Utils;
using JustReadIt.WebApp.Core.App;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class EntriesController : FeedbinApiController {

    private const int _MaxIdsInFilterCount = 100;
    private const int _MaxEntriesToMarkCount = 1000;

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

        if (feedItemFilter.Ids != null && feedItemFilter.Ids.Count > _MaxIdsInFilterCount) {
          throw HttpBadRequest();
        }

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

      return entriesModel;
    }

    [HttpGet]
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

    [HttpGet]
    public IEnumerable<int> GetAllUnread() {
      int userAccountId = CurrentUserAccountId;
      IEnumerable<int> unreadFeedItemIds;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        unreadFeedItemIds =
          _feedItemRepository.GetAllUnreadIds(userAccountId);

        ts.Complete();
      }

      return unreadFeedItemIds;
    }

    [HttpPost]
    public IEnumerable<int> CreateUnread(CreateUnreadInputModel input) {
      if (input == null || input.UnreadEntries == null) {
        throw HttpBadRequest();
      }

      if (input.UnreadEntries.Count > _MaxEntriesToMarkCount) {
        throw HttpBadRequest();
      }

      int userAccountId = CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        List<int> existingFeedItemIds =
          _feedItemRepository.GetExistingFeedItemIds(
            userAccountId,
            input.UnreadEntries)
            .ToList();

        if (existingFeedItemIds.Count == 0) {
          ts.Complete();

          return Enumerable.Empty<int>();
        }

        _feedItemRepository.MarkUnread(
          userAccountId,
          existingFeedItemIds);

        ts.Complete();

        return existingFeedItemIds;
      }
    }

    [HttpDelete]
    public IEnumerable<int> DeleteUnreadViaDelete(DeleteUnreadInputModel input) {
      return DoDeleteUnread(input);
    }

    [HttpPost]
    public IEnumerable<int> DeleteUnreadViaPost(DeleteUnreadInputModel input) {
      return DoDeleteUnread(input);
    }

    [HttpGet]
    public IEnumerable<int> GetAllStarred() {
      int userAccountId = CurrentUserAccountId;
      IEnumerable<int> starredFeedItemIds;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        starredFeedItemIds =
          _feedItemRepository.GetAllStarredIds(userAccountId);

        ts.Complete();
      }

      return starredFeedItemIds;
    }

    [HttpPost]
    public IEnumerable<int> CreateStarred(CreateStarredInputModel input) {
      if (input == null || input.StarredEntries == null) {
        throw HttpBadRequest();
      }

      if (input.StarredEntries.Count > _MaxEntriesToMarkCount) {
        throw HttpBadRequest();
      }

      int userAccountId = CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        List<int> existingFeedItemIds =
          _feedItemRepository.GetExistingFeedItemIds(
            userAccountId,
            input.StarredEntries)
            .ToList();

        if (existingFeedItemIds.Count == 0) {
          ts.Complete();

          return Enumerable.Empty<int>();
        }

        _feedItemRepository.MarkStarred(
          userAccountId,
          existingFeedItemIds);

        ts.Complete();

        return existingFeedItemIds;
      }
    }

    [HttpDelete]
    public IEnumerable<int> DeleteStarredViaDelete(DeleteStarredInputModel input) {
      return DoDeleteStarred(input);
    }

    [HttpPost]
    public IEnumerable<int> DeleteStarredViaPost(DeleteStarredInputModel input) {
      return DoDeleteStarred(input);
    }

    private IEnumerable<int> DoDeleteUnread(DeleteUnreadInputModel input) {
      if (input == null || input.UnreadEntries == null) {
        throw HttpBadRequest();
      }

      if (input.UnreadEntries.Count > _MaxEntriesToMarkCount) {
        throw HttpBadRequest();
      }

      if (input.UnreadEntries.Count == 0) {
        return Enumerable.Empty<int>();
      }

      int userAccountId = CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        List<int> existingFeedItemIds =
          _feedItemRepository.GetExistingFeedItemIds(
            userAccountId,
            input.UnreadEntries)
            .ToList();

        if (existingFeedItemIds.Count == 0) {
          ts.Complete();

          return Enumerable.Empty<int>();
        }

        _feedItemRepository.MarkRead(
          userAccountId,
          existingFeedItemIds);

        ts.Complete();

        return existingFeedItemIds;
      }
    }

    private IEnumerable<int> DoDeleteStarred(DeleteStarredInputModel input) {
      if (input == null || input.StarredEntries == null) {
        throw HttpBadRequest();
      }

      if (input.StarredEntries.Count > _MaxEntriesToMarkCount) {
        throw HttpBadRequest();
      }

      if (input.StarredEntries.Count == 0) {
        return Enumerable.Empty<int>();
      }

      int userAccountId = CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        List<int> existingFeedItemIds =
          _feedItemRepository.GetExistingFeedItemIds(
            userAccountId,
            input.StarredEntries)
            .ToList();

        if (existingFeedItemIds.Count == 0) {
          ts.Complete();

          return Enumerable.Empty<int>();
        }

        _feedItemRepository.MarkUnstarred(
          userAccountId,
          existingFeedItemIds);

        ts.Complete();

        return existingFeedItemIds;
      }
    }

  }

}
