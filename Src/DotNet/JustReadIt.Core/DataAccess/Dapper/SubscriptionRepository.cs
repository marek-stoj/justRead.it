﻿using System;
using System.Collections.Generic;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class SubscriptionRepository : DapperRepository, ISubscriptionRepository {

    public SubscriptionRepository(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<Subscription> GetAll(int userAccountId, DateTime? dateCreatedSince) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Subscription> subscriptions =
          db.Query<Subscription, Feed, Subscription>(
            " select" +
            "   ufgf.Id," +
            "   ufgf.DateCreated," +
            "   f.Id," +
            "   f.Title," +
            "   f.FeedUrl," +
            "   f.SiteUrl" +
            " from UserFeedGroup ufg" +
            " join UserFeedGroupFeed ufgf on ufgf.UserFeedGroupId = ufg.Id" +
            " join Feed f on f.Id = ufgf.FeedId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and (@DateCreatedSince is null or ufgf.DateCreated = @DateCreatedSince)" +
            " order by f.Title, ufgf.DateCreated asc",
            (subscription, feed) => {
              subscription.Feed = feed;

              return subscription;
            },
            new {
              UserAccountId = userAccountId,
              DateCreatedSince = dateCreatedSince,
            });

        return subscriptions;
      }
    }

  }

}
