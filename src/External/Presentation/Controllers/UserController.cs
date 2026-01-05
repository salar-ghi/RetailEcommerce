namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly RoleService _roleService;
    private readonly UserRoleService _userRoleService;
    private readonly UserAddressService _addressService;


    public UserController(ICurrentUserService currentUserService,
        IUserService userService,
        RoleService roleService,
        UserRoleService userRoleService,
        UserAddressService addressService)
    {
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _userService = userService;
        _roleService = roleService;
        _userRoleService = userRoleService;
        _addressService = addressService;
    }


    // User CRUD Operations
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            // Log all model errors
            var errors = ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            Console.Clear();
            Console.WriteLine("Model validation failed: {@Errors}", errors);
            return ValidationProblem(ModelState); // same format as automatic 400
        }
        try
        {
            var user = await _userService.AddUserAsync(userDto);
            return Ok(new { Message = "User added successfully", UserId = user.Value });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, UserDto userDto)
    {
        if (id != userDto.Id) return BadRequest();
        await _userService.UpdateUserAsync(userDto);
        return NoContent();
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpGet("users/search/username")]
    public async Task<IActionResult> SearchUsersByUsername(string username)
    {
        var users = await _userService.SearchUsersByUsernameAsync(username);
        return Ok(users);
    }

    [HttpGet("users/search/email")]
    public async Task<IActionResult> SearchUsersByEmail(string email)
    {
        var users = await _userService.SearchUsersByEmailAsync(email);
        return Ok(users);
    }

    // Role CRUD Operations
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("roles/{id}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        return Ok(role);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> AddRole(RoleDto roleDto)
    {
        await _roleService.AddRoleAsync(roleDto);
        //return Ok("role added successfully");
        return CreatedAtAction(nameof(GetRoleById), new { id = roleDto.Id }, roleDto);
    }

    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(int id, RoleDto roleDto)
    {
        if (id != roleDto.Id) return BadRequest();
        await _roleService.UpdateRoleAsync(roleDto);
        return NoContent();
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }

    [HttpGet("roles/search/name")]
    public async Task<IActionResult> SearchRolesByName(string name)
    {
        var roles = await _roleService.SearchRolesByNameAsync(name);
        return Ok(roles);
    }

    // UserRole CRUD Operations
    [HttpGet("user-roles")]
    public async Task<IActionResult> GetAllUserRoles()
    {
        var userRoles = await _userRoleService.GetAllUserRolesAsync();
        return Ok(userRoles);
    }

    [HttpGet("user-roles/{userId}/{roleId}")]
    public async Task<IActionResult> GetUserRoleById(string userId, int roleId)
    {
        var userRole = await _userRoleService.GetUserRoleByIdAsync(userId, roleId);
        return Ok(userRole);
    }

    [HttpPost("user-roles")]
    public async Task<IActionResult> AddUserRole(UserRoleDto userRoleDto)
    {
        await _userRoleService.AddUserRoleAsync(userRoleDto);
        return CreatedAtAction(nameof(GetUserRoleById), new { userId = userRoleDto.UserId, roleId = userRoleDto.RoleId }, userRoleDto);
    }

    [HttpPut("user-roles/{userId}/{roleId}")]
    public async Task<IActionResult> UpdateUserRole(string userId, int roleId, UserRoleDto userRoleDto)
    {
        if (userId != userRoleDto.UserId || roleId != userRoleDto.RoleId) return BadRequest();
        await _userRoleService.UpdateUserRoleAsync(userRoleDto);
        return NoContent();
    }

    [HttpDelete("user-roles/{userId}/{roleId}")]
    public async Task<IActionResult> DeleteUserRole(string userId, int roleId)
    {
        await _userRoleService.DeleteUserRoleAsync(userId, roleId);
        return NoContent();
    }

    [HttpGet("user-roles/search/user")]
    public async Task<IActionResult> SearchUserRolesByUserId(string userId)
    {
        var userRoles = await _userRoleService.SearchUserRolesByUserIdAsync(userId);
        return Ok(userRoles);
    }

    [HttpGet("user-roles/search/role")]
    public async Task<IActionResult> SearchUserRolesByRoleId(int roleId)
    {
        var userRoles = await _userRoleService.SearchUserRolesByRoleIdAsync(roleId);
        return Ok(userRoles);
    }

    // UserAddress CRUD Operations
    [HttpGet("addresses")]
    public async Task<IActionResult> GetAllAddresses()
    {
        var addresses = await _addressService.GetAllAddressesAsync();
        return Ok(addresses);
    }

    [HttpGet("addresses/{id}")]
    public async Task<IActionResult> GetAddressById(int id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);
        return Ok(address);
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress(UserAddressDto addressDto)
    {
        await _addressService.AddAddressAsync(addressDto);
        return CreatedAtAction(nameof(GetAddressById), new { id = addressDto.Id }, addressDto);
    }

    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(int id, UserAddressDto addressDto)
    {
        if (id != addressDto.Id) return BadRequest();
        await _addressService.UpdateAddressAsync(addressDto);
        return NoContent();
    }

    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _addressService.DeleteAddressAsync(id);
        return NoContent();
    }

    [HttpGet("addresses/search/user")]
    public async Task<IActionResult> SearchAddressesByUserId(string userId)
    {
        var addresses = await _addressService.SearchAddressesByUserIdAsync(userId);
        return Ok(addresses);
    }

    [HttpGet("addresses/search/city")]
    public async Task<IActionResult> SearchAddressesByCity(string city)
    {
        var addresses = await _addressService.SearchAddressesByCityAsync(city);
        return Ok(addresses);
    }

    [HttpGet("addresses/search/country")]
    public async Task<IActionResult> SearchAddressesByCountry(string country)
    {
        var addresses = await _addressService.SearchAddressesByCountryAsync(country);
        return Ok(addresses);
    }

    [HttpGet("addresses/search/primary")]
    public async Task<IActionResult> SearchAddressesByPrimary(bool isPrimary)
    {
        var addresses = await _addressService.SearchAddressesByPrimaryAsync(isPrimary);
        return Ok(addresses);
    }


}
