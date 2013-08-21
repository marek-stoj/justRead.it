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
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using JustReadIt.WebApp.Areas.App.Core.Services;
using JustReadIt.WebApp.Core.Security;
using log4net;
using QueryModel = JustReadIt.Core.Domain.Query.Model;
using JustReadIt.Core.Common.Logging;

// TODO IMM HI: get rid of flickering when changing feed

namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  public class SubscriptionsController : AppApiController {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly ISubscriptionQueryDao _subscriptionQueryDao;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IQueryModelToJsonModelMapper _queryModelToJsonModelMapper;
    private readonly IOpmlImporter _opmlImporter;

    public SubscriptionsController(ISubscriptionQueryDao subscriptionQueryDao, ISubscriptionRepository subscriptionRepository, IQueryModelToJsonModelMapper queryModelToJsonModelMapper, IOpmlImporter opmlImporter) {
      Guard.ArgNotNull(subscriptionQueryDao, "subscriptionQueryDao");
      Guard.ArgNotNull(subscriptionRepository, "subscriptionRepository");
      Guard.ArgNotNull(queryModelToJsonModelMapper, "QueryModelToJsonModelMapper");
      Guard.ArgNotNull(opmlImporter, "opmlImporter");

      _subscriptionQueryDao = subscriptionQueryDao;
      _subscriptionRepository = subscriptionRepository;
      _queryModelToJsonModelMapper = queryModelToJsonModelMapper;
      _opmlImporter = opmlImporter;
    }

    public SubscriptionsController()
      : this(CommonIoC.GetSubscriptionQueryDao(), CommonIoC.GetSubscriptionRepository(), IoC.GetQueryModelToJsonModelMapper(), CommonIoC.GetOpmlImporter()) {
    }

    [HttpGet]
    public SubscriptionsList GetSubscriptionsList() {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.GroupedSubscription> subscriptions;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        subscriptions = _subscriptionQueryDao.GetGroupedSubscriptions(userAccountId);

        ts.Complete();
      }

      SubscriptionsList subscriptionsList =
        _queryModelToJsonModelMapper.CreateSubscriptionsList(subscriptions);

      return subscriptionsList;
    }

    [HttpGet]
    public FeedItemsList GetItems(int id, bool returnRead) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;
      IEnumerable<QueryModel.FeedItem> feedItems;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        feedItems = _subscriptionQueryDao.GetFeedItems(userAccountId, id, returnRead);

        ts.Complete();
      }

      var feedItemsList =
        new FeedItemsList {
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
    public AddSubscriptionOutputModel Add(AddSubscriptionInputModel inputModel) {
      Uri feedUri;

      if (!Uri.TryCreate(inputModel.Url, UriKind.Absolute, out feedUri)
       || (!feedUri.Scheme.EqualsOrdinalIgnoreCase("http") && !feedUri.Scheme.EqualsOrdinalIgnoreCase("https"))) {
        return
          new AddSubscriptionOutputModel {
            IsUrlValid = false,
          };
      }

      return
        new AddSubscriptionOutputModel {
          IsUrlValid = true,
        };
    }

    [HttpPost]
    public HttpResponseMessage Import() {
      HttpFileCollection uploadedFiles = HttpContext.Current.Request.Files;
      HttpPostedFile uploadedFile = uploadedFiles.Count > 0 ? uploadedFiles[0] : null;

      if (uploadedFile == null
       || uploadedFile.ContentLength == 0
       || uploadedFile.FileName.IsNullOrEmpty()) {
        return PlainJson(new ImportSubscriptionsResult { Status = ImportSubscriptionsResultStatus.Failed_NoFileUploaded, });
      }

      string fileExtension = Path.GetExtension(uploadedFile.FileName);

      if (!".opml".EqualsOrdinalIgnoreCase(fileExtension)
       && !".xml".EqualsOrdinalIgnoreCase(fileExtension)) {
        return PlainJson(new ImportSubscriptionsResult { Status = ImportSubscriptionsResultStatus.Failed_UnsupportedFileExtension, });
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

        return PlainJson(new ImportSubscriptionsResult { Status = ImportSubscriptionsResultStatus.Failed_UnknownError, });
      }

      return PlainJson(new ImportSubscriptionsResult { Status = ImportSubscriptionsResultStatus.Success, });
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
