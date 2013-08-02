using System.Collections.Generic;
using System.Linq;
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

  public class TaggingsController : FeedbinApiController {

    private readonly ITaggingRepository _taggingRepository;
    private readonly IDomainToJsonModelMapper _domainToJsonModelMapper;

    public TaggingsController(ITaggingRepository taggingRepository, IDomainToJsonModelMapper domainToJsonModelMapper) {
      Guard.ArgNotNull(taggingRepository, "taggingRepository");
      Guard.ArgNotNull(domainToJsonModelMapper, "domainToJsonModelMapper");

      _taggingRepository = taggingRepository;
      _domainToJsonModelMapper = domainToJsonModelMapper;
    }

    public TaggingsController()
      : this(CommonIoC.GetTaggingRepository(), IoC.GetDomainToJsonModelMapper()) {
    }

    [HttpGet]
    public IEnumerable<JsonModel.Tagging> GetAll() {
      int userAccountId = CurrentUserAccountId;

      IEnumerable<Tagging> taggings;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        taggings = _taggingRepository.GetAll(userAccountId);

        ts.Complete();
      }

      List<JsonModel.Tagging> taggingsModel =
        taggings.Select(_domainToJsonModelMapper.CreateTagging)
          .ToList();

      return taggingsModel;
    }

    [HttpGet]
    public JsonModel.Tagging Get(int id) {
      int userAccountId = CurrentUserAccountId;
      Tagging tagging;

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        tagging = _taggingRepository.FindById(id);

        ts.Complete();
      }

      if (tagging == null || tagging.UserAccountId != userAccountId) {
        throw HttpForbidden();
      }

      JsonModel.Tagging taggingModel =
        _domainToJsonModelMapper.CreateTagging(tagging);

      return taggingModel;
    }

  }

}
