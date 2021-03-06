﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class ImportSubscriptionsOutputModel {

    [JsonProperty("status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public ImportSubscriptionsResultStatus Status { get; set; }

  }

}
