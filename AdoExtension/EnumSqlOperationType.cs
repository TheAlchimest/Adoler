namespace Adoler
{
    public enum EnumSqlOperationType
    {
        Create = 1, Update = 2, Delete = 3
    }

    public enum EnumSqlOperationResult
    {
        AlreadyExist = -2,
        Error = -1,
        Undefiend = 0,
        Successeded = 1
    }
}
