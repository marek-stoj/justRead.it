using System;
using System.Collections.Generic;
using System.Linq;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.WebApp.Areas.App.Core.Services {

  public class QueryModelToJsonModelMapper : IQueryModelToJsonModelMapper {

    public SubscriptionsList CreateSubscriptionsList(IEnumerable<QueryModel.Subscription> subscriptions) {
      if (subscriptions == null) {
        throw new ArgumentNullException("subscriptions");
      }

      var subscriptionGroups =
        (
          from s in subscriptions
          group s by s.GroupTitle
          into g
          select
            new SubscriptionsGroup {
              Title = g.Key,
              Subscriptions =
                g.Select(
                  gs =>
                  new Subscription {
                    Id = gs.Id,
                    FeedId = gs.FeedId,
                    Title = gs.Title,
                    SiteUrl = gs.SiteUrl,
                  }).ToList(),
            }
        );

      return
        new SubscriptionsList {
          Groups = subscriptionGroups.ToList(),
        };
    }

  }

}
