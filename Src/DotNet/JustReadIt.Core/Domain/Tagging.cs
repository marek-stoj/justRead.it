using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class Tagging {

    private DateTime _dateCreated;
    private string _name;

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public int FeedId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string Name {
      get { return _name; }
      set { _name = value.TrimmedOrNull(); }
    }

  }

}
