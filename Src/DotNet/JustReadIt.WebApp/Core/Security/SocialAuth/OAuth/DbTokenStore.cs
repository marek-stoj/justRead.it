using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using JustReadIt.Core.Common;
using JustReadIt.Core.DataAccess.Dapper;

namespace JustReadIt.WebApp.Core.Security.SocialAuth.OAuth {

  // TODO IMM HI: move somewhere else?
  public class DbTokenStore : ITokenStore {

    private readonly string _connectionString;
    private readonly string _tableName;

    #region Constructor(s)

    public DbTokenStore(string connectionString, string tableName) {
      Guard.ArgNotNullNorEmpty(connectionString, "connectionString");
      Guard.ArgNotNullNorEmpty(tableName, "tableName");

      _connectionString = connectionString;
      _tableName = tableName;
    }

    #endregion

    #region ITokenStore Members

    public Guid Store(string token, string secret) {
      Guard.ArgNotNullNorEmpty(token, "token");
      Guard.ArgNotNullNorEmpty(secret, "secret");

      using (var db = CreateOpenedConnection()) {
        Guid tokenId = Guid.NewGuid();
        DateTime utcNow = DateTime.UtcNow;

        string queryText = "insert into " + _tableName + " ([Id], [DateCreated], [Token], [Secret]) values (@id, @dateCreated, @token, @secret)";
        var args = new { id = tokenId, dateCreated = utcNow, token = token, secret = secret };

        db.Execute(queryText, args);

        return tokenId;
      }
    }

    public string Get(string token) {
      Guard.ArgNotNullNorEmpty(token, "token");

      using (var db = CreateOpenedConnection()) {
        string queryText = "select [Secret] from " + _tableName + " where [Token] = @token";
        var args = new { token = token };

        string secret =
          db.Query(queryText, args)
            .Select(x => x.Secret)
            .SingleOrDefault();

        return secret;
      }
    }

    public void Remove(string token) {
      Guard.ArgNotNullNorEmpty(token, "token");

      using (var db = CreateOpenedConnection()) {
        string queryText = "delete from " + _tableName + " where [Token] = @token";
        var args = new { token = token };

        db.Execute(queryText, args);
      }
    }

    #endregion

    #region Private helper methods

    private IDbConnection CreateOpenedConnection() {
      var connection = new SqlConnection(_connectionString);

      connection.Open();

      return connection;
    }

    #endregion
  }

}
