using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services.Exceptions;

namespace JustReadIt.Core.Services {

  public class OpmlImporter : IOpmlImporter {

    private readonly Opml.IOpmlParser _opmlParser;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly IUserFeedGroupRepository _userFeedGroupRepository;
    private readonly IUserFeedGroupFeedRepository _userFeedGroupFeedRepository;

    public OpmlImporter(Opml.IOpmlParser opmlParser, IUserAccountRepository userAccountRepository, IFeedRepository feedRepository, IUserFeedGroupRepository userFeedGroupRepository, IUserFeedGroupFeedRepository userFeedGroupFeedRepository) {
      Guard.ArgNotNull(opmlParser, "opmlParser");
      Guard.ArgNotNull(userAccountRepository, "userAccountRepository");
      Guard.ArgNotNull(feedRepository, "feedRepository");
      Guard.ArgNotNull(userFeedGroupRepository, "userFeedGroupRepository");
      Guard.ArgNotNull(userFeedGroupFeedRepository, "userFeedGroupFeedRepository");

      _opmlParser = opmlParser;
      _userAccountRepository = userAccountRepository;
      _feedRepository = feedRepository;
      _userFeedGroupRepository = userFeedGroupRepository;
      _userFeedGroupFeedRepository = userFeedGroupFeedRepository;
    }

    public void Import(string opmlXml, int userAccountId) {
      Guard.ArgNotNullNorEmpty(opmlXml, "opmlXml");

      Opml.ParseResult parseResult = _opmlParser.Parse(opmlXml);

      using (TransactionScope ts = TransactionUtils.CreateTransactionScope()) {
        if (!_userAccountRepository.UserAccountExists(userAccountId)) {
          throw new ArgumentException(string.Format("User account with id '{0}' doesn't exist.", userAccountId), "userAccountId");
        }

        ImportUncategorizedFeeds(parseResult.UncategorizedFeeds, userAccountId);
        ImportFeedGroups(parseResult.FeedGroups, userAccountId);

        ts.Complete();
      }
    }

    private void ImportUncategorizedFeeds(IEnumerable<Opml.Feed> uncategorizedFeeds, int userAccountId) {
      List<Opml.Feed> uncategorizedFeedsList = uncategorizedFeeds.ToList();

      if (uncategorizedFeedsList.Count == 0) {
        return;
      }

      int? uncategorizedUserFeedGroupId =
        _userFeedGroupRepository.FindSpecialFeedGroupId(userAccountId, SpecialUserFeedGroupType.Uncategorized);

      if (!uncategorizedUserFeedGroupId.HasValue) {
        throw new InternalException(string.Format("Special 'Uncategorized' group is not present for user account id '{0}'.", userAccountId));
      }

      foreach (Opml.Feed feed in uncategorizedFeedsList) {
        ImportFeed(feed, uncategorizedUserFeedGroupId.Value);
      }
    }

    private void ImportFeedGroups(IEnumerable<Opml.FeedGroup> feedGroups, int userAccountId) {
      List<Opml.FeedGroup> feedGroupsList = feedGroups.ToList();

      if (feedGroupsList.Count == 0) {
        return;
      }

      foreach (Opml.FeedGroup feedGroup in feedGroupsList) {
        int? userFeedGroupId =
          _userFeedGroupRepository.FindGroupIdByTitle(userAccountId, feedGroup.Title);

        if (!userFeedGroupId.HasValue) {
          var userFeedGroup =
            new UserFeedGroup {
              UserAccountId = userAccountId,
              SpecialType = null,
              Title = feedGroup.Title,
            };

          _userFeedGroupRepository.Add(userFeedGroup);

          Debug.Assert(userFeedGroup.Id != 0);

          userFeedGroupId = userFeedGroup.Id;
        }

        foreach (Opml.Feed feed in feedGroup.Feeds) {
          ImportFeed(feed, userFeedGroupId.Value);
        }
      }
    }

    private void ImportFeed(Opml.Feed opmlFeed, int userFeedGroupId) {
      int? feedId =
        _feedRepository.FindFeedId(opmlFeed.FeedUrl);

      if (!feedId.HasValue) {
        var feed =
          new Feed {
            Title = opmlFeed.Title,
            FeedUrl = opmlFeed.FeedUrl,
            SiteUrl = opmlFeed.SiteUrl,
          };

        _feedRepository.Add(feed);

        Debug.Assert(feed.Id != 0);

        feedId = feed.Id;
      }

      int? userFeedGroupFeedId =
        _userFeedGroupFeedRepository.FindFeedGroupFeedId(userFeedGroupId, feedId.Value);

      if (!userFeedGroupFeedId.HasValue) {
        var userFeedGroupFeed =
          new UserFeedGroupFeed {
            UserFeedGroupId = userFeedGroupId,
            FeedId = feedId.Value,
          };

        _userFeedGroupFeedRepository.Add(userFeedGroupFeed);

        Debug.Assert(userFeedGroupFeed.Id != 0);
      }
    }

  }

}
