using System.Collections.Generic;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Entries {

  public class CreateUnreadInputModel {

    [JsonProperty("unread_entries")]
    public List<int> UnreadEntries { get; set; }

  }

}
