﻿using System;
using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.Resources;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using QueryModel = JustReadIt.Core.Domain.Query.Model;

namespace JustReadIt.WebApp.Areas.App.Core.Services {

  public class QueryModelToJsonModelMapper : IQueryModelToJsonModelMapper {

    private const int _MaxSummaryLength = 256;

    public SubscriptionsList CreateSubscriptionsList(IEnumerable<QueryModel.GroupedSubscription> subscriptions) {
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
          Title = feedItem.Title,
          Date = feedItem.Date.ToShortDateString(),
          Summary = summary,
        };
    }

  }

}