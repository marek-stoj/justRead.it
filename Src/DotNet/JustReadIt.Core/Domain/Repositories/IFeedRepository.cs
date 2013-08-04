﻿using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedRepository {

    IEnumerable<Feed> GetFeedsToCrawl(DateTime maxDateLastCrawlStarted);

    bool Exists(int id);

    Feed FindById(int id);

    void Add(Feed feed);

    int? FindFeedId(string feedUrl);

    bool SetDateLastCrawlStarted(int id, DateTime dateTime);

  }

}
