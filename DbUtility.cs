using Microsoft.Data.SqlClient;

namespace CITC.DatabaseUtilities
{
    public class DbUtility
    {
        public static SqlParameter[] GetSqlParamsforOne(string ParameterName, string ParameterValue)
        {
            return new SqlParameter[] { new SqlParameter(ParameterName, ParameterValue) };

        }
    }
}
