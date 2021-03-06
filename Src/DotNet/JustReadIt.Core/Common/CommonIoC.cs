﻿using System;
using System.Configuration;
using ImmRafSoft.Configuration;
using ImmRafSoft.Net;
using ImmRafSoft.Security;
using JustReadIt.Core.DataAccess.Dapper;
using JustReadIt.Core.Domain.Query;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.Core.Services.Feeds;
using JustReadIt.Core.Services.Opml;
using JustReadIt.Core.Services.Workers;
using NReadability;

namespace JustReadIt.Core.Common {

  public static class CommonIoC {

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
    private static IWebClientFactory _webClientFactory;

    static CommonIoC() {
      _ConnectionString_JustReadIt =
        ConfigurationManager.ConnectionStrings[_ConnectionStringName_JustReadIt]
          .ConnectionString;
    }

    public static IUserAccountRepository GetUserAccountRepository() {
      return new UserAccountRepository(_ConnectionString_JustReadIt);
    }

    public static IFeedRepository GetFeedRepository() {
      return new FeedRepository(_ConnectionString_JustReadIt);
    }

    public static IFeedItemRepository GetFeedItemRepository() {
      return new FeedItemRepository(_ConnectionString_JustReadIt);
    }

    public static IUserFeedGroupRepository GetUserFeedGroupRepository() {
      return new UserFeedGroupRepository(_ConnectionString_JustReadIt);
    }

    public static IUserFeedGroupFeedRepository GetUserFeedGroupFeedRepository() {
      return new UserFeedGroupFeedRepository(_ConnectionString_JustReadIt);
    }

    public static ITaggingRepository GetTaggingRepository() {
      return new TaggingRepository(_ConnectionString_JustReadIt);
    }

    public static IOpmlParser GetOpmlParser() {
      return new OpmlParser();
    }

    public static IOpmlImporter GetOpmlImporter() {
      return
        new OpmlImporter(
          GetOpmlParser(),
          GetUserAccountRepository(),
          GetFeedRepository(),
          GetUserFeedGroupRepository(),
          GetUserFeedGroupFeedRepository());
    }

    public static IMembershipService GetMembershipService() {
      return
        new MembershipService(
          GetUserAccountRepository(),
          GetUserFeedGroupRepository(),
          GetEmailVerificationTokenRepository(),
          GetCryptoUtils(),
          GetMailingService());
    }

    private static ICryptoUtils GetCryptoUtils() {
      string ivBase64Encoded =
        AppSettingsUtils.ReadAppSettingString(_AppSettingKey_RijndaelBase64CryptoUtils_InitializationVector);

      string keyBase64Encoded =
        AppSettingsUtils.ReadAppSettingString(_AppSettingKey_RijndaelBase64CryptoUtils_Key);

      byte[] iv = Convert.FromBase64String(ivBase64Encoded);
      byte[] key = Convert.FromBase64String(keyBase64Encoded);

      return new RijndaelBase64CryptoUtils(iv, key);
    }

    public static IEmailVerificationTokenRepository GetEmailVerificationTokenRepository() {
      return new EmailVerificationTokenRepository(_ConnectionString_JustReadIt);
    }

    public static IMailingService GetMailingService() {
      string from = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_MailingService_From);

      return
        new MailingService(
          GetMailer(),
          GetEmailVerificationTokenRepository(),
          from);
    }

    private static IMailer GetMailer() {
      if (_mailer != null) {
        return _mailer;
      }

      string smptHost = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpHost);
      int smtpPort = AppSettingsUtils.ReadAppSettingInt(_AppSettingKey_SmtpMailer_SmtpPort);
      bool enableSsl = AppSettingsUtils.ReadAppSettingBool(_AppSettingKey_SmtpMailer_EnableSsl);
      string smtpUserName = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpUsername);
      string smtpPasswordEncrypted = AppSettingsUtils.ReadAppSettingString(_AppSettingKey_SmtpMailer_SmtpPassword);

      ICryptoUtils cryptoUtils = GetCryptoUtils();

      string smtpPassword = cryptoUtils.Decrypt(smtpPasswordEncrypted);

      _mailer = new SmtpMailer(smptHost, smtpPort, enableSsl, smtpUserName, smtpPassword);

      return _mailer;
    }

    public static ISubscriptionRepository GetSubscriptionRepository() {
      return new SubscriptionRepository(_ConnectionString_JustReadIt);
    }

    public static IWebClientFactory GetWebClientFactory() {
      return _webClientFactory ?? (_webClientFactory = new SmartWebClientFactory());
    }

    public static IFeedFetcher GetFeedFetcher() {
      return new FeedFetcher(GetWebClientFactory());
    }

    public static IFeedParser GetFeedParser() {
      return new FeedParser();
    }

    public static IFeedsCrawler GetFeedsCrawler() {
      return
        new FeedsCrawler(
          GetFeedRepository(),
          GetFeedItemRepository(),
          GetFeedFetcher(),
          GetFeedParser());
    }

    public static ISubscriptionQueryDao GetSubscriptionQueryDao() {
      return new SubscriptionQueryDao(_ConnectionString_JustReadIt);
    }

    public static IUrlFetcher GetUrlFetcher() {
      return new UrlFetcher();
    }

    public static IArticlesService GetArticlesService() {
      return new ArticlesService(GetUrlFetcher(), GetWebClientFactory());
    }

    public static ISubscriptionsService GetSubscriptionsService() {
      return
        new SubscriptionsService(
          GetFeedFetcher(),
          GetFeedParser(),
          GetSubscriptionRepository(),
          GetUserFeedGroupRepository(),
          GetSubscriptionQueryDao());
    }

  }

}
