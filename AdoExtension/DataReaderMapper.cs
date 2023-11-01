using System.Collections.Specialized;
using System.Data;
using System.Reflection;


namespace Adoler
{

    public class DataReaderMapper
    {
        #region GetColumnsName
        public static StringDictionary GetColumnsSchema(IDataReader reader)
        {
            StringDictionary columnsNames = new StringDictionary();
            DataTable dt = reader.GetSchemaTable();
            //---------------------------------
            foreach (DataColumn c in dt.Columns)
            {
                columnsNames.Add(c.ColumnName, null);
            }
            //---------------------------------
            return columnsNames;
        }
        #endregion

        #region --------------GetEntity--------------
        //---------------------------------------------------------------------
        //GetEntity
        //---------------------------------------------------------------------
        /// <summary>
        /// conver datareader object to an entity object
        /// </summary>
        /// <param name="reader">data reader </param>
        /// <param name="t">type of object we need to convert to</param>
        /// <returns></returns>
        public static T GetEntity<T>(IDataReader reader)
        {
            Type t = typeof(T);
            return (T)GetEntity(reader, t);
        }
        //---------------------------------------------------------------------
        public static object GetEntity(IDataReader reader, Type t)
        {
            object obj = Activator.CreateInstance(t);
            //object obj = new t();
            StringDictionary columnsNames = new StringDictionary();
            DataTable dt = reader.GetSchemaTable();
            Type nullableType;
            object value;
            object safeValue;
            //---------------------------------
            string columnname;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnname = reader.GetName(i);
                if (!columnsNames.ContainsKey(columnname))
                {
                    columnsNames.Add(columnname, null);
                    PropertyInfo myPropInfo;
                    myPropInfo = t.GetProperty(columnname);
                    value = reader[columnname];

                    if (value != DBNull.Value && myPropInfo != null && (!myPropInfo.PropertyType.IsClass  || myPropInfo.PropertyType == typeof(string)))
                    {
                        //myPropInfo.SetValue(obj, Convert.ChangeType(value, myPropInfo.PropertyType), null);
                        //if (myPropInfo.PropertyType.BaseType == typeof(System.Enum))
                        if (myPropInfo.PropertyType.IsEnum)
                        {

                            //int intVal = Convert.ToInt32(attr.Value);
                            myPropInfo.SetValue(obj, Enum.Parse(myPropInfo.PropertyType, value.ToString()), null);
                            //Enum.Parse(typeof(myPropInfo.), "FirstName");   
                        }
                        /*
                        else if (value.GetType() == typeof(Byte[]))
                        {
                            byte[] buf = (byte[])value;
                            myPropInfo.SetValue(obj, Convert.ChangeType(OurSerializer.Deserialize(buf), myPropInfo.PropertyType), null);
                        }
                        */
                        else if (Nullable.GetUnderlyingType(myPropInfo.PropertyType) != null)
                        {
                            nullableType = Nullable.GetUnderlyingType(myPropInfo.PropertyType) ?? myPropInfo.PropertyType;
                            if (value != null)
                            {
                                if (nullableType.IsEnum)
                                {
                                    object enumValue = Enum.ToObject(nullableType, value);
                                    myPropInfo.SetValue(obj, enumValue);
                                }
                                else
                                {
                                    safeValue = Convert.ChangeType(value, nullableType);
                                    myPropInfo.SetValue(obj, safeValue);
                                }
                            }
                        }
                        else
                        {
                            myPropInfo.SetValue(obj, Convert.ChangeType(value, myPropInfo.PropertyType), null);
                        }
                    }
                }
            }
            //---------------------------------
            return obj;
        }


        #endregion

    }
}