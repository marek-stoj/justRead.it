using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Subscriptions {

  public class UpdateInputModel {

    private string _title;

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

  }

}
