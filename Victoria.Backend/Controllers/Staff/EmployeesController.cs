using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Staff;
using Victoria.Domain.Entities.Staff;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Staff;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Employees
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(items.Select(Map));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);
        if (emp == null) return NotFound();

        return Ok(Map(emp));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
    {
        // prosta walidacja
        if (string.IsNullOrWhiteSpace(dto.FullName)) return BadRequest("FullName is required");
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email is required");

        var emailExists = await _db.Employees.AnyAsync(x => x.Email == dto.Email);
        if (emailExists) return BadRequest("Employee with this email already exists");

        var emp = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber ?? "",
            Position = dto.Position ?? "",
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Employees.Add(emp);
        await _db.SaveChangesAsync();

        return Ok(Map(emp));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
    {
        var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);
        if (emp == null) return NotFound();

        var emailTakenByOther = await _db.Employees.AnyAsync(x => x.Email == dto.Email && x.Id != id);
        if (emailTakenByOther) return BadRequest("Employee with this email already exists");

        emp.FullName = dto.FullName;
        emp.Email = dto.Email;
        emp.PhoneNumber = dto.PhoneNumber ?? "";
        emp.Position = dto.Position ?? "";
        emp.IsActive = dto.IsActive;
        emp.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(Map(emp));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")] // usuwać może tylko Admin
    public async Task<IActionResult> Delete(int id)
    {
        var emp = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);
        if (emp == null) return NotFound();

        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static EmployeeGetDto Map(Employee e) => new()
    {
        Id = e.Id,
        FullName = e.FullName,
        Email = e.Email,
        PhoneNumber = e.PhoneNumber,
        Position = e.Position,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
