using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Services;
using JustReadIt.WebApp.Core.App;
using JsonModel = JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class SubscriptionsController : FeedbinApiController {

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public SubscriptionsController(ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public SubscriptionsController()
      : this(IoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    [HttpGet]
    public IEnumerable<JsonModel.Subscription> GetAll(string since = null) {
      string link = Url.Link("FeedbinApi_Subscriptions_Get", new { id = 886 });

      // TODO IMM HI: remove
      int userAccountId = CurrentUserAccountId;

      DateTime? sinceDate =
        !string.IsNullOrEmpty(since)
          ? ParseFeedbinDateTime(since)
          : null;

      IEnumerable<Subscription> subscriptions =
        _subscriptionRepository.GetAll(userAccountId, sinceDate);

      List<JsonModel.Subscription> subscriptionsModel =
        subscriptions.Select(_domainToJsonModelMapper.CreateSubscription)
          .ToList();

      return subscriptionsModel;
    }

    [HttpGet]
    public JsonModel.Subscription Get(int id) {
      int userAccountId = CurrentUserAccountId;

      Subscription subscription =
        _subscriptionRepository.FindById(userAccountId, id);

      if (subscription == null) {
        throw HttpNotFound();
      }

      JsonModel.Subscription subscriptionModel =
        _domainToJsonModelMapper.CreateSubscription(subscription);

      return subscriptionModel;
    }

  }

}
