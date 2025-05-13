using Domain.Entities;

namespace Application.Services;
public class RoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto> GetRoleByIdAsync(int id)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id);
        if (role == null) throw new KeyNotFoundException($"Role with ID {id} not found.");
        return _mapper.Map<RoleDto>(role);
    }

    public async Task AddRoleAsync(RoleDto roleDto)
    {
        var role = _mapper.Map<Role>(roleDto);
        
        role.CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        role.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        role.ModifiedTime = DateTime.Now;

        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(RoleDto roleDto)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(roleDto.Id);
        if (role == null) throw new KeyNotFoundException($"Role with ID {roleDto.Id} not found.");
        _mapper.Map(roleDto, role);
        await _unitOfWork.Roles.UpdateAsync(role);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteRoleAsync(int id)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(id);
        if (role != null)
        {
            await _unitOfWork.Roles.DeleteAsync(role);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<RoleDto>> SearchRolesByNameAsync(string name)
    {
        var roles = await _unitOfWork.Roles.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }
}