using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace CITC.DatabaseUtilities.AdoExtension
{
    /// <summary>
    /// custom parameter list is a collection class that holds sql parameters
    /// </summary>
    public class DbParameterList
    {
        public List<SqlParameter> Parameters { get; set; }
        public List<Object> ParametersValues { get; set; }
        public List<SqlParameter> OutputParameters { get; set; }

        #region --------------Constructor--------------
        //---------------------------------------------
        //Constructor
        //---------------------------------------------
        public DbParameterList()
        {
            Parameters = new List<SqlParameter>();
         OutputParameters = new List<SqlParameter>();

        }
    //---------------------------------------------
    #endregion

       
    }
}