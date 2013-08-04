using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain.Query.Model {

  public class FeedItem {

    private string _title;
    private DateTime _date;
    private string _summary;

    public int Id { get; set; }

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public DateTime Date {
      get { return _date; }
      set { _date = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string Summary {
      get { return _summary; }
      set { _summary = value.TrimmedOrNull(); }
    }

  }

}
