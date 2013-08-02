using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Taggings {

  public class CreateInputModel {

    private string _name;

    [JsonProperty("feed_id")]
    public int FeedId { get; set; }


    [JsonProperty("name")]
    public string Name {
      get { return _name; }
      set { _name = value.TrimmedOrNull(); }
    }

  }

}
