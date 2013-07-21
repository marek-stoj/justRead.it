﻿using System;
using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper.Exceptions;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  public class FeedRepository : DapperRepository, IFeedRepository {

    public FeedRepository(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<Feed> GetAll() {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Feed> feeds =
          db.Query<Feed>(
            "select * from Feed");

        return feeds;
      }

    }

    public void Add(Feed feed) {
      Guard.ArgNotNull(feed, "feed");

      if (feed.Id != 0) {
        throw new ArgumentException("Non-transient entity can't be added. Id must be 0.", "feed");
      }

      using (var db = CreateOpenedConnection()) {
        int feedId =
          db.Query<int>(
            " insert into Feed" +
            " (Title, FeedUrl, SiteUrl)" +
            " values" +
            " (@Title, @FeedUrl, @SiteUrl);" +
            " " +
            " select cast(scope_identity() as int);",
            new {
              Title = feed.Title,
              FeedUrl = feed.FeedUrl,
              SiteUrl = feed.SiteUrl,
            })
            .Single();

        if (feedId <= 0) {
          throw new IdentityInsertFailedException();
        }

        feed.Id = feedId;
      }
    }

    public int? FindFeedId(string feedUrl) {
      Guard.ArgNotNullNorEmpty(feedUrl, "feedUrl");

      using (var db = CreateOpenedConnection()) {
        int? feedId =
          db.Query<int?>(
            " select" +
            "   Id" +
            " from Feed" +
            " where FeedUrlChecksum = checksum(@FeedUrl)" +
            "   and FeedUrl = @FeedUrl",
            new { FeedUrl = feedUrl })
            .SingleOrDefault();

        return feedId;
      }
    }

  }

}
