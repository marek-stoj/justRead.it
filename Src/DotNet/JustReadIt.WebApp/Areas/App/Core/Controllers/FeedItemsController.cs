using System.Web.Http;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Areas.App.Core.Models.JsonModel;
using JustReadIt.WebApp.Core.Security;

namespace JustReadIt.WebApp.Areas.App.Core.Controllers {

  public class FeedItemsController : AppApiController {

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

      string articleContentHtml =
        _articlesService.GetArticleContentHtml(
          feedItemUrl);

      return
        new FeedItemContent {
          ContentHtml = articleContentHtml,
        };
    }

    [HttpPost]
    public void MarkAsRead(int id) {
      int userAccountId = SecurityUtils.CurrentUserAccountId;

      using (var ts = TransactionUtils.CreateTransactionScope()) {
        _feedItemRepository.MarkRead(userAccountId, new[] { id });

        ts.Complete();
      }
    }

  }

}
