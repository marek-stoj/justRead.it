﻿using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Services.Feeds {

  public class Feed {

    private string _title;
    private string _feedUrl;
    private string _siteUrl;
    private IEnumerable<FeedItem> _items;

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public string FeedUrl {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

    public IEnumerable<FeedItem> Items {
      get { return _items ?? Enumerable.Empty<FeedItem>(); }
      set { _items = value; }
    }

  }

}
