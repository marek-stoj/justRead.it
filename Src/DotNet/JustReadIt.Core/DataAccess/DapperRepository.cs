using System.Data;
using System.Data.SqlClient;

namespace JustReadIt.Core.DataAccess {

  public abstract class DapperRepository {
    private readonly string _connectionString;

    protected DapperRepository(string connectionString) {
      _connectionString = connectionString;
    }

    protected IDbConnection CreateOpenedConnection() {
      var connection = new SqlConnection(_connectionString);

      connection.Open();

      return connection;
    }
  }

}
