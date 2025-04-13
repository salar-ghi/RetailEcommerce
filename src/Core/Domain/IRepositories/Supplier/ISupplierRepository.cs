namespace Domain.IRepositories;

public interface ISupplierRepository : IRepository<Supplier, int> 
{
    Task<IEnumerable<Supplier>> SearchByNameAsync(string name);
    Task<IEnumerable<Supplier>> SearchByContactInfoAsync(string contactInfo);

}
