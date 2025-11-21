using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure;
using PublicTransit.Infrastructure.Models;
using PublicTransit.REST.Constants;
using PublicTransit.REST.Models;

namespace PublicTransit.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitatsController : ControllerBase
{
    private readonly ICrudServiceAsync<HabitatModel> _crudService;
    private readonly PublicTransitContext _context;

    public HabitatsController(ICrudServiceAsync<HabitatModel> crudService, PublicTransitContext context)
    {
        _crudService = crudService;
        _context = context;
    }

    /// <summary>
    /// Отримати всі середовища проживання
    /// </summary>
    /// <response code="200">Повертає список середовищ проживання</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HabitatResponseDto>>> GetAll()
    {
        var habitats = await _crudService.ReadAllAsync();
        var habitatsWithDetails = await _context.Habitats
            .Include(h => h.Insect)
            .Where(h => habitats.Select(x => x.Id).Contains(h.Id))
            .ToListAsync();
        var result = habitatsWithDetails.Select(h => MapToResponseDto(h));
        return Ok(result);
    }

    /// <summary>
    /// Отримати середовище проживання за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор середовища проживання</param>
    /// <response code="200">Середовище проживання знайдене</response>
    /// <response code="404">Середовище проживання не знайдене</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitatResponseDto>> GetById(Guid id)
    {
        try
        {
            var habitat = await _crudService.ReadAsync(id);
            var habitatWithDetails = await _context.Habitats
                .Include(h => h.Insect)
                .FirstOrDefaultAsync(h => h.Id == id);
            
            return Ok(MapToResponseDto(habitatWithDetails ?? habitat));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Створити нове середовище проживання
    /// </summary>
    /// <param name="createDto">Дані для створення середовища проживання</param>
    /// <response code="201">Середовище проживання успішно створене</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPost]
    [Authorize(Roles = $"{Roles.User},{Roles.Editor},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<HabitatResponseDto>> Create([FromBody] HabitatCreateDto createDto)
    {
        var habitat = new HabitatModel
        {
            Id = Guid.NewGuid(),
            Location = createDto.Location,
            Climate = createDto.Climate,
            Temperature = createDto.Temperature,
            Humidity = createDto.Humidity,
            InsectId = createDto.InsectId
        };

        var created = await _crudService.CreateAsync(habitat);
        if (!created)
        {
            return BadRequest("Не вдалося створити середовище проживання");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        var createdHabitat = await _context.Habitats
            .Include(h => h.Insect)
            .FirstOrDefaultAsync(h => h.Id == habitat.Id);

        return CreatedAtAction(nameof(GetById), new { id = habitat.Id }, MapToResponseDto(createdHabitat!));
    }

    /// <summary>
    /// Оновити середовище проживання
    /// </summary>
    /// <param name="id">Ідентифікатор середовища проживання</param>
    /// <param name="updateDto">Дані для оновлення середовища проживання</param>
    /// <response code="204">Середовище проживання успішно оновлене</response>
    /// <response code="404">Середовище проживання не знайдене</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Editor},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] HabitatUpdateDto updateDto)
    {
        HabitatModel habitat;
        try
        {
            habitat = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        habitat.Location = updateDto.Location;
        habitat.Climate = updateDto.Climate;
        habitat.Temperature = updateDto.Temperature;
        habitat.Humidity = updateDto.Humidity;
        habitat.InsectId = updateDto.InsectId;

        var updated = await _crudService.UpdateAsync(habitat);
        if (!updated)
        {
            return BadRequest("Не вдалося оновити середовище проживання");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити середовище проживання
    /// </summary>
    /// <param name="id">Ідентифікатор середовища проживання</param>
    /// <response code="204">Середовище проживання успішно видалене</response>
    /// <response code="404">Середовище проживання не знайдене</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        HabitatModel habitat;
        try
        {
            habitat = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        var removed = await _crudService.RemoveAsync(habitat);
        if (!removed)
        {
            return BadRequest("Не вдалося видалити середовище проживання");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    private static HabitatResponseDto MapToResponseDto(HabitatModel habitat)
    {
        return new HabitatResponseDto
        {
            Id = habitat.Id,
            Location = habitat.Location,
            Climate = habitat.Climate,
            Temperature = habitat.Temperature,
            Humidity = habitat.Humidity,
            InsectId = habitat.InsectId,
            InsectName = habitat.Insect?.Name
        };
    }
}

