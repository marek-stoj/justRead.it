using System.Collections.Generic;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.WebApp.Areas.App.Core.Services {

  public interface IQueryModelToJsonModelMapper {

    SubscriptionsList CreateSubscriptionsList(IEnumerable<QueryModel.GroupedSubscription> subscriptions);

    FeedItem CreateFeedItem(QueryModel.FeedItem feedItem);

  }

}
