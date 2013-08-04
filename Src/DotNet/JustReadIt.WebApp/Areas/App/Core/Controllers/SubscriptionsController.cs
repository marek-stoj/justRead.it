using System.Collections.Generic;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Query;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using JustReadIt.WebApp.Areas.App.Core.Services;
using JustReadIt.WebApp.Core.Security;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

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
      IEnumerable<QueryModel.Subscription> subscriptions;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionsQueryDao.GetAll(userAccountId);

        ts.Complete();
      }

      SubscriptionsList subscriptionsList =
        _queryModelToJsonModelMapper.CreateSubscriptionsList(subscriptions);

      return subscriptionsList;
    }


  }

}
