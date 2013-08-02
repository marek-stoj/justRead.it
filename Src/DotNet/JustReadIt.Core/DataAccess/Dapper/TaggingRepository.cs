using System.Collections.Generic;
using System.Linq;
using JustReadIt.Core.Domain;
using JustReadIt.Core.Domain.Repositories;

namespace JustReadIt.Core.DataAccess.Dapper {

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

  }

}
