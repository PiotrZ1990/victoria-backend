using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Victoria.Backend.DTOs.Cms;
using Victoria.Domain.Entities.CMS;
using Victoria.Infrastructure.Data;

namespace Victoria.Backend.Controllers.Cms;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public class TestimonialsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TestimonialsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/testimonials
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Testimonials
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TestimonialListDto
            {
                Id = x.Id,
                AuthorName = x.AuthorName,
                Rating = x.Rating,
                Country = x.Country,
                IsPublished = x.IsPublished,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/testimonials/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var x = await _db.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
        if (x == null) return NotFound();

        return Ok(new TestimonialDetailsDto
        {
            Id = x.Id,
            AuthorName = x.AuthorName,
            AuthorTitle = x.AuthorTitle,
            Country = x.Country,
            Content = x.Content,
            Rating = x.Rating,
            IsPublished = x.IsPublished,
            CreatedAt = x.CreatedAt
        });
    }

    // POST: api/testimonials
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TestimonialCreateDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        var entity = new Testimonial
        {
            AuthorName = dto.AuthorName,
            AuthorTitle = dto.AuthorTitle,
            Country = dto.Country,
            Content = dto.Content,
            Rating = dto.Rating,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        _db.Testimonials.Add(entity);
        await _db.SaveChangesAsync();

        return Ok(new { entity.Id });
    }

    // PUT: api/testimonials/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TestimonialUpdateDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        var entity = await _db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        entity.AuthorName = dto.AuthorName;
        entity.AuthorTitle = dto.AuthorTitle;
        entity.Country = dto.Country;
        entity.Content = dto.Content;
        entity.Rating = dto.Rating;
        entity.IsPublished = dto.IsPublished;

        await _db.SaveChangesAsync();
        return Ok(new { entity.Id });
    }

    // DELETE: api/testimonials/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Testimonials.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        _db.Testimonials.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
