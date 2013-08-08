using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class FeedItemContent {

    private string _contentHtml;

    [JsonProperty("contentHtml")]
    public string ContentHtml {
      get { return _contentHtml; }
      set { _contentHtml = value.TrimmedOrNull(); }
    }

  }

}
