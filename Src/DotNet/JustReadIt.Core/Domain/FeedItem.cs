using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class FeedItem {

    private DateTime _dateCreated;
    private string _title;
    private string _url;
    private DateTime? _datePublished;
    private string _author;
    private string _content;
    private string _summary;

    public int Id { get; set; }

    public int FeedId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public string Url {
      get { return _url; }
      set { _url = value.TrimmedOrNull(); }
    }

    public DateTime? DatePublished {
      get { return _datePublished; }
      set { _datePublished = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null; }
    }

    public string Author {
      get { return _author; }
      set { _author = value.TrimmedOrNull(); }
    }

    public string Summary {
      get { return _summary; }
      set { _summary = value.TrimmedOrNull(); }
    }

    public string Content {
      get { return _content; }
      set { _content = value.TrimmedOrNull(); }
    }

  }

}
