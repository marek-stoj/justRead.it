using System;
using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class SubscriptionRepository : DapperRepository, ISubscriptionRepository {

    private const string _SubscriptionProjection =
      "   ufgf.Id," +
      "   ufgf.DateCreated," +
      "   ufg.UserAccountId," +
      "   f.Id," +
      "   f.Title," +
      "   f.FeedUrl," +
      "   f.SiteUrl";

    public SubscriptionRepository(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<Subscription> GetAll(int userAccountId, DateTime? dateCreatedSince) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Subscription> subscriptions =
          db.Query<Subscription, Feed, Subscription>(
            " select" +
            _SubscriptionProjection +
            " from UserFeedGroup ufg" +
            " join UserFeedGroupFeed ufgf on ufgf.UserFeedGroupId = ufg.Id" +
            " join Feed f on f.Id = ufgf.FeedId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and (@DateCreatedSince is null or ufgf.DateCreated = @DateCreatedSince)" +
            " order by f.Title, ufgf.DateCreated asc",
            (s, f) => {
              s.Feed = f;

              return s;
            },
            new {
              UserAccountId = userAccountId,
              DateCreatedSince = dateCreatedSince,
            });

        return subscriptions;
      }
    }

    public Subscription FindById(int userAccountId, int id) {
      using (var db = CreateOpenedConnection()) {
        Subscription subscription =
          db.Query<Subscription, Feed, Subscription>(
            " select" +
            _SubscriptionProjection +
            " from UserFeedGroup ufg" +
            " join UserFeedGroupFeed ufgf on ufgf.UserFeedGroupId = ufg.Id" +
            " join Feed f on f.Id = ufgf.FeedId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and ufgf.Id = @Id",
            (s, f) => {
              s.Feed = f;

              return s;
            },
            new {
              Id = id,
              UserAccountId = userAccountId,
            }).SingleOrDefault();

        return subscription;
      }
    }

    public int? FindIdByFeedUrl(int userAccountId, string feedUrl) {
      using (var db = CreateOpenedConnection()) {
        int? id =
          db.Query<int?>(
            " select" +
            "   ufgf.Id" +
            " from UserFeedGroupFeed ufgf" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " join Feed f on f.Id = ufgf.FeedId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and f.FeedUrlChecksum = checksum(@FeedUrl)" +
            "   and f.FeedUrl = @FeedUrl",
            new {
              UserAccountId = userAccountId,
              FeedUrl = feedUrl,
            }).SingleOrDefault();

        return id;
      }
    }

    public void Add(Subscription subscription) {
      Guard.ArgNotNull(subscription, "subscription");

      if (subscription.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "subscription");
      }

      using (var db = CreateOpenedConnection()) {
        DateTime now = DateTime.UtcNow;

        subscription.DateCreated = now;
        subscription.Feed.DateCreated = now;

        int? feedId =
          db.Query<int?>(
            " select" +
            "   f.Id" +
            " from Feed f" +
            " where 1 = 1" +
            "   and f.FeedUrlChecksum = checksum(@FeedUrl)" +
            "   and f.FeedUrl = @FeedUrl",
            new {
              FeedUrl = subscription.Feed.FeedUrl,
            }).SingleOrDefault();

        if (!feedId.HasValue) {
          feedId =
            db.Query<int>(
              " insert into Feed" +
              " (DateCreated, Title, FeedUrl, SiteUrl)" +
              " values" +
              " (@DateCreated, @Title, @FeedUrl, @SiteUrl);" +
              " " +
              " select cast(scope_identity() as int);",
              new {
                DateCreated = subscription.Feed.DateCreated,
                Title = subscription.Feed.Title,
                FeedUrl = subscription.Feed.FeedUrl,
                SiteUrl = subscription.Feed.SiteUrl,
              })
              .Single();
        }

        int? uncategorizedFeedGroupId =
          db.Query<int?>(
            " select" +
            "   ufg.Id" +
            " from UserFeedGroup ufg" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and ufg.SpecialType = @SpecialUserFeedGroupType",
            new {
              UserAccountId = subscription.UserAccountId,
              SpecialUserFeedGroupType = SpecialUserFeedGroupType.Uncategorized.ToString(),
            }).SingleOrDefault();

        if (!uncategorizedFeedGroupId.HasValue) {
          throw new InternalException(string.Format("Uncategorized feed group doesn't exist. User account id: '{0}'.", subscription.UserAccountId));
        }

        int subscriptionId =
          db.Query<int>(
            " insert into UserFeedGroupFeed" +
            " (UserFeedGroupId, FeedId, DateCreated)" +
            " values" +
            " (@UserFeedGroupId, @FeedId, @DateCreated)" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              UserFeedGroupId = uncategorizedFeedGroupId,
              FeedId = feedId.Value,
              DateCreated = subscription.DateCreated,
            })
            .Single();

        subscription.Id = subscriptionId;
      }

    }

  }

}
