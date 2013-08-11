using Newtonsoft.Json;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class Subscription {

    private string _title;
    private string _siteUrl;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("feedId")]
    public int FeedId { get; set; }

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("siteUrl")]
    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

    [JsonProperty("unreadItemsCount")]
    public int UnreadItemsCount { get; set; }

    [JsonProperty("containsUnreadItems")]
    public bool ContainsUnreadItems {
      get { return UnreadItemsCount > 0; }
    }

  }

}
