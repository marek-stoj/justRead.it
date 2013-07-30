using JustReadIt.Core.Domain;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Services {

  public interface IDomainToJsonModelMapper {

    JsonModel.Subscription CreateSubscription(Subscription subscription);

    JsonModel.Feed CreateFeed(Feed feed);

  }

}
