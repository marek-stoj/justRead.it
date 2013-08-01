using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Core.App;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class FeedsController : FeedbinApiController {

    private readonly IFeedRepository _feedRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public FeedsController(IFeedRepository feedRepository, ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(feedRepository, "feedRepository");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _feedRepository = feedRepository;
      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public FeedsController()
      : this(CommonIoC.GetFeedRepository(), CommonIoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    /// <remarks>
    /// This behaves differently than in the spec.
    /// We won't return feeds with just any id - only those that are subscribed to by the current user.
    /// </remarks>
    [HttpGet]
    public JsonModel.Feed Get(int id) {
      int userAccountId = CurrentUserAccountId;
      Feed feed;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        feed = _feedRepository.FindById(id);

        if (feed == null) {
          throw HttpNotFound();
        }

        if (!_subscriptionRepository.IsSubscribedToFeed(userAccountId, id)) {
          ts.Complete();

          throw HttpNotFound();
        }

        ts.Complete();
      }

      JsonModel.Feed feedModel =
        _domainToJsonModelMapper.CreateFeed(feed);

      return feedModel;
    }

  }

}
