using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

  // TODO IMM HI: WRONG TAGGING IDS! WE SHOULD PROBABLY CREATE A NEW JOINING TABLE FOR TAGGINS
  public class TaggingRepository : DapperRepository, ITaggingRepository {

    private const string _TaggingProjection =
      " ufgf.Id as Id," +
      " ufgf.FeedId as FeedId," +
      " ufg.UserAccountId as UserAccountId," +
      " ufg.Title as Name";

    public TaggingRepository(string connectionString)
      : base(connectionString) {
    }

    public IEnumerable<Tagging> GetAll(int userAccountId) {
      using (var db = CreateOpenedConnection()) {
        IEnumerable<Tagging> taggings =
          db.Query<Tagging>(
            " select" +
            _TaggingProjection +
            " from UserFeedGroupFeed ufgf" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where ufg.UserAccountId = @UserAccountId" +
            " order by ufgf.DateCreated desc, ufgf.Id desc",
            new {
              UserAccountId = userAccountId,
            });

        return taggings;
      }
    }

    public Tagging FindById(int id) {
      using (var db = CreateOpenedConnection()) {
        Tagging tagging =
          db.Query<Tagging>(
            " select" +
            _TaggingProjection +
            " from UserFeedGroupFeed ufgf" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where ufgf.Id = @Id",
            new {
              Id = id,
            }).SingleOrDefault();

        return tagging;
      }
    }

    // TODO IMM HI: WRONG!
    public int? FindIdByFeedId(int userAccountId, int feedId) {
      using (var db = CreateOpenedConnection()) {
        int? id =
          db.Query<int?>(
            " select" +
            "   ufgf.Id" +
            " from UserFeedGroupFeed ufgf" +
            " join UserFeedGroup ufg on ufg.Id = ufgf.UserFeedGroupId" +
            " where 1 = 1" +
            "   and ufg.UserAccountId = @UserAccountId" +
            "   and ufgf.FeedId = @FeedId" +
            new {
              UserAccountId = userAccountId,
              FeedId = feedId,
            }).SingleOrDefault();

        return id;
      }
    }

    public void Add(Tagging tagging) {
      // TODO IMM HI: IMPLEMENT!
    }

  }

}
