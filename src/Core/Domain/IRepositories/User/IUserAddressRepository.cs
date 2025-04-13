namespace Domain.IRepositories;

public interface IUserAddressRepository : IRepository<UserAddress, int> 
{
    Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserAddress>> SearchByCityAsync(string city);
    Task<IEnumerable<UserAddress>> SearchByCountryAsync(string country);
    Task<IEnumerable<UserAddress>> SearchByPrimaryAsync(bool isPrimary);
}
