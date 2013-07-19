using System.Collections.Generic;

namespace JustReadIt.Core.Domain {

  public interface IFeedRepository {

    IEnumerable<Feed> GetAll();

  }

}
