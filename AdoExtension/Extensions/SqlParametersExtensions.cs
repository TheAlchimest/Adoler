using Adoler.AdoExtension.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Adoler.AdoExtension.Extensions
{
    public static class SqlParametersExtensions
    {

        public static List<SqlParameter> ConvertToParameters<T>(this T instance)
        {
            return SqlParametersHelper.ConvertToParameters(instance, ParameterDirection.Input);
        }

        public static List<SqlParameter> ConvertToParameters<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParameters(instance, ParameterDirection.Input, expressions);
        }

        public static List<SqlParameter> ConvertToParametersExcept<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParametersExcept(instance, ParameterDirection.Input, expressions);
        }
        public static List<SqlParameter> ConvertDynamicToParameters(ExpandoObject dynamicObject)
        {
            return SqlParametersHelper.ConvertDynamicToParameters(dynamicObject, ParameterDirection.Input);
        }



        public static List<SqlParameter> ConvertToOutputParameters<T>(this T instance)
        {
            return SqlParametersHelper.ConvertToParameters(instance, ParameterDirection.Output);
        }

        public static List<SqlParameter> ConvertToOutputParameters<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParameters(instance, ParameterDirection.Output, expressions);
        }

        public static List<SqlParameter> ConvertToOutputParametersExcept<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParametersExcept(instance, ParameterDirection.Output, expressions);
        }
        public static List<SqlParameter> ConvertDynamicToOutputParameters(ExpandoObject dynamicObject)
        {
            return SqlParametersHelper.ConvertDynamicToParameters(dynamicObject, ParameterDirection.Output);
        }

        /*
        public static List<SqlParameter> ConvertToParameters<T>(this T instance, ParameterDirection direction = ParameterDirection.Input)
        {
            return SqlParametersHelper.ConvertToParameters(instance, direction);
        }

        public static List<SqlParameter> ConvertToParameters<T>(this T instance, ParameterDirection direction = ParameterDirection.Input, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParameters(instance, direction, expressions);
        }

        public static List<SqlParameter> ConvertToParametersExcept<T>(T instance, ParameterDirection direction = ParameterDirection.Input, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParametersHelper.ConvertToParametersExcept(instance, direction, expressions);
        }
        public static List<SqlParameter> ConvertDynamicToParameters(ExpandoObject dynamicObject, ParameterDirection direction = ParameterDirection.Input)
        {
            return SqlParametersHelper.ConvertDynamicToParameters(dynamicObject, direction);
        }
        */
        public static void UpdateWithOutputParameters<T>(T instance, List<SqlParameter> plist)
        {
            SqlParametersHelper.UpdateWithOutputParameters(instance, plist);
        }
        public static List<SqlParameter> ToSqlParamsList<T>(T instance, ICollection<SqlParameter> additionalParams = null, string ignoredParameters = null)
        {
            return SqlParametersHelper.ToSqlParamsList(instance, additionalParams, ignoredParameters);
        }



    }


}
