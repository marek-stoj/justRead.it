using System;

namespace JustReadIt.Core.Domain {

  public class Feed {

    private DateTime _dateCreated;

    public int Id { get; set; }

    public string Title { get; set; }

    public string FeedUrl { get; set; }
    
    public string SiteUrl { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

  }

}
