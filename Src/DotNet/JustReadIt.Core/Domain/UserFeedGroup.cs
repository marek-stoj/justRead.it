using System;

namespace JustReadIt.Core.Domain {

  public class UserFeedGroup {

    private DateTime _dateCreated;

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public SpecialUserFeedGroupType? SpecialType { get; set; }

    public string Title { get; set; }

  }

}
