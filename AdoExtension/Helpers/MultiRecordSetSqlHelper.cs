using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Collections;
using Adoler.Dtos;

namespace Adoler.AdoExtension.Helpers
{


    public class MultiRecordSetSqlHelper
    {
        private string _connectionString = null;
        public MultiRecordSetSqlHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region --------------GetSqlConnection--------------
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }
        #endregion
        public async Task<Dictionary<string, object>> ExecuteReaderForMultiRecordSetAsync(string procedureName, List<SqlParameter> parameters, params Type[] types)
        {
            var recordSetDefinitions = GenerateRecordSetDefinition(types);
            return await ExecuteReaderForMultiRecordSetAsync(procedureName, parameters, recordSetDefinitions).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, object>> ExecuteReaderForMultiRecordSetAsync(string procedureName, List<SqlParameter> parameters, Type[] types, List<string> names)
        {
            var recordSetDefinitions = GenerateRecordSetDefinition(types, names);
            return await ExecuteReaderForMultiRecordSetAsync(procedureName, parameters, recordSetDefinitions).ConfigureAwait(false);
        }
        public async Task<Dictionary<string, object>> ExecuteReaderForMultiRecordSetAsync(string procedureName, List<SqlParameter> parameters, List<RecordSetDefinition> tepesDefinition)
        {
            var resultSet = new Dictionary<string, object>();
            using (var myConnection = GetSqlConnection())
            {
                using (var myCommand = new SqlCommand(procedureName.Trim(), myConnection))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    foreach (var p in parameters)
                    {
                        myCommand.Parameters.Add(p);
                    }

                    int index = 0;

                    await myConnection.OpenAsync().ConfigureAwait(false);

                    using (DbDataReader dr = await myCommand.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        foreach (var t in tepesDefinition)
                        {
                            if (index > 0)
                            {
                                await dr.NextResultAsync().ConfigureAwait(false);
                            }

                            if (t.IsGenericType)
                            {
                                var itemsList = GetListOfDataFromDataReader(dr, t);
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
                    }

                    myConnection.Close();
                }
            }

            return resultSet;
        }


        private List<RecordSetDefinition> GenerateRecordSetDefinition(Type[] types)
        {
            return GenerateRecordSetDefinition(types, null);
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
        public List<RecordSetDefinition> GenerateRecordSetDefinition(Type[] types, List<string> names)
        {
            var recordSetDefinitions = new List<RecordSetDefinition>();
            RecordSetDefinition item = null;
            int index = 0;
            foreach (var t in types)
            {
                item = new RecordSetDefinition();
                item.Type = t;
                item.IsGenericType = t.IsGenericType;
                if (t.IsGenericType)
                {
                    item.GenericObjectType = t.GetGenericArguments()[0];
                }
                if (names == null)
                {
                    item.Name = t.IsGenericType ? item.GenericObjectType.Name : item.Type.Name;
                }
                else
                {
                    item.Name = names[index];
                }
                recordSetDefinitions.Add(item);
                ++index;
            }
            return recordSetDefinitions;
        }

    }
}
