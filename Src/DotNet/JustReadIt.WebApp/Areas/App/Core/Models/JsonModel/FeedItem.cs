using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class FeedItem {

    private string _title;
    private string _summary;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("feedId")]
    public int FeedId { get; set; }

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("summary")]
    public string Summary {
      get { return _summary; }
      set { _summary = value.TrimmedOrNull(); }
    }

    [JsonProperty("isRead")]
    public bool IsRead { get; set; }

  }

}
