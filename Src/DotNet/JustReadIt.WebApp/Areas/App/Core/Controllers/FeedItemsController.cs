using System;
using System.Reflection;
using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using JustReadIt.WebApp.Core.Resources;
using JustReadIt.WebApp.Core.Security;
using log4net;
using JustReadIt.Core.Common.Logging;

namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  public class FeedItemsController : AppApiController {

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IFeedItemRepository _feedItemRepository;
    private readonly IArticlesService _articlesService;

    public FeedItemsController(IFeedItemRepository feedItemRepository, IArticlesService articlesService) {
      Guard.ArgNotNull(feedItemRepository, "feedItemRepository");
      Guard.ArgNotNull(articlesService, "articlesService");

      _feedItemRepository = feedItemRepository;
      _articlesService = articlesService;
    }

    public FeedItemsController()
      : this(CommonIoC.GetFeedItemRepository(), CommonIoC.GetArticlesService()) {
    }

    [HttpGet]
    public FeedItemContent GetFeedItemContent(int id) {
      string feedItemUrl;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        feedItemUrl = _feedItemRepository.FindUrlById(id);

        if (string.IsNullOrEmpty(feedItemUrl)) {
          throw HttpNotFound();
        }

        ts.Complete();
      }

      string contentHtml;

      try {
        contentHtml =
          _articlesService.GetArticleContentHtml(feedItemUrl);
      }
      catch (Exception exc) {
        _log.ErrorIfEnabled(() => "Error while getting article content HTML - returning generic message to the user.", exc);

        contentHtml = null;
      }

      if (string.IsNullOrEmpty(contentHtml)) {
        contentHtml =
          CommonResources.CouldntLoadArticleContentHtmlTemplate
            .Replace("${originalUrl}", feedItemUrl);
      }

      return
        new FeedItemContent {
          ContentHtml = contentHtml,
        };
    }

    [HttpPost]
    public void ToggleIsRead(int id, bool isRead) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        if (isRead) {
          _feedItemRepository.MarkRead(userAccountId, new[] { id });
        }
        else {
          _feedItemRepository.MarkUnread(userAccountId, new[] { id });
        }

        ts.Complete();
      }
    }

  }

}
