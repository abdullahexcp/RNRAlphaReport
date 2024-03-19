using System.Data;

namespace RNRAlphaReport.DataAccess.DatabaseUtil
{
    public interface IDatabaseUtil
    {
        DataTable ExecuteQuery(string connectionString, string query, IDictionary<string, object> parameters = null);

        int ExecuteNonQuerySingleOrBulk(string connectionString, string query, IList<IDictionary<string, object>> parameters = null);

    }

}
