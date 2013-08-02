using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain {

  public class FeedItemFilter {

    public int? FeedId { get; set; }

    public DateTime? DateCreatedSince { get; set; }

    public int? PageNumber { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsStarred { get; set; }

    public IEnumerable<int> Ids { get; set; }

  }

}
