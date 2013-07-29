using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class FeedItem {

    private DateTime? _datePublished;
    private string _title;
    private string _url;

    public DateTime? DatePublished {
      get { return _datePublished; }
      set { _datePublished = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null; }
    }

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public string Url {
      get { return _url; }
      set { _url = value.TrimmedOrNull(); }
    }

  }

}
