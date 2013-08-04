using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.Feedbin.Core.Models.Taggings;
using JustReadIt.WebApp.Areas.Feedbin.Core.Services;
using JustReadIt.WebApp.Core.Security;
using Tagging = JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel.Tagging;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Controllers {

  public class TaggingsController : FeedbinApiController {

    private readonly ITaggingRepository _taggingRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public TaggingsController(ITaggingRepository taggingRepository, IFeedRepository feedRepository, ISubscriptionRepository subscriptionRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(taggingRepository, "taggingRepository");
      Guard.ArgNotNull(feedRepository, "feedRepository");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _taggingRepository = taggingRepository;
      _feedRepository = feedRepository;
      _subscriptionRepository = subscriptionRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public TaggingsController()
      : this(CommonIoC.GetTaggingRepository(), CommonIoC.GetFeedRepository(), CommonIoC.GetSubscriptionRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    [HttpGet]
    public IEnumerable<Tagging> GetAll() {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      IEnumerable<JustReadIt.Core.Domain.Tagging> taggings;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        taggings = _taggingRepository.GetAll(userAccountId);

        ts.Complete();
      }

      List<Tagging> taggingsModel =
        taggings.Select(_domainToJsonModelMapper.CreateTagging)
          .ToList();

      return taggingsModel;
    }

    [HttpGet]
    public Tagging Get(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      JustReadIt.Core.Domain.Tagging tagging;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        tagging = _taggingRepository.FindById(id);

        ts.Complete();
      }

      if (tagging == null || tagging.UserAccountId != userAccountId) {
        throw HttpForbidden();
      }

      Tagging taggingModel =
        _domainToJsonModelMapper.CreateTagging(tagging);

      return taggingModel;
    }

    [HttpPost]
    public void Create(CreateInputModel input) {
      if (input == null) {
        throw HttpBadRequest();
      }

      if (string.IsNullOrEmpty(input.Name)) {
        throw HttpBadRequest();
      }

      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        string apiTaggingUrl;

        if (!_feedRepository.Exists(input.FeedId)) {
          throw HttpForbidden();
        }

        if (!_subscriptionRepository.IsSubscribedToFeed(userAccountId, input.FeedId)) {
          throw HttpForbidden();
        }

        int? taggingId =
          _taggingRepository.FindIdByFeedId(userAccountId, input.FeedId);

        if (taggingId.HasValue) {
          apiTaggingUrl = Routes.CreateApiUrlForGetTagging(Url, taggingId.Value);

          ts.Complete();

          throw HttpFound(new Dictionary<string, string> { { "Location", apiTaggingUrl }, });
        }

        var tagging =
          new JustReadIt.Core.Domain.Tagging {
            UserAccountId = userAccountId,
            FeedId = input.FeedId,
            Name = input.Name,
          };

        _taggingRepository.Add(tagging);

        Debug.Assert(tagging.Id > 0);

        apiTaggingUrl = Routes.CreateApiUrlForGetTagging(Url, tagging.Id);

        ts.Complete();

        throw HttpCreated(new Dictionary<string, string> { { "Location", apiTaggingUrl }, });
      }
    }

    [HttpDelete]
    public void Delete(int id) {
      // TODO IMM HI: IMPLEMENT!
      throw new NotImplementedException();
    }

  }

}
