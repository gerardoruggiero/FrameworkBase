namespace Framework.Utils.Results
{
    public enum EExecutionResultCode
    {
        OK = 1,
        DDBBError = 2,
        BusinessOperation = 3,
        IsValidOperation = 4,
        DeleteError = 5,
        BulkInsertError = 6,
        AddOrUpdateError = 7
    }
}
