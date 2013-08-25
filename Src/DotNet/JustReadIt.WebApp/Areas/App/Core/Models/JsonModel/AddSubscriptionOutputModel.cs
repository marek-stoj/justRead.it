using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class AddSubscriptionOutputModel {

    [JsonProperty("status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public AddSubscriptionResultStatus Status { get; set; }

    [JsonProperty("isUrlValid")]
    public bool IsUrlValid { get; set; }

    [JsonProperty("subscriptionId")]
    public int SubscriptionId { get; set; }

  }

}
