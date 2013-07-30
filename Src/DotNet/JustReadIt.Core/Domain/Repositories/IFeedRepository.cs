using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface IFeedRepository {

    IEnumerable<Feed> GetAll();

    Feed FindById(int id);

    void Add(Feed feed);

    int? FindFeedId(string feedUrl);

  }

}
