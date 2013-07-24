using System;
using System.Collections.Generic;
using System.Web.Http;
using JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class SubscriptionsController : FeedbinApiController {

    [HttpGet]
    public IEnumerable<Subscription> GetAll(string since = null) {
      DateTime? sinceDate =
        !string.IsNullOrEmpty(since)
          ? ParseFeedbinDateTime(since)
          : null;

      var subscriptions =
        new List<Subscription> {
          new Subscription {
            Id = 525,
            CreatedAt = sinceDate.HasValue ? sinceDate.Value : DateTime.UtcNow, // TODO IMM HI: 
            FeedId = 47,
            Title = "Daring Fireball",
            FeedUrl = "http://daringfireball.net/index.xml",
            SiteUrl = "http://daringfireball.net/",
          },
        };

      return subscriptions;
    }

  }

}
