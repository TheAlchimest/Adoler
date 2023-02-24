using Adoler;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace Adoler
{
    public static class SqlParameterListExtensions {

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
        public static void AddOutputParameter(this List<SqlParameter> plist, string parameterName, SqlDbType sqltype)
        {
            plist.Add(parameterName, sqltype, ParameterDirection.Output);
        }
        public static void AddOutputParameter(this List<SqlParameter> plist, string parameterName, object value)
        {
            plist.Add(parameterName, value, ParameterDirection.Output);
        }
        public static void AddOutputParameterInteger(this List<SqlParameter> plist, string parameterName)
        {
            plist.AddOutputParameter(parameterName, SqlDbType.Int);
        }
        public static void AddOutputParameterLong(this List<SqlParameter> plist, string parameterName)
        {
            plist.AddOutputParameter(parameterName, SqlDbType.BigInt);
        }
        public static void AddOutputParameterBoolean(this List<SqlParameter> plist, string parameterName)
        {
            plist.AddOutputParameter(parameterName, SqlDbType.Bit);
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
