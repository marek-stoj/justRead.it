using System.Collections.Generic;

namespace JustReadIt.Core.Domain.Repositories {

  public interface ITaggingRepository {

    IEnumerable<Tagging> GetAll(int userAccountId);

    Tagging FindById(int id);

  }

}
