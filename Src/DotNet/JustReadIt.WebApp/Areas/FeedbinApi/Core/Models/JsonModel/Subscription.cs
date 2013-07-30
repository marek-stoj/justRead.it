﻿using System;
using Newtonsoft.Json;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.JsonModel {

  public class Subscription {

    private string _title;
    private string _feedUrl;
    private string _siteUrl;

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("feed_id")]
    public int FeedId { get; set; }

    [JsonProperty("title")]
    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    [JsonProperty("feed_url")]
    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

    [JsonProperty("site_url")]
    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

  }

}
