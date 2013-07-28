using JustReadIt.Core.Domain;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Services {

  public class DomainToJsonModelMapper : IDomainToJsonModelMapper {

    public JsonModel.Subscription CreateSubscription(Subscription subscription) {
      return
        new JsonModel.Subscription {
          Id = subscription.Id,
          FeedId = subscription.Feed.Id,
          CreatedAt = subscription.DateCreated,
          Title = subscription.Feed.Title,
          FeedUrl = subscription.Feed.FeedUrl,
          SiteUrl = subscription.Feed.SiteUrl,
        };
    }

  }

}
