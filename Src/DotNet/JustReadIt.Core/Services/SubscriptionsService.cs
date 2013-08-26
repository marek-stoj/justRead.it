using System.Collections.Generic;
using System.Diagnostics;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Query;
using JustReadIt.Core.Domain.Query.Model;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.Services {

  public class SubscriptionsService : ISubscriptionsService {

    private readonly Feeds.IFeedFetcher _feedFetcher;
    private readonly Feeds.IFeedParser _feedParser;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserFeedGroupRepository _userFeedGroupRepository;
    private readonly ISubscriptionQueryDao _subscriptionQueryDao;

    public SubscriptionsService(Feeds.IFeedFetcher feedFetcher, Feeds.IFeedParser feedParser, ISubscriptionRepository subscriptionRepository, IUserFeedGroupRepository userFeedGroupRepository, ISubscriptionQueryDao subscriptionQueryDao) {
      Guard.ArgNotNull(feedFetcher, "feedFetcher");
      Guard.ArgNotNull(feedParser, "feedParser");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(userFeedGroupRepository, "userFeedGroupRepository");
      Guard.ArgNotNull(subscriptionQueryDao, "subscriptionQueryDao");

      _feedFetcher = feedFetcher;
      _feedParser = feedParser;
      _subscriptionRepository = subscriptionRepository;
      _userFeedGroupRepository = userFeedGroupRepository;
      _subscriptionQueryDao = subscriptionQueryDao;
    }

    public int Subscribe(int userAccountId, string url, string groupTitle) {
      Guard.ArgNotNullNorEmpty(url, "url");
      Guard.ArgNotNullNorEmpty(groupTitle, "groupTitle");

      // check if subscription aready exists
      using (var ts = TransactionUtils.CreateTransactionScope()) {
        int? existingSubscriptionId =
          _subscriptionRepository.FindIdByFeedUrl(userAccountId, url);

        if (existingSubscriptionId.HasValue) {
          // subscription for this URL and user account id already exists - just return

          ts.Complete();

          return existingSubscriptionId.Value;
        }

        ts.Complete();
      }

      // subscription doesn't exist - let's add it
      Feeds.FetchFeedResult fetchFeedResult = _feedFetcher.FetchFeed(url);
      Feeds.Feed parsedFeed = _feedParser.Parse(fetchFeedResult.FeedContent);

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        var newSubscription =
          new Domain.Subscription {
            UserAccountId = userAccountId,
            Feed =
              new Domain.Feed {
                Title = parsedFeed.Title,
                FeedUrl = url,
                SiteUrl = parsedFeed.SiteUrl,
              },
          };

        int? groupId =
          _userFeedGroupRepository.FindGroupIdByTitle(userAccountId, groupTitle);

        if (!groupId.HasValue) {
          var newUserFeedGroup =
            new Domain.UserFeedGroup {
              UserAccountId = userAccountId,
              Title = groupTitle,
            };

          _userFeedGroupRepository.Add(newUserFeedGroup);

          Debug.Assert(newUserFeedGroup.Id > 0);

          groupId = newUserFeedGroup.Id;
        }

        _subscriptionRepository.Add(newSubscription, groupId.Value);

        Debug.Assert(newSubscription.Id > 0);

        ts.Complete();

        return newSubscription.Id;
      }
    }

    public IEnumerable<GroupedSubscription> GetGroupedSubscriptions(int userAccountId) {
      using (var ts = TransactionUtils.CreateTransactionScope()) {
        IEnumerable<GroupedSubscription> groupedSubscriptions =
          _subscriptionQueryDao.GetGroupedSubscriptions(userAccountId);

        ts.Complete();

        return groupedSubscriptions;
      }
    }

    public IEnumerable<FeedItem> GetFeedItems(int userAccountId, int subscriptionId, bool returnRead) {
      using (var ts = TransactionUtils.CreateTransactionScope()) {
        IEnumerable<FeedItem> feedItems =
          _subscriptionQueryDao.GetFeedItems(userAccountId, subscriptionId, returnRead);

        ts.Complete();

        return feedItems;
      }
    }

    public void MarkAllItemsAsRead(int userAccountId, int subscriptionId) {
      using (var ts = TransactionUtils.CreateTransactionScope()) {
        _subscriptionRepository.MarkAllItemsAsRead(userAccountId, subscriptionId);

        ts.Complete();
      }
    }

  }

}
