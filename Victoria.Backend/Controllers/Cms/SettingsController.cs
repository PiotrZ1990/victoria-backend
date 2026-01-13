using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Victoria.Backend.DTOs.Cms;
using Victoria.Domain.Entities.Cms;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Cms;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // docelowo Staff/Admin
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SettingsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/settings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Settings
            .OrderBy(x => x.Key)
            .Select(x => new SettingListDto
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/settings/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Settings.FirstOrDefaultAsync(s => s.Id == id);
        if (x == null) return NotFound();

        return Ok(new SettingDetailsDto
        {
            Id = x.Id,
            Key = x.Key,
            Value = x.Value,
            Description = x.Description,
            UpdatedAt = x.UpdatedAt
        });
    }

    // GET: api/settings/by-key/{key}
    [HttpGet("by-key/{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var x = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (x == null) return NotFound();

        return Ok(new SettingDetailsDto
        {
            Id = x.Id,
            Key = x.Key,
            Value = x.Value,
            Description = x.Description,
            UpdatedAt = x.UpdatedAt
        });
    }

    // POST: api/settings
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SettingCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Key))
            return BadRequest("Key is required");

        var exists = await _db.Settings.AnyAsync(x => x.Key == dto.Key);
        if (exists)
            return BadRequest("Setting with this Key already exists");

        var entity = new Setting
        {
            Key = dto.Key.Trim(),
            Value = dto.Value,
            Description = dto.Description,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Settings.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/settings/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SettingUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Key))
            return BadRequest("Key is required");

        var entity = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        var keyTrim = dto.Key.Trim();

        var keyTaken = await _db.Settings.AnyAsync(x => x.Id != id && x.Key == keyTrim);
        if (keyTaken)
            return BadRequest("Another setting already uses this Key");

        entity.Key = keyTrim;
        entity.Value = dto.Value;
        entity.Description = dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/settings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Settings.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
