namespace Repository.Pattern.Abstractions
{
    public interface IRepositoryConfiguration<TDomainModel> where TDomainModel : class, new()
    {
        string TableName { get; set; }
    }  
}
