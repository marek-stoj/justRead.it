using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class AddSubscriptionInputModel {

    private string _url;
    private string _category;

    [JsonProperty("url")]
    public string Url {
      get { return _url; }
      set { _url = value.TrimmedOrNull(); }
    }

    [JsonProperty("category")]
    public string Category {
      get { return _category; }
      set { _category = value.TrimmedOrNull(); }
    }

  }

}
