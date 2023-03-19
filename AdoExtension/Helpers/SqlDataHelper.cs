using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Dynamic;


namespace Adoler.AdoExtension.Helpers
{

    public class SqlDataHelper
    {
        private string _connectionString = null;
        public SqlDataHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region GetSqlConnection
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }
        #endregion

        #region ExecuteNonQueryAsync
        public async Task<int> ExecuteNonQueryAsync(string procedureName)
        {
            return await ExecuteNonQueryAsync(procedureName, new List<SqlParameter>());
        }
        public async Task<int> ExecuteNonQueryAsync(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParametersHelper.ConvertDynamicToParameters(data);
            return await ExecuteNonQueryAsync(procedureName, parameters);
        }


        public async Task<int> ExecuteNonQueryAsync(string procedureName, params SqlParameter[] commandParameters)
        {
            var plist = commandParameters?.ToList();
            return await ExecuteNonQueryAsync(procedureName, plist);
        }

        public async Task<int> ExecuteNonQueryAsync(string procedureName, List<SqlParameter> plist)
        {
            int resultCount = 0;
            using (var myConnection = GetSqlConnection())
            {
                using (var myCommand = new SqlCommand(procedureName.Trim(), myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Set the parameters
                    if (plist != null && plist.Count > 0)
                    {
                        myCommand.Parameters.AddRange(plist.ToArray());
                    }
                    // Execute the command asynchronously
                    await myConnection.OpenAsync();
                    resultCount = await myCommand.ExecuteNonQueryAsync();
                }
                // Close the connection
                myConnection.Close();
            }
            return resultCount;
        }



        #endregion

        #region ExecuteScalarAsync
        public async Task<object> ExecuteScalarAsync(string procedureName)
        {
            return await ExecuteScalarAsync(procedureName, new List<SqlParameter>());
        }
        public async Task<object> ExecuteScalarAsync(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParametersHelper.ConvertDynamicToParameters(data);
            return await ExecuteScalarAsync(procedureName, parameters);
        }
        public async Task<object> ExecuteScalarAsync(string procedureName, params SqlParameter[] commandParameters)
        {
            var plist = commandParameters?.ToList();
            return await ExecuteScalarAsync(procedureName, plist);
        }

        public async Task<object> ExecuteScalarAsync(string procedureName, List<SqlParameter> plist)
        {
            object result = null;
            using (var myConnection = GetSqlConnection())
            {
                using (var myCommand = new SqlCommand(procedureName.Trim(), myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;
                    // Set the parameters
                    if (plist != null && plist.Count > 0)
                    {
                        myCommand.Parameters.AddRange(plist.ToArray());
                    }
                    // ExecuteScalar the command asynchronously
                    await myConnection.OpenAsync();
                    result = await myCommand.ExecuteScalarAsync();
                }
                // Close the connection
                myConnection.Close();
            }
            return result;
        }
        #endregion

        #region GetSingleRecordAsync
        public async Task<T> GetSingleRecordAsync<T>(string procedureName)
        {
            return await GetSingleRecordAsync<T>(procedureName, new List<SqlParameter>());
        }

        public async Task<T> GetSingleRecordAsync<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParametersHelper.ConvertDynamicToParameters(data);
            return await GetSingleRecordAsync<T>(procedureName, parameters);
        }

        public async Task<T> GetSingleRecordAsync<T>(string procedureName, params SqlParameter[] commandParameters)
        {
            var plist = commandParameters?.ToList();
            return await GetSingleRecordAsync<T>(procedureName, plist);
        }

        public async Task<T> GetSingleRecordAsync<T>(string procedureName, List<SqlParameter> plist)
        {
            var t = typeof(T);
            var list = await GetRecordListAsync<T>(procedureName, plist);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return (T)Activator.CreateInstance(t);
            }
        }
        #endregion

        #region GetListAsync

        public async Task<List<T>> GetRecordListAsync<T>(string procedureName)
        {
            var parameters = new List<SqlParameter>();
            return await GetRecordListAsync<T>(procedureName, parameters);
        }

        public async Task<List<T>> GetRecordListAsync<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParametersHelper.ConvertDynamicToParameters(data);
            return await GetRecordListAsync<T>(procedureName, parameters);
        }
        public async Task<List<T>> GetRecordListAsync<T>(string procedureName, params SqlParameter[] commandParameters)
        {
            var plist = commandParameters?.ToList();
            return await GetRecordListAsync<T>(procedureName, plist);
        }
        public async Task<List<T>> GetRecordListAsync<T>(string procedureName, List<SqlParameter> plist)
        {
            var itemsList = new List<T>();
            using (var myConnection = GetSqlConnection())
            {
                using (var myCommand = new SqlCommand(procedureName.Trim(), myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    // Set the parameters
                    if (plist != null && plist.Count > 0)
                    {
                        myCommand.Parameters.AddRange(plist.ToArray());
                    }

                    // Execute the command
                    await myConnection.OpenAsync();
                    using (var dr = await myCommand.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            var item = DataReaderMapper.GetEntity<T>(dr);
                            if (item != null)
                            {
                                itemsList.Add(item);
                            }
                        }
                    }
                    myConnection.Close();
                }
            }
            return itemsList;
        }

        #endregion

    }
}