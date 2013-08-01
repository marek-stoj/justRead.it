using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class Feed {

    private DateTime _dateCreated;
    private string _title;
    private string _feedUrl;
    private string _siteUrl;

    public int Id { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

  }

}
