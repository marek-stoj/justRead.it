using System.Collections.Generic;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.App.Core.Models.JsonModel {

  public class FeedItemsList {

    private List<FeedItem> _items;

    [JsonProperty("items")]
    public List<FeedItem> Items {
      get { return _items ?? (_items = new List<FeedItem>()); }
      set { _items = value; }
    }

  }

}
