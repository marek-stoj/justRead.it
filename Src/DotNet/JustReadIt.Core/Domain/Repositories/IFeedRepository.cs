using System;
using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedRepository {

    IEnumerable<Feed> GetFeedsToCrawl(DateTime maxDateLastCrawlStarted);

    bool Exists(int id);

    Feed FindById(int id);

    Feed FindByFeedUrl(string feedUrl);

    void Add(Feed feed);

    int? FindFeedId(string feedUrl);

    bool SetDateLastCrawlStarted(int id, DateTime dateTime);

    DateTime? GetDateLastCrawlStarted(int id);

  }

}
