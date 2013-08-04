using JustReadIt.Core.Domain;
using JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel;
using Feed = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Feed;
using Subscription = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Subscription;
using Tagging = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Tagging;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Services {

  public interface IDomainToJsonModelMapper {

    Subscription CreateSubscription(JustReadIt.Core.Domain.Subscription subscription);

    Feed CreateFeed(JustReadIt.Core.Domain.Feed feed);

    Entry CreateEntry(FeedItem feedItem);

    Tagging CreateTagging(JustReadIt.Core.Domain.Tagging tagging);

  }

}
