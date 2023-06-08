using Adoler.AdoExtension.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Adoler.AdoExtension.Helpers
{
    public static class SqlParametersHelper
    {
        private static readonly Dictionary<Type, PropertyInfo[]> propertyCache = new Dictionary<Type, PropertyInfo[]>();
        public static PropertyInfo[] GetPropertyCache<T>(T instance)
        {
            var t = instance.GetType();
            if (!propertyCache.TryGetValue(t, out var properties))
            {
                properties = t.GetProperties();
                propertyCache[t] = properties;
            }
            return properties;
        }

        public static List<SqlParameter> ConvertToParameters<T>(T instance, ParameterDirection direction = ParameterDirection.Input)
        {
            var properties = GetPropertyCache(instance);
            var plist = new List<SqlParameter>();
            foreach (var prop in properties)
            {
                string name = prop.Name;
                object value = prop.GetValue(instance);
                plist.Add(name, value, direction);
            }
            return plist;
        }

        public static List<SqlParameter> ConvertToParameters<T>(T instance, ParameterDirection direction = ParameterDirection.Input, params Expression<Func<T, object>>[] expressions)
        {
            var parameterlist = new List<SqlParameter>();
            var propList = ExpressionHelper.GetProperiesFromExpression(expressions);
            foreach (var prop in propList)
            {
                parameterlist.Add(prop, prop.GetValue(instance), direction);
            }
            return parameterlist;
        }

        public static List<SqlParameter> ConvertToParametersExcept<T>(T instance, ParameterDirection direction = ParameterDirection.Input, params Expression<Func<T, object>>[] expressions)
        {
            var parameterlist = new List<SqlParameter>();
            var excludedpropListNames = ExpressionHelper.GetProperiesFromExpression(expressions).Select(e => e.Name).ToList();
            var properties = GetPropertyCache(instance);
            foreach (var prop in properties.Where(p => !excludedpropListNames.Contains(p.Name)))
            {
                parameterlist.Add(prop, prop.GetValue(instance), direction);
            }
            return parameterlist;
        }
        public static List<SqlParameter> ConvertDynamicToParameters(ExpandoObject instance, ParameterDirection direction = ParameterDirection.Input)
        {
            var plist = new List<SqlParameter>();
            foreach ((string name, object value) in instance.ToList())
            {
                plist.Add(name, value, direction);
            }
            return plist;
        }
        public static void UpdateWithOutputParameters<T>(T instance, List<SqlParameter> plist)
        {
            var properties = GetPropertyCache(instance);
            var ouputPlist = plist.Where(p => (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput) && p.Value != DBNull.Value);
            foreach (var parameter in ouputPlist)
            {
                var prop = properties.FirstOrDefault(p => p.Name == parameter.ParameterName);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(instance, parameter.Value);
                }
            }
        }
        public static List<SqlParameter> ToSqlParamsList<T>(T instance, ICollection<SqlParameter> additionalParams = null, string ignoredParameters = null)
        {
            var properties = GetPropertyCache(instance);
            var ignoredParametersList = string.IsNullOrEmpty(ignoredParameters) ? null : new HashSet<string>(ignoredParameters.Split(','), StringComparer.OrdinalIgnoreCase);
            var parameters = new List<SqlParameter>(properties.Length);

            foreach (var property in properties)
            {
                if (ignoredParametersList != null && ignoredParametersList.Contains(property.Name))
                    continue;

                var value = property.GetValue(instance) ?? DBNull.Value;
                parameters.Add(new SqlParameter(property.Name, value));
            }

            if (additionalParams != null && additionalParams.Count > 0)
                parameters.AddRange(additionalParams);

            return parameters;
        }



    }
}
