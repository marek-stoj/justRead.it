using System;

namespace JustReadIt.Core.Domain {

  public class UserFeedGroupFeed {

    private DateTime _dateCreated;

    public int Id { get; set; }

    public int UserFeedGroupId { get; set; }

    public int FeedId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

  }

}
