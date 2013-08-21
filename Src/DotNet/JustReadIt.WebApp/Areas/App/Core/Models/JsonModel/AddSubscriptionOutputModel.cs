using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class AddSubscriptionOutputModel {

    [JsonProperty("isUrlValid")]
    public bool IsUrlValid { get; set; }

  }

}
