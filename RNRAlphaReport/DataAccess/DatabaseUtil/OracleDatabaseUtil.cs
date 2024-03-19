using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace RNRAlphaReport.DataAccess.DatabaseUtil
{
    public class OracleDatabaseUtil : IDatabaseUtil
    {


        public DataTable ExecuteQuery(string connectionString, string query, IDictionary<string, object> parameters = null)
        {
            OracleConnection conn = null;
            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();
                using (var cmd = new OracleCommand(query, conn))
                {
                    //injecting params values
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var param in parameters)
                        {

                            // it automatically maps the CLR type to the appropriate SQL type
                            var parameter = cmd.CreateParameter();
                            parameter.ParameterName = param.Key;
                            parameter.Value = param.Value ?? DBNull.Value;
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        conn.Close();

                        return dataTable;
                    }
                }
            }

            finally { if (conn != null && conn.State == ConnectionState.Open) conn.Close(); }

        }
        public int ExecuteNonQuerySingleOrBulk(string connectionString, string query, IList<IDictionary<string, object>> parameters = null)
        {
            int totalRowsAffected = 0;

            OracleConnection conn = null;
            try
            {
                conn = new OracleConnection(connectionString);
                conn.Open();

                var transaction = conn.BeginTransaction();

                // Split the query into individual statements based on the semicolon and end of line.
                string[] nonQueries = query.Split(new[] { ";", ";\n", ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (var nonQueryLineNo = 0; nonQueryLineNo < nonQueries.Count(); nonQueryLineNo++)
                {
                    string trimmedQuery = nonQueries[nonQueryLineNo].Trim();
                    if (!string.IsNullOrEmpty(trimmedQuery))
                    {
                        using (var cmd = new OracleCommand(trimmedQuery, conn))
                        {
                            //injecting params
                            if (parameters[nonQueryLineNo] != null && parameters[nonQueryLineNo].Count > 0)
                            {
                                foreach (var param in parameters[nonQueryLineNo])
                                {
                                    // Add parameter to the SqlCommand object
                                    // ADO.NET automatically maps the CLR type to the appropriate SQL type
                                    var parameter = cmd.CreateParameter();
                                    parameter.ParameterName = param.Key;
                                    parameter.Value = param.Value ?? DBNull.Value; // Handle null values
                                    cmd.Parameters.Add(parameter);
                                }
                            }

                            // Regex to find the Arabic N'...' values and create a parameterized SQL statement
                            var regex = new Regex(@"SET\s+(\w+)\s*=\s*N'([^']*)'");
                            var match = regex.Match(trimmedQuery);
                            if (match.Success)
                            {
                                // Extracted column name and value
                                string columnName = match.Groups[1].Value;
                                string value = match.Groups[2].Value;

                                // Replace the original value with a parameter placeholder
                                string paramPlaceholder = $"@param{columnName}";
                                string paramSqlStatement = regex.Replace(trimmedQuery, $"SET {columnName} = {paramPlaceholder}");
                                cmd.CommandText = paramSqlStatement;
                                cmd.Parameters.Add(paramPlaceholder, value);
                            }

                            int rowsAffected = cmd.ExecuteNonQuery();

                            totalRowsAffected += rowsAffected;
                        }
                    }
                }

                transaction.Commit();
            }

            finally { if (conn != null && conn.State == ConnectionState.Open) conn.Close(); }

            return totalRowsAffected;
        }

    }
}
