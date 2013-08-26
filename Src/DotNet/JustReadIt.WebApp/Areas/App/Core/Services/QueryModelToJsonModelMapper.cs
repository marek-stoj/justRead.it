using System;
using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.Resources;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.WebApp.Areas.App.Core.Services {

  public class QueryModelToJsonModelMapper : IQueryModelToJsonModelMapper {

    private const int _MaxSummaryLength = 256;

    public SubscriptionsList CreateSubscriptionsList(IEnumerable<QueryModel.GroupedSubscription> groupedSubscriptions) {
      if (groupedSubscriptions == null) {
        throw new ArgumentNullException("groupedSubscriptions");
      }

      var subscriptionGroups =
        (
          from s in groupedSubscriptions
          group s by new Tuple<int, string>(s.GroupId, s.GroupTitle)
          into g
          select
            new SubscriptionsGroup {
              Id = g.Key.Item1,
              Title = g.Key.Item2,
              Subscriptions =
                g
                .Where(gs => gs.SubscriptionInfo != null)
                .Select(
                  gs =>
                  new Subscription {
                    Id = gs.SubscriptionInfo.Id,
                    FeedId = gs.SubscriptionInfo.FeedId,
                    Title = gs.SubscriptionInfo.Title,
                    SiteUrl = gs.SubscriptionInfo.SiteUrl,
                    UnreadItemsCount = gs.SubscriptionInfo.UnreadItemsCount,
                  }).ToList(),
            }
        );

      return
        new SubscriptionsList {
          Groups = subscriptionGroups.ToList(),
        };
    }

    public FeedItem CreateFeedItem(QueryModel.FeedItem feedItem) {
      string summary = feedItem.Summary ?? "";

      if (!string.IsNullOrEmpty(summary)) {
        summary = summary.StripHtml();

        if (summary.Length > _MaxSummaryLength) {
          summary = summary.Substring(0, _MaxSummaryLength);
        }

        summary += " …";
      }
      else {
        summary = CommonResources.NoFeedItemSummary;
      }

      return
        new FeedItem {
          Id = feedItem.Id,
          FeedId = feedItem.FeedId,
          Title = feedItem.Title,
          Url = feedItem.Url,
          Date = feedItem.Date.ToShortDateString(),
          Summary = summary,
          IsRead = feedItem.IsRead,
        };
    }

  }

}
