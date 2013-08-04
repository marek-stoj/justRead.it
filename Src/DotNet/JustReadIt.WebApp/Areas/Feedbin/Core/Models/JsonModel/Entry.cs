using System;
using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel {

  public class Entry {

    private string _title;
    private string _url;
    private string _author;
    private string _content;
    private string _summary;
    private DateTime? _published;
    private DateTime? _createdAt;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("feed_id")]
    public int FeedId { get; set; }

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("url")]
    public string Url {
      get { return _url; }
      set { _url = value.TrimmedOrNull(); }
    }

    [JsonProperty("author")]
    public string Author {
      get { return _author; }
      set { _author = value.TrimmedOrNull(); }
    }

    [JsonProperty("content")]
    public string Content {
      get { return _content; }
      set { _content = value.TrimmedOrNull(); }
    }

    [JsonProperty("summary")]
    public string Summary {
      get { return _summary; }
      set { _summary = value.TrimmedOrNull(); }
    }

    [JsonProperty("published")]
    public DateTime? Published {
      get { return _published; }
      set { _published = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null; }
    }

    [JsonProperty("created_at")]
    public DateTime? CreatedAt {
      get { return _createdAt; }
      set { _createdAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null; }
    }

  }

}
