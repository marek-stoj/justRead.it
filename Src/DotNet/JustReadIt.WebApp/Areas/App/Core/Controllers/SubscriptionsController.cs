using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Query;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using JustReadIt.WebApp.Areas.App.Core.Services;
using JustReadIt.WebApp.Core.Security;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

// TODO IMM HI: get rid of flickering when changing feed
namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  public class SubscriptionsController : AppApiController {

    private readonly ISubscriptionsQueryDao _subscriptionsQueryDao;
    private readonly IQueryModelToJsonModelMapper _queryModelToJsonModelMapper;

    public SubscriptionsController(ISubscriptionsQueryDao subscriptionsQueryDao, IQueryModelToJsonModelMapper queryModelToJsonModelMapper) {
      Guard.ArgNotNull(subscriptionsQueryDao, "_subscriptionsQueryDao");
      Guard.ArgNotNull(queryModelToJsonModelMapper, "QueryModelToJsonModelMapper");

      _subscriptionsQueryDao = subscriptionsQueryDao;
      _queryModelToJsonModelMapper = queryModelToJsonModelMapper;
    }

    public SubscriptionsController()
      : this(CommonIoC.GetSubscriptionsQueryDao(), IoC.GetQueryModelToJsonModelMapper()) {
    }

    [HttpGet]
    public SubscriptionsList GetSubscriptionsList() {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.GroupedSubscription> subscriptions;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionsQueryDao.GetGroupedSubscriptions(userAccountId);

        ts.Complete();
      }

      SubscriptionsList subscriptionsList =
        _queryModelToJsonModelMapper.CreateSubscriptionsList(subscriptions);

      return subscriptionsList;
    }

    [HttpGet]
    public FeedItemsList GetItems(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.FeedItem> feedItems;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        feedItems = _subscriptionsQueryDao.GetFeedItems(userAccountId, id);

        ts.Complete();
      }

      var feedItemsList =
        new FeedItemsList {
          Items = feedItems.Select(_queryModelToJsonModelMapper.CreateFeedItem).ToList(),
        };

      return feedItemsList;
    }

  }

}
