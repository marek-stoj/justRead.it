using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel;
using Feed = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Feed;
using Subscription = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Subscription;
using Tagging = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Tagging;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Services {

  public class DomainToJsonModelMapper : IDomainToJsonModelMapper {

    public Subscription CreateSubscription(JustReadIt.Core.Domain.Subscription subscription) {
      Guard.ArgNotNull(subscription, "subscription");

      return
        new Subscription {
          Id = subscription.Id,
          FeedId = subscription.Feed.Id,
          CreatedAt = subscription.DateCreated,
          Title = subscription.Title,
          FeedUrl = subscription.Feed.FeedUrl,
          SiteUrl = subscription.Feed.SiteUrl,
        };
    }

    public Feed CreateFeed(JustReadIt.Core.Domain.Feed feed) {
      Guard.ArgNotNull(feed, "feed");

      return
        new Feed {
          Id = feed.Id,
          Title = feed.Title,
          FeedUrl = feed.FeedUrl,
          SiteUrl = feed.SiteUrl,
        };
    }

    public Entry CreateEntry(FeedItem feedItem) {
      Guard.ArgNotNull(feedItem, "feedItem");

      return
        new Entry {
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

    public Tagging CreateTagging(JustReadIt.Core.Domain.Tagging tagging) {
      return
        new Tagging {
          Id = tagging.Id,
          FeedId = tagging.FeedId,
          Name = tagging.Name,
        };
    }

  }

}
