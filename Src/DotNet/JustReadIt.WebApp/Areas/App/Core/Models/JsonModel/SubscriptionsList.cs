using System.Collections.Generic;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class SubscriptionsList {

    private List<SubscriptionsGroup> _groups;

    [JsonProperty("groups")]
    public List<SubscriptionsGroup> Groups {
      get { return _groups ?? (_groups = new List<SubscriptionsGroup>()); }
      set { _groups = value; }
    }

  }

}
