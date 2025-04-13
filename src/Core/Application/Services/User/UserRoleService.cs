namespace Application.Services;

public class UserRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserRoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync()
    {
        var userRoles = await _unitOfWork.UserRoles.GetAllAsync();
        return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
    }

    public async Task<UserRoleDto> GetUserRoleByIdAsync(int userId, int roleId)
    {
        var userRoles = await _unitOfWork.UserRoles.GetAllAsync();
        var userRole = userRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole == null) throw new KeyNotFoundException($"UserRole with User ID {userId} and Role ID {roleId} not found.");
        return _mapper.Map<UserRoleDto>(userRole);
    }

    public async Task AddUserRoleAsync(UserRoleDto userRoleDto)
    {
        var userRole = _mapper.Map<UserRole>(userRoleDto);
        await _unitOfWork.UserRoles.AddAsync(userRole);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateUserRoleAsync(UserRoleDto userRoleDto)
    {
        var userRoles = await _unitOfWork.UserRoles.GetAllAsync();
        var existing = userRoles.FirstOrDefault(ur => ur.UserId == userRoleDto.UserId && ur.RoleId == userRoleDto.RoleId);
        if (existing == null) throw new KeyNotFoundException($"UserRole with User ID {userRoleDto.UserId} and Role ID {userRoleDto.RoleId} not found.");
        _mapper.Map(userRoleDto, existing);
        await _unitOfWork.UserRoles.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUserRoleAsync(int userId, int roleId)
    {
        await _unitOfWork.UserRoles.DeleteAsync(userId, roleId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserRoleDto>> SearchUserRolesByUserIdAsync(int userId)
    {
        var userRoles = await _unitOfWork.UserRoles.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
    }

    public async Task<IEnumerable<UserRoleDto>> SearchUserRolesByRoleIdAsync(int roleId)
    {
        var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
        return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
    }
}