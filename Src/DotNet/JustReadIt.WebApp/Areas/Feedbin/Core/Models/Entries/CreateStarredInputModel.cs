using System.Collections.Generic;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Models.Entries {

  public class CreateStarredInputModel {

    [JsonProperty("starred_entries")]
    public List<int> StarredEntries { get; set; }

  }

}
