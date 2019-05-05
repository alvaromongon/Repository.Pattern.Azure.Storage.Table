namespace Repository.Pattern.Abstractions.Batches
{
    public enum BatchInsertMethod
    {
        Insert = 0,
        InsertOrReplace = 10,
        InsertOrMerge = 20
    }
}
