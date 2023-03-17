using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Adoler
{
    public static class SqlParameterListExtensions
    {

        public static void Add(this List<SqlParameter> plist, PropertyInfo prop, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = new SqlParameter(prop.Name, value);
            if (value != null)
            {
                Type typeOfParameter = prop.GetType();
                if (typeOfParameter == typeof(string))
                {
                    p.Size = ((string)value).Length;
                }
            }
            p.Direction = direction;
            plist.Add(p);
        }

        public static void Add(this List<SqlParameter> plist, string parameterName, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = new SqlParameter(parameterName, value);
            if (value != null)
            {
                Type typeOfParameter = value.GetType();
                if (typeOfParameter == typeof(string))
                {
                    p.Size = ((string)value).Length;
                }
            }
            p.Direction = direction;
            plist.Add(p);
        }

        public static void Add(this List<SqlParameter> plist, string parameterName, SqlDbType sqlDbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = new SqlParameter(parameterName, sqlDbType);
            p.Direction = direction;
            plist.Add(p);
        }
        public static void Add(this List<SqlParameter> plist, string parameterName, SqlDbType sqlDbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = new SqlParameter(parameterName, sqlDbType, size);
            p.Direction = direction;
            plist.Add(p);
        }
        public static void Add(this List<SqlParameter> plist, SqlParameter parameter)
        {
            plist.Add(parameter);
        }
        public static SqlParameter AddOutputTotalCountOutput(this List<SqlParameter> plist)
        {
            var p = new SqlParameter("Count" , SqlDbType.Int);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);
            return p;
        }
        public static SqlParameter AddOutputParameter(this List<SqlParameter> plist, string parameterName, SqlDbType sqltype)
        {
            var p = new SqlParameter(parameterName, sqltype);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);
            return p;
        }
        public static SqlParameter AddOutputParameter(this List<SqlParameter> plist, string parameterName, object value)
        {
            var p = new SqlParameter(parameterName, value);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);
            return p;
        }
        public static SqlParameter AddOutputParameterInteger(this List<SqlParameter> plist, string parameterName)
        {
            var p = new SqlParameter(parameterName, SqlDbType.Int);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);
            return p;
        }
        public static SqlParameter AddOutputParameterLong(this List<SqlParameter> plist, string parameterName)
        {
            var p = new SqlParameter(parameterName, SqlDbType.BigInt);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);

            return p;
        }
        public static SqlParameter AddOutputParameterBoolean(this List<SqlParameter> plist, string parameterName)
        {
            var p = new SqlParameter(parameterName, SqlDbType.Bit);
            p.Direction = ParameterDirection.Output;
            plist.Add(p);
            return p;
        }
        public static void GenerateParametersFromEntity(this List<SqlParameter> plist, object obj, string parametersNames)
        {
            Type t = obj.GetType();
            string[] parametersNamesArray = parametersNames.Split(',');

            PropertyInfo myPropInfo;
            object parameterValue;
            foreach (var parameterName in parametersNamesArray)
            {
                myPropInfo = t.GetProperty(parameterName);
                parameterValue = myPropInfo.GetValue(obj);
                plist.Add("parameterName", parameterValue);
            }
        }
        public static void AddSqlOperationParameter(this List<SqlParameter> plist, EnumSqlOperationType sqlOperation)
        {
            var p = new SqlParameter();
            p.ParameterName = "SqlOperation";
            p.SqlDbType = SqlDbType.Int;
            p.Value = (int)sqlOperation;
            plist.Add(p);
        }
        public static void AddSqlOperationParameter(this List<SqlParameter> plist, string parameterName, EnumSqlOperationType sqlOperation)
        {
            var p = new SqlParameter();
            p.ParameterName = parameterName;
            p.SqlDbType = SqlDbType.Int;
            p.Value = (int)sqlOperation;
            plist.Add(p);
        }
    }
}
