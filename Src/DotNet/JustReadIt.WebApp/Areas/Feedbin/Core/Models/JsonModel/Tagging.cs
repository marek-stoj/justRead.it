using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Models.JsonModel {

  public class Tagging {

    private string _name;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("feed_id")]
    public int FeedId { get; set; }

    [JsonProperty("name")]
    public string Name {
      get { return _name; }
      set { _name = value.TrimmedOrNull(); }
    }

  }

}
