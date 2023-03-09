using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Adoler
{
    public static class SqlParameters
    {
        #region Convert To Input parameters 
        public static List<SqlParameter> ConvertToParameters<T>(T instance)
        {
            return ConvertToParameters<T>(instance, ParameterDirection.Input);
        }

        public static List<SqlParameter> ConvertToParameters<T>(T instance, params Expression<Func<T, object>>[] expressions)
        {
            return ConvertToParameters<T>(instance, ParameterDirection.Input, expressions);
        }

        public static List<SqlParameter> ConvertToParametersExcept<T>(T instance, params Expression<Func<T, object>>[] expressions)
        {
            return ConvertToParametersExcept<T>(instance, ParameterDirection.Input, expressions);
        }


        public static List<SqlParameter> ConvertDynamicToParameters(ExpandoObject instance)
        {
            return ConvertDynamicToParameters(instance, ParameterDirection.Input);
        }
        #endregion

        #region Convert To Input parameters 

        public static List<SqlParameter> ConvertToOutputParameters<T>(T instance)
        {
            return ConvertToParameters<T>(instance, ParameterDirection.Output);
        }
        public static List<SqlParameter> ConvertToOutputParameters<T>(T instance, params Expression<Func<T, object>>[] expressions)
        {
            return ConvertToParameters<T>(instance, ParameterDirection.Output, expressions);
        }
        public static List<SqlParameter> ConvertToOutputParametersExcept<T>(T instance, params Expression<Func<T, object>>[] expressions)
        {
            return ConvertToParametersExcept<T>(instance, ParameterDirection.Output, expressions);
        }
        public static List<SqlParameter> ConvertDynamicToOutputParameters(ExpandoObject instance)
        {
            return ConvertDynamicToParameters(instance, ParameterDirection.Output);
        }
        #endregion



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
        public static List<SqlParameter> ConvertToParameters<T>(T instance, ParameterDirection direction)
        {
            string name = null;
            object value;
            List<SqlParameter> plist = new List<SqlParameter>();
            Type t = instance.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                name = prop.Name;
                value = prop.GetValue(instance);
                plist.Add(name, value, direction);
            }
            return plist;
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

        public static List<SqlParameter> ConvertToParameters<T>(T instance, ParameterDirection direction, params Expression<Func<T, object>>[] expressions)
        {
            List<SqlParameter> parameterlist = new List<SqlParameter>();
            List<PropertyInfo> propList = ExpressionHelper.GetProperiesFromExpression(expressions);
            foreach (PropertyInfo prop in propList)
            {
                parameterlist.Add(prop, prop.GetValue(instance), direction);
            }
            return parameterlist;
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

        public static List<SqlParameter> ConvertToParametersExcept<T>(T instance, ParameterDirection direction, params Expression<Func<T, object>>[] expressions)
        {
            List<SqlParameter> parameterlist = new List<SqlParameter>();
            List<PropertyInfo> execludedpropList = ExpressionHelper.GetProperiesFromExpression(expressions);
            List<string> execludedpropListNames = execludedpropList.Select(e => e.Name).ToList();
            Type t = instance.GetType();
            PropertyInfo[] propList = t.GetProperties();
            foreach (var prop in propList)
            {
                if (expressions == null || !execludedpropListNames.Contains(prop.Name))
                {
                    parameterlist.Add(prop, prop.GetValue(instance), direction);
                }
            }
            return parameterlist;
        }
        //---------------------------------------------------------------------
        #endregion

        public static List<SqlParameter> ConvertDynamicToParameters(ExpandoObject instance, ParameterDirection direction)
        {
            string name = null;
            object value;
            IDictionary<string, object> propertyValues = instance;
            List<SqlParameter> plist = new List<SqlParameter>();
            foreach (var property in propertyValues.Keys)
            {
                name = property;
                value = propertyValues[property];
                plist.Add(name, value, direction);
            }
            return plist;
        }


        #region --------------UpdateWithOutputParameters--------------
        /// <summary>
        /// Updates the with output parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="plist">The plist.</param>
        public static void UpdateWithOutputParameters<T>(T instance, List<SqlParameter> plist)
        {
            Type t = instance.GetType();
            PropertyInfo[] properties = t.GetProperties();
            PropertyInfo prop;
            foreach (var parameter in plist.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput))
            {
                prop = t.GetProperty(parameter.ParameterName);
                if (prop != null && prop.CanWrite)
                {
                    if (parameter.Value != DBNull.Value)
                    {
                        prop.SetValue(instance, parameter.Value);
                    }

                }
            }

        }
        #endregion
    }
}
