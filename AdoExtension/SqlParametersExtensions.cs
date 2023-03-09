using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace Adoler
{
    public static class ObjectSqlParametersExtensions
    {
        #region --------------ConvertToParameters--------------
        //---------------------------------------------------------------------
        //ConvertToParameters
        //---------------------------------------------------------------------
        /// <summary>
        /// convert any object properties to a collection of  sql parameters as CustomDbParameterList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static List<SqlParameter> ConvertToParameters<T>(this T instance)
        {
            return SqlParameters.ConvertToParameters(instance);
        }

        //---------------------------------------------------------------------
        #endregion

        #region --------------ConvertToParameters--------------
        //---------------------------------------------------------------------
        //ConvertToParameters
        //---------------------------------------------------------------------
        /// <summary>
        /// convert specific properties to a collection of  sql parameters as CustomDbParameterList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static List<SqlParameter> ConvertToParameters<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParameters.ConvertToParameters(instance, expressions);
        }
        //---------------------------------------------------------------------

        #endregion

        #region --------------ConvertToParametersExcept--------------
        //---------------------------------------------------------------------
        //ConvertToParametersExcept
        //---------------------------------------------------------------------
        /// <summary>
        /// convert any object properties to a collection of  sql parameters as CustomDbParameterList except some properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public static List<SqlParameter> ConvertToParametersExcept<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParameters.ConvertToParametersExcept(instance, expressions);
        }
        //---------------------------------------------------------------------
        #endregion
        public static List<SqlParameter> ConvertToOutputParameters<T>(this T instance)
        {
            return SqlParameters.ConvertToOutputParameters(instance);
        }

        public static List<SqlParameter> ConvertToOutputParameters<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParameters.ConvertToOutputParameters(instance, expressions);
        }

        public static List<SqlParameter> ConvertToOutputParametersExcept<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            return SqlParameters.ConvertToOutputParametersExcept(instance, expressions);
        }

        public static SqlParameter Get(this List<SqlParameter> parameterList, string parameterName)
        {
            return parameterList.Where(e => e.ParameterName == parameterName).FirstOrDefault();
        }
        public static object GetParameterValue(this List<SqlParameter> parameterList, string parameterName)
        {
            var p = parameterList.Where(e => e.ParameterName == parameterName).FirstOrDefault();
            return p?.Value;
        }
        public static void UpdateWithOutputParameters<T>(this T instance, List<SqlParameter> plist)
        {
            SqlParameters.UpdateWithOutputParameters(instance, plist);
        }
        public static List<SqlParameter> ToSqlParamsList(this object obj, SqlParameter[] additionalParams = null, List<string> igoredParameters = null)
        {
            List<SqlParameter> additionalParamsList = (additionalParams != null) ? additionalParams.ToList() : null;
            return ToSqlParamsList(obj, additionalParamsList, igoredParameters);
        }
        public static List<SqlParameter> ToSqlParamsList(this object obj, List<SqlParameter> additionalParams = null, List<string> igoredParameters = null)
        {
            var props = obj.GetType().GetProperties().ToList();

            var plist = new List<SqlParameter>();
            string pName = null;
            object pValue;
            props.ForEach(p =>
            {
                if (igoredParameters != null && igoredParameters.Contains(p.Name))
                    return;
                if (String.IsNullOrWhiteSpace(p.Name))
                    return;
                pValue = p.GetValue(obj) ?? DBNull.Value;
                plist.Add(p.Name, pValue);
            });

            if (additionalParams != null && additionalParams.Count > 0)
                plist.AddRange(additionalParams);

            return plist;

        }






    }


}
