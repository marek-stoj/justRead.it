using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Models.Subscriptions {

  public class CreateInputModel {

    private string _feedUrl;

    [JsonProperty("feed_url")]
    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

  }

}
