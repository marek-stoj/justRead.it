using Newtonsoft.Json;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel {

  public class Feed {

    private string _title;
    private string _feedUrl;
    private string _siteUrl;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("feed_url")]
    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

    [JsonProperty("site_url")]
    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

  }

}
