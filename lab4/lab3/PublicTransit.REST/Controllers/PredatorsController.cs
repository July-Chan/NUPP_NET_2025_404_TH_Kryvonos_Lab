using Microsoft.AspNetCore.Mvc;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure.Models;
using PublicTransit.REST.Models;

namespace PublicTransit.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredatorsController : ControllerBase
{
    private readonly ICrudServiceAsync<PredatorModel> _crudService;

    public PredatorsController(ICrudServiceAsync<PredatorModel> crudService)
    {
        _crudService = crudService;
    }

    /// <summary>
    /// Отримати всіх хижаків
    /// </summary>
    /// <response code="200">Повертає список хижаків</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PredatorResponseDto>>> GetAll()
    {
        var predators = await _crudService.ReadAllAsync();
        var result = predators.Select(p => MapToResponseDto(p));
        return Ok(result);
    }

    /// <summary>
    /// Отримати хижака за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор хижака</param>
    /// <response code="200">Хижак знайдений</response>
    /// <response code="404">Хижак не знайдений</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PredatorResponseDto>> GetById(Guid id)
    {
        try
        {
            var predator = await _crudService.ReadAsync(id);
            return Ok(MapToResponseDto(predator));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Створити нового хижака
    /// </summary>
    /// <param name="createDto">Дані для створення хижака</param>
    /// <response code="201">Хижак успішно створений</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PredatorResponseDto>> Create([FromBody] PredatorCreateDto createDto)
    {
        var predator = new PredatorModel
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Species = createDto.Species,
            HuntingSuccess = createDto.HuntingSuccess,
            Size = createDto.Size
        };

        var created = await _crudService.CreateAsync(predator);
        if (!created)
        {
            return BadRequest("Не вдалося створити хижака");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return CreatedAtAction(nameof(GetById), new { id = predator.Id }, MapToResponseDto(predator));
    }

    /// <summary>
    /// Оновити хижака
    /// </summary>
    /// <param name="id">Ідентифікатор хижака</param>
    /// <param name="updateDto">Дані для оновлення хижака</param>
    /// <response code="204">Хижак успішно оновлений</response>
    /// <response code="404">Хижак не знайдений</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PredatorUpdateDto updateDto)
    {
        PredatorModel predator;
        try
        {
            predator = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        predator.Name = updateDto.Name;
        predator.Species = updateDto.Species;
        predator.HuntingSuccess = updateDto.HuntingSuccess;
        predator.Size = updateDto.Size;

        var updated = await _crudService.UpdateAsync(predator);
        if (!updated)
        {
            return BadRequest("Не вдалося оновити хижака");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити хижака
    /// </summary>
    /// <param name="id">Ідентифікатор хижака</param>
    /// <response code="204">Хижак успішно видалений</response>
    /// <response code="404">Хижак не знайдений</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        PredatorModel predator;
        try
        {
            predator = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        var removed = await _crudService.RemoveAsync(predator);
        if (!removed)
        {
            return BadRequest("Не вдалося видалити хижака");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    private static PredatorResponseDto MapToResponseDto(PredatorModel predator)
    {
        return new PredatorResponseDto
        {
            Id = predator.Id,
            Name = predator.Name,
            Species = predator.Species,
            HuntingSuccess = predator.HuntingSuccess,
            Size = predator.Size
        };
    }
}

