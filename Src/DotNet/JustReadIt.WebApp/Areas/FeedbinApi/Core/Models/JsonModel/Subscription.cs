using System;
using Newtonsoft.Json;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel {

  public class Subscription {

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("feed_id")]
    public int FeedId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("feed_url")]
    public string FeedUrl { get; set; }

    [JsonProperty("site_url")]
    public string SiteUrl { get; set; }

  }

}
