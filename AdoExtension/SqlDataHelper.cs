using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Reflection;
using System.Linq;


namespace Adoler
{

    public class SqlDataHelper
    {
        private string _connectionString = null;
        public SqlDataHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region --------------GetSqlConnection--------------
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }
        #endregion

        #region --------------ExecuteNonQuery--------------
        public int ExecuteNonQuery(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return ExecuteNonQuery(procedureName, parameters);
        }
        public int ExecuteNonQuery(string procedureName, params SqlParameter[] commandParameters)
        {
            List<SqlParameter> plist = (commandParameters != null) ? commandParameters.ToList() : null;
            return ExecuteNonQuery(procedureName, plist);
        }
        public int ExecuteNonQuery(string procedureName, List<SqlParameter> plist)
        {
            int resultCount = 0;
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                //---------------------------------------------------------------------
                // Execute the command
                myConnection.Open();
                resultCount = myCommand.ExecuteNonQuery();
                myConnection.Close();
                //----------------------------------------------------------------
                return resultCount;
            }
        }
        #endregion

        #region --------------ExecuteScalar--------------
        /// <summary>
        /// Executes scalar stored procedure
        /// </summary>
        /// <param name="procedureName">stored procedure</param>
        /// <param name="data">dynamic parameters object</param>
        /// <returns></returns>
        /// <example>
        /// dynamic queryParameters = new ExpandoObject();
        /// queryParameters.UserAccount = userAcc;
        /// return (bool) _sqlDataHelper.ExecuteScalar("[ERP].[EmploymentProcess_IsEmployeeHasAnyOpenRequestsAsManager]", queryParameters);
        /// 
        /// </example>
        public object ExecuteScalar(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return ExecuteScalar(procedureName, parameters);
        }
        public object ExecuteScalar(string procedureName, params SqlParameter[] commandParameters)
        {
            List<SqlParameter> plist = (commandParameters != null) ? commandParameters.ToList() : null;
            return ExecuteScalar(procedureName, plist);
        }
        public object ExecuteScalar(string procedureName, List<SqlParameter> plist)
        {
            object result = 0;
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                //---------------------------------------------------------------------
                // ExecuteScalar the command
                myConnection.Open();
                result = myCommand.ExecuteScalar();
                myConnection.Close();
                //----------------------------------------------------------------
                return result;
            }
        }
        #endregion

        #region --------------ExecuteReaderForSingleRecord--------------
        public T ExecuteReaderForSingleRecord<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return ExecuteReaderForSingleRecord<T>(procedureName, parameters);
        }
        public T ExecuteReaderForSingleRecord<T>(string procedureName, params SqlParameter[] commandParameters)
        {
            List<SqlParameter> plist = (commandParameters != null) ? commandParameters.ToList() : null;
            return ExecuteReaderForSingleRecord<T>(procedureName, plist);
        }

        public T ExecuteReaderForSingleRecord<T>(string procedureName, List<SqlParameter> plist)
        {
            Type t = typeof(T);
            List<T> list = ExecuteReaderForList<T>(procedureName, plist);
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

        #region --------------ExecuteReaderForList--------------

        public List<T> ExecuteReaderForList<T>(string procedureName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            return ExecuteReaderForList<T>(procedureName, parameters);
        }
        public List<T> ExecuteReaderForList<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return ExecuteReaderForList<T>(procedureName, parameters);
        }
        public List<T> ExecuteReaderForList<T>(string procedureName, List<SqlParameter> plist)
        {
            List<T> itemsList = new List<T>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                // Execute the command
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    T item = DataReaderMapper.GetEntity<T>(dr);
                    if (item != null)
                    {
                        itemsList.Add(item);
                    }
                }
                dr.Close();
                myConnection.Close();
                //----------------------------------------------------------------
                return itemsList;
            }
        }

        #endregion

        #region --------------ExecuteReaderForMultiRecordSet--------------
        public Dictionary<string, object> ExecuteReaderForMultiRecordSet(string procedureName, List<SqlParameter> parameters, params Type[] types)
        {
            var recordSetDefinitions = GenerateRecordSetDefinition(types);
            return ExecuteReaderForMultiRecordSet(procedureName, parameters, recordSetDefinitions);

        }
        public Dictionary<string, object> ExecuteReaderForMultiRecordSet(string procedureName, List<SqlParameter> parameters, Type[] types, List<string> names)
        {
            var rsDefinitionManager = new RecordSetDefinitionManager();
            var recordSetDefinitions = rsDefinitionManager.GenerateRecordSetDefinition(types, names);
            return ExecuteReaderForMultiRecordSet(procedureName, parameters, recordSetDefinitions);
        }
        public Dictionary<string, object> ExecuteReaderForMultiRecordSet(string procedureName, List<SqlParameter> parameters, List<RecordSetDefinition> tepesDefinition)
        {
            Dictionary<string, object> resultSet = new Dictionary<string, object>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------
                // Execute the command
                int index = 0;
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                foreach (var t in tepesDefinition)
                {
                    if (index > 0) { dr.NextResult(); }
                    if (t.IsGenericType)
                    {
                        IList itemsList = GetListOfDataFromDataReader(dr, t);
                        resultSet.Add(t.Name, itemsList);
                    }
                    else
                    {
                        var item = GetSingleobjectFromDataReader(dr, t);
                        resultSet.Add(t.Name, item);
                    }
                    ++index;
                }
                dr.Close();
                myConnection.Close();
                //----------------------------------------------------------------
                return resultSet;
            }
        }
        private List<RecordSetDefinition> GenerateRecordSetDefinition(Type[] types)
        {
            var rsDefinitionManager = new RecordSetDefinitionManager();
            return rsDefinitionManager.GenerateRecordSetDefinition(types, null);
        }

        private IList GetListOfDataFromDataReader(IDataReader dr, RecordSetDefinition t)
        {

            var itemsList = (IList)Activator.CreateInstance(t.Type);
            while (dr.Read())
            {
                var item = DataReaderMapper.GetEntity(dr, t.GenericObjectType);
                if (item != null)
                {
                    itemsList.Add(item);
                }
            }
            return itemsList;
        }

        private object GetSingleobjectFromDataReader(IDataReader dr, RecordSetDefinition t)
        {
            object item = null;
            while (dr.Read())
            {
                item = DataReaderMapper.GetEntity(dr, t.Type);
                break;
            }
            return item;

        }

        #endregion

        #region --------------RetrievePaggedEntityList--------------
        public List<T> RetrievePaggedEntityList<T>(string procedureName, ExpandoObject data, int pageNo, int pageSize, out int count)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return RetrievePaggedEntityList<T>(procedureName, parameters, pageNo, pageSize, out count);
        }

        public List<T> RetrievePaggedEntityList<T>(string procedureName, int pageNo, int pageSize, out int count)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            return RetrievePaggedEntityList<T>(procedureName, parameters, pageNo, pageSize, out count);
        }

        public List<T> RetrievePaggedEntityList<T>(string procedureName, List<SqlParameter> plist, int pageNo, int pageSize, out int count)
        {
            List<T> itemsList = new List<T>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                //add pagging parameters 
                myCommand.Parameters.Add("@PageNo", SqlDbType.Int).Value = pageNo;
                myCommand.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                myCommand.Parameters.Add("@Count", SqlDbType.Int).Direction = ParameterDirection.Output;
                // Execute the command
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    var item = DataReaderMapper.GetEntity<T>(dr);
                    if (item != null)
                    {
                        itemsList.Add(item);
                    }
                }
                dr.Close();
                myConnection.Close();
                count = (int)myCommand.Parameters["@Count"].Value;
                //----------------------------------------------------------------
                return itemsList;
            }
        }

        #endregion


        #region --------------GetList--------------

        public T GetOne<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return GetOne<T>(procedureName, parameters);
        }

        public T GetOne<T>(string procedureName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            return GetOne<T>(procedureName, parameters);

        }

        public T GetOne<T>(string procedureName, List<SqlParameter> plist)
        {
            T item = default(T);
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                // Execute the command
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    item = DataReaderMapper.GetEntity<T>(dr);
                }
                dr.Close();
                myConnection.Close();
                return item;
            }
        }

        #endregion

        #region --------------GetList--------------

        public List<T> GetList<T>(string procedureName, ExpandoObject data)
        {
            var parameters = SqlParameters.ConvertDynamicToParameters(data);
            return GetList<T>(procedureName, parameters);
        }

        public List<T> GetList<T>(string procedureName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            return GetList<T>(procedureName, parameters);

        }

        public List<T> GetList<T>(string procedureName, List<SqlParameter> plist)
        {
            List<T> itemsList = new List<T>();
            using (SqlConnection myConnection = GetSqlConnection())
            {
                SqlCommand myCommand = new SqlCommand(procedureName.Trim(), myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                if (plist != null && plist.Count > 0)
                {
                    foreach (SqlParameter p in plist)
                    {
                        myCommand.Parameters.Add(p);
                    }
                }
                // Execute the command
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    var item = DataReaderMapper.GetEntity<T>(dr);
                    if (item != null)
                    {
                        itemsList.Add(item);
                    }
                }
                dr.Close();
                myConnection.Close();
                return itemsList;
            }
        }

        #endregion

    }
}