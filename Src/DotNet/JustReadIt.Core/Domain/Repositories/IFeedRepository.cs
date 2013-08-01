using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedRepository {

    IEnumerable<Feed> GetAll();

    IEnumerable<Feed> GetFeedsToCrawl(int maxCount, DateTime maxDateLastCrawlStarted);

    Feed FindById(int id);

    void Add(Feed feed);

    int? FindFeedId(string feedUrl);

    bool SetDateLastCrawlStarted(int id, DateTime dateTime);

  }

}
