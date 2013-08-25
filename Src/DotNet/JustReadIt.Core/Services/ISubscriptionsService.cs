namespace JustReadIt.Core.Services {

  public interface ISubscriptionsService {

    /// <returns>Existing or newly created subscription id.</returns>
    int Subscribe(int userAccountId, string url, string groupTitle);

  }

}
