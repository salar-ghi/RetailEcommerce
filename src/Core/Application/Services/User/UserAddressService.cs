namespace Application.Services;

// Application/Services/UserAddressService.cs
public class UserAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserAddressService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserAddressDto>> GetAllAddressesAsync()
    {
        var addresses = await _unitOfWork.UserAddresses.GetAllAsync();
        return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
    }

    public async Task<UserAddressDto> GetAddressByIdAsync(int id)
    {
        var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);
        if (address == null) throw new KeyNotFoundException($"Address with ID {id} not found.");
        return _mapper.Map<UserAddressDto>(address);
    }

    public async Task AddAddressAsync(UserAddressDto addressDto)
    {
        var address = _mapper.Map<UserAddress>(addressDto);
        await _unitOfWork.UserAddresses.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAddressAsync(UserAddressDto addressDto)
    {
        var address = await _unitOfWork.UserAddresses.GetByIdAsync(addressDto.Id);
        if (address == null) throw new KeyNotFoundException($"Address with ID {addressDto.Id} not found.");
        _mapper.Map(addressDto, address);
        await _unitOfWork.UserAddresses.UpdateAsync(address);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAddressAsync(int id)
    {
        var address = await _unitOfWork.UserAddresses.GetByIdAsync(id);
        if (address != null)
        {
            await _unitOfWork.UserAddresses.DeleteAsync(address);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UserAddressDto>> SearchAddressesByUserIdAsync(int userId)
    {
        var addresses = await _unitOfWork.UserAddresses.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
    }

    public async Task<IEnumerable<UserAddressDto>> SearchAddressesByCityAsync(string city)
    {
        var addresses = await _unitOfWork.UserAddresses.SearchByCityAsync(city);
        return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
    }

    public async Task<IEnumerable<UserAddressDto>> SearchAddressesByCountryAsync(string country)
    {
        var addresses = await _unitOfWork.UserAddresses.SearchByCountryAsync(country);
        return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
    }

    public async Task<IEnumerable<UserAddressDto>> SearchAddressesByPrimaryAsync(bool isPrimary)
    {
        var addresses = await _unitOfWork.UserAddresses.SearchByPrimaryAsync(isPrimary);
        return _mapper.Map<IEnumerable<UserAddressDto>>(addresses);
    }
}