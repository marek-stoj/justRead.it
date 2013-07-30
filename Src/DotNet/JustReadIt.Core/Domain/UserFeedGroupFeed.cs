using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class UserFeedGroupFeed {

    private DateTime _dateCreated;
    private string _customTitle;

    public int Id { get; set; }

    public int UserFeedGroupId { get; set; }

    public int FeedId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    /// <summary>
    /// Can be null.
    /// </summary>
    public string CustomTitle {
      get { return _customTitle; }
      set { _customTitle = value.TrimmedOrNull(); }
    }

  }

}
