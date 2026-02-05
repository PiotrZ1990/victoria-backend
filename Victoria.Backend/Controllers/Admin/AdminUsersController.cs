using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Admin;
using Victoria.Infrastructure.Identity;

namespace Victoria.Backend.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: api/admin/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var result = new List<AdminUserListDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);

            result.Add(new AdminUserListDto
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName ?? "",
                StudentId = u.StudentId,
                Roles = roles.ToList()
            });
        }

        return Ok(result);
    }

    // GET: api/admin/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var u = await _userManager.FindByIdAsync(id);
        if (u == null) return NotFound("User not found");

        var roles = await _userManager.GetRolesAsync(u);

        return Ok(new AdminUserListDto
        {
            Id = u.Id,
            Email = u.Email ?? "",
            FullName = u.FullName ?? "",
            StudentId = u.StudentId,
            Roles = roles.ToList()
        });
    }

    // GET: api/admin/users/roles
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();

        return Ok(roles);
    }

    // PUT: api/admin/users/{id}/roles
    [HttpPut("{id}/roles")]
    public async Task<IActionResult> UpdateRoles(string id, [FromBody] AdminUserRolesUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("User not found");

        var newRoles = (dto.Roles ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // upewnij się że role istnieją (jak nie - twórz)
        foreach (var r in newRoles)
        {
            if (!await _roleManager.RoleExistsAsync(r))
                await _roleManager.CreateAsync(new IdentityRole(r));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeRes.Succeeded)
            return BadRequest(string.Join(" | ", removeRes.Errors.Select(e => e.Description)));

        var addRes = await _userManager.AddToRolesAsync(user, newRoles);
        if (!addRes.Succeeded)
            return BadRequest(string.Join(" | ", addRes.Errors.Select(e => e.Description)));

        var finalRoles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            userId = user.Id,
            roles = finalRoles.ToList()
        });
    }
}
