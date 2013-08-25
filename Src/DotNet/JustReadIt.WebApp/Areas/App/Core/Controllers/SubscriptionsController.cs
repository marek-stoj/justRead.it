using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Query;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Resources;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Services;
using JustReadIt.WebApp.Core.Security;
using log4net;
using JustReadIt.Core.Common.Logging;
using JsonModel = JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using QueryModel = JustReadIt.Core.Domain.Query.Model;
using DomainModel = JustReadIt.Core.Domain;

// TODO IMM HI: get rid of flickering when changing feed

namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  public class SubscriptionsController : AppApiController {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    // TODO IMM HI: xxx refactor into SubscriptionsService
    private readonly ISubscriptionQueryDao _subscriptionQueryDao;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IQueryModelToJsonModelMapper _queryModelToJsonModelMapper;
    private readonly IOpmlImporter _opmlImporter;
    private readonly ISubscriptionsService _subscriptionsService;

    public SubscriptionsController(ISubscriptionQueryDao subscriptionQueryDao, ISubscriptionRepository subscriptionRepository, IQueryModelToJsonModelMapper queryModelToJsonModelMapper, IOpmlImporter opmlImporter, ISubscriptionsService subscriptionsService) {
      Guard.ArgNotNull(subscriptionQueryDao, "subscriptionQueryDao");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(queryModelToJsonModelMapper, "QueryModelToJsonModelMapper");
      Guard.ArgNotNull(opmlImporter, "opmlImporter");
      Guard.ArgNotNull(subscriptionsService, "subscriptionsService");

      _subscriptionQueryDao = subscriptionQueryDao;
      _subscriptionRepository = subscriptionRepository;
      _queryModelToJsonModelMapper = queryModelToJsonModelMapper;
      _opmlImporter = opmlImporter;
      _subscriptionsService = subscriptionsService;
    }

    public SubscriptionsController()
      : this(CommonIoC.GetSubscriptionQueryDao(), CommonIoC.GetSubscriptionRepository(), IoC.GetQueryModelToJsonModelMapper(), CommonIoC.GetOpmlImporter(), CommonIoC.GetSubscriptionsService()) {
    }

    [HttpGet]
    public JsonModel.SubscriptionsList GetSubscriptionsList() {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.GroupedSubscription> subscriptions;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionQueryDao.GetGroupedSubscriptions(userAccountId);

        ts.Complete();
      }

      JsonModel.SubscriptionsList subscriptionsList =
        _queryModelToJsonModelMapper.CreateSubscriptionsList(subscriptions);

      if (subscriptionsList.Groups.Count == 0) {
        subscriptionsList.Groups.Add(
          new JsonModel.SubscriptionsGroup {
            Title = CommonResources.UncategorizedFeedGroupTitle,
          });
      }

      return subscriptionsList;
    }

    [HttpGet]
    public JsonModel.FeedItemsList GetItems(int id, bool returnRead) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.FeedItem> feedItems;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        feedItems = _subscriptionQueryDao.GetFeedItems(userAccountId, id, returnRead);

        ts.Complete();
      }

      var feedItemsList =
        new JsonModel.FeedItemsList {
          Items = feedItems.Select(_queryModelToJsonModelMapper.CreateFeedItem).ToList(),
        };

      return feedItemsList;
    }

    [HttpPost]
    public HttpResponseMessage MarkAllItemsAsRead(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        _subscriptionRepository.MarkAllItemsAsRead(userAccountId, id);

        ts.Complete();
      }

      throw HttpOk();
    }

    [HttpPost]
    public JsonModel.AddSubscriptionOutputModel Add(JsonModel.AddSubscriptionInputModel inputModel) {
      Uri feedUri;

      if (!Uri.TryCreate(inputModel.Url, UriKind.Absolute, out feedUri)
       || (!feedUri.Scheme.EqualsOrdinalIgnoreCase("http") && !feedUri.Scheme.EqualsOrdinalIgnoreCase("https"))) {
        return
          new JsonModel.AddSubscriptionOutputModel {
            Status = JsonModel.AddSubscriptionResultStatus.Failed_InvalidInputData,
            IsUrlValid = false,
          };
      }

      int userAccountId = SecurityUtils.CurrentUserAccountId;

      // TODO IMM HI: xxx handle errors
      // TODO IMM HI: xxx crawl feed when adding a new one
      int subscriptionId =
        _subscriptionsService.Subscribe(
          userAccountId,
          inputModel.Url,
          inputModel.Category);

      return
        new JsonModel.AddSubscriptionOutputModel {
          Status = JsonModel.AddSubscriptionResultStatus.Success,
          IsUrlValid = true,
          SubscriptionId = subscriptionId,
        };
    }

    [HttpPost]
    public HttpResponseMessage Import() {
      HttpFileCollection uploadedFiles = HttpContext.Current.Request.Files;
      HttpPostedFile uploadedFile = uploadedFiles.Count > 0 ? uploadedFiles[0] : null;

      if (uploadedFile == null
       || uploadedFile.ContentLength == 0
       || uploadedFile.FileName.IsNullOrEmpty()) {
         return PlainJson(new JsonModel.ImportSubscriptionsOutputModel { Status = JsonModel.ImportSubscriptionsResultStatus.Failed_NoFileUploaded, });
      }

      string fileExtension = Path.GetExtension(uploadedFile.FileName);

      if (!".opml".EqualsOrdinalIgnoreCase(fileExtension)
       && !".xml".EqualsOrdinalIgnoreCase(fileExtension)) {
         return PlainJson(new JsonModel.ImportSubscriptionsOutputModel { Status = JsonModel.ImportSubscriptionsResultStatus.Failed_UnsupportedFileExtension, });
      }

      string opmlXml;

      using (var sr = new StreamReader(uploadedFile.InputStream)) {
        opmlXml = sr.ReadToEnd();
      }

      int userAccountId = SecurityUtils.CurrentUserAccountId;

      try {
        _opmlImporter.Import(opmlXml, userAccountId);
      }
      catch (Exception exc) {
        _log.ErrorIfEnabled(() => "Unhandled exception during import.", exc);

        return PlainJson(new JsonModel.ImportSubscriptionsOutputModel { Status = JsonModel.ImportSubscriptionsResultStatus.Failed_UnknownError, });
      }

      return PlainJson(new JsonModel.ImportSubscriptionsOutputModel { Status = JsonModel.ImportSubscriptionsResultStatus.Success, });
    }

    /// <remarks>
    /// Needed for IE because it tries to download the response when it's type is application/json (sic!).
    /// </remarks>
    private HttpResponseMessage PlainJson<T>(T value) {
      HttpResponseMessage response =
        Request.CreateResponse(HttpStatusCode.OK, value);

      response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

      return response;
    }

  }

}
