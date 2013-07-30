using JustReadIt.Core.Common;
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

  }

}
