using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class AddSubscriptionInputModel {

    private string _url;

    [JsonProperty("url")]
    public string Url {
      get { return _url; }
      set { _url = value.TrimmedOrNull(); }
    }

  }

}
