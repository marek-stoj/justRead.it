using System.Collections.Generic;
using JustReadIt.Core.Common;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class SubscriptionsGroup {

    private string _title;
    private List<Subscription> _subscriptions;

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("subscriptions")]
    public List<Subscription> Subscriptions {
      get { return _subscriptions ?? (_subscriptions = new List<Subscription>()); }
      set { _subscriptions = value; }
    }

  }

}
