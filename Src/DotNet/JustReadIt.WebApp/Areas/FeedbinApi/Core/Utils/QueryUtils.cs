using System;
using JustReadIt.Core.Domain;
using System.Linq;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Utils {

  public static class QueryUtils {

    private const int _MaxEntriesForGetAllCount = 100;

    public static FeedItemFilter CreateFeedItemQueryParams(out int maxCount, int? feedId, int? perPage = null, int? page = null, string since = null, bool? read = null, bool? starred = null, string ids = null) {
      maxCount =
        perPage.HasValue
          ? Math.Min(_MaxEntriesForGetAllCount, perPage.Value)
          : _MaxEntriesForGetAllCount;

      return
        new FeedItemFilter {
          FeedId = feedId,
          DateCreatedSince =
            !string.IsNullOrEmpty(since)
              ? ModelUtils.ParseFeedbinDateTime(since)
              : null,
          PageNumber = page,
          IsRead = read,
          IsStarred = starred,
          Ids =
            !string.IsNullOrEmpty(ids)
              ? ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(int.Parse)
              : null,
        };
    }

  }

}
