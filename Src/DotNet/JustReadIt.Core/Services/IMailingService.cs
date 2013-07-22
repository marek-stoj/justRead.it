namespace JustReadIt.Core.Services {

  public interface IMailingService {

    void SendVerificationEmail(int userAccountId, string emailAddress);

  }

}
