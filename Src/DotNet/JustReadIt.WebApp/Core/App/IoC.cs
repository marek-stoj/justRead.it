using System;
using System.Configuration;
using ImmRafSoft.Configuration;
using ImmRafSoft.Net;
using ImmRafSoft.Security;
using JustReadIt.Core.DataAccess.Dapper;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.Core.Services.Opml;
using JustReadIt.WebApp.Core.Services;

namespace JustReadIt.WebApp.Core.App {

  public static class IoC {

    private const string _ConnectionStringName_JustReadIt = "JustReadIt";

    private static readonly string _ConnectionString_JustReadIt;

    private const string _AppSettingKey_RijndaelBase64CryptoUtils_InitializationVector = "RijndaelBase64CryptoUtils_InitializationVector";
    private const string _AppSettingKey_RijndaelBase64CryptoUtils_Key = "RijndaelBase64CryptoUtils_Key";

    private const string _AppSettingKey_SmtpMailer_SmtpHost = "SmtpMailer_SmtpHost";
    private const string _AppSettingKey_SmtpMailer_SmtpPort = "SmtpMailer_SmtpPort";
    private const string _AppSettingKey_SmtpMailer_EnableSsl = "SmtpMailer_EnableSsl";
    private const string _AppSettingKey_SmtpMailer_SmtpUsername = "SmtpMailer_SmtpUsername";
    private const string _AppSettingKey_SmtpMailer_SmtpPassword = "SmtpMailer_SmtpPassword";

    private const string _AppSettingKey_MailingService_From = "MailingService_From";

    private static IMailer _mailer;

    static IoC() {
      _ConnectionString_JustReadIt =
        ConfigurationManager.ConnectionStrings[_ConnectionStringName_JustReadIt]
          .ConnectionString;
    }

    public static IUserAccountRepository CreateUserAccountRepository() {
      return new UserAccountRepository(_ConnectionString_JustReadIt);
    }

    public static IFeedRepository CreateFeedRepository() {
      return new FeedRepository(_ConnectionString_JustReadIt);
    }

    private static IUserFeedGroupRepository CreateUserFeedGroupRepository() {
      return new UserFeedGroupRepository(_ConnectionString_JustReadIt);
    }

    private static IUserFeedGroupFeedRepository CreateUserFeedGroupFeedRepository() {
      return new UserFeedGroupFeedRepository(_ConnectionString_JustReadIt);
    }

    public static IOpmlParser CreateOpmlParser() {
      return new OpmlParser();
    }

    public static IOpmlImporter CreateOpmlImporter() {
      return
        new OpmlImporter(
          CreateOpmlParser(),
          CreateUserAccountRepository(),
          CreateFeedRepository(),
          CreateUserFeedGroupRepository(),
          CreateUserFeedGroupFeedRepository());
    }

    public static IAuthenticationService CreateAuthenticationService() {
      return new FormsAuthenticationService();
    }

    public static IMembershipService CreateMembershipService() {
      return
        new MembershipService(
          CreateUserAccountRepository(),
          CreateEmailVerificationTokenRepository(),
          CreateCryptoUtils(),
          CreateMailingService());
    }

    private static ICryptoUtils CreateCryptoUtils() {
      string ivBase64Encoded =
        AppSettingsUtils.ReadAppSettingString(_AppSettingKey_RijndaelBase64CryptoUtils_InitializationVector);

      string keyBase64Encoded =
        AppSettingsUtils.ReadAppSettingString(_AppSettingKey_RijndaelBase64CryptoUtils_Key);

      byte[] iv = Convert.FromBase64String(ivBase64Encoded);
      byte[] key = Convert.FromBase64String(keyBase64Encoded);

      return new RijndaelBase64CryptoUtils(iv, key);
    }

    public static IEmailVerificationTokenRepository CreateEmailVerificationTokenRepository() {
      return new EmailVerificationTokenRepository(_ConnectionString_JustReadIt);
    }

    public static IMailingService CreateMailingService() {
      string from = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_MailingService_From);

      return
        new MailingService(
          CreateMailer(),
          CreateEmailVerificationTokenRepository(),
          from);
    }

    private static IMailer CreateMailer() {
      if (_mailer != null) {
        return _mailer;
      }

      string smptHost = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpHost);
      int smtpPort = AppSettingsUtils.ReadAppSettingInt(_AppSettingKey_SmtpMailer_SmtpPort);
      bool enableSsl = AppSettingsUtils.ReadAppSettingBool(_AppSettingKey_SmtpMailer_EnableSsl);
      string smtpUserName = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpUsername);
      string smtpPasswordEncrypted = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpPassword);

      ICryptoUtils cryptoUtils = CreateCryptoUtils();

      string smtpPassword = cryptoUtils.Decrypt(smtpPasswordEncrypted);

      _mailer = new SmtpMailer(smptHost, smtpPort, enableSsl, smtpUserName, smtpPassword);

      return _mailer;
    }

  }

}
