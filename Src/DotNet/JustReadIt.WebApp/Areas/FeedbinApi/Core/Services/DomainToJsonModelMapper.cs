﻿using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Services {

  public class DomainToJsonModelMapper : IDomainToJsonModelMapper {

    public JsonModel.Subscription CreateSubscription(Subscription subscription) {
      Guard.ArgNotNull(subscription, "subscription");

      return
        new JsonModel.Subscription {
          Id = subscription.Id,
          FeedId = subscription.Feed.Id,
          CreatedAt = subscription.DateCreated,
          Title = subscription.Title,
          FeedUrl = subscription.Feed.FeedUrl,
          SiteUrl = subscription.Feed.SiteUrl,
        };
    }

    public JsonModel.Feed CreateFeed(Feed feed) {
      Guard.ArgNotNull(feed, "feed");

      return
        new JsonModel.Feed {
          Id = feed.Id,
          Title = feed.Title,
          FeedUrl = feed.FeedUrl,
          SiteUrl = feed.SiteUrl,
        };
    }

    public JsonModel.Entry CreateEntry(FeedItem feedItem) {
      Guard.ArgNotNull(feedItem, "feedItem");

      return
        new JsonModel.Entry {
          Id = feedItem.Id,
          FeedId = feedItem.FeedId,
          Title = feedItem.Title,
          Url = feedItem.Url,
          Author = feedItem.Author,
          Content = feedItem.Content,
          Summary = feedItem.Summary,
          Published = feedItem.DatePublished,
          CreatedAt = feedItem.DateCreated,
        };
    }

    public JsonModel.Tagging CreateTagging(Tagging tagging) {
      return
        new JsonModel.Tagging {
          Id = tagging.Id,
          FeedId = tagging.FeedId,
          Name = tagging.Name,
        };
    }

  }

}
