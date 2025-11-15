using Microsoft.AspNetCore.Mvc;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure.Models;
using PublicTransit.REST.Models;

namespace PublicTransit.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodSourcesController : ControllerBase
{
    private readonly ICrudServiceAsync<FoodSourceModel> _crudService;

    public FoodSourcesController(ICrudServiceAsync<FoodSourceModel> crudService)
    {
        _crudService = crudService;
    }

    /// <summary>
    /// Отримати всі джерела їжі
    /// </summary>
    /// <response code="200">Повертає список джерел їжі</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FoodSourceResponseDto>>> GetAll()
    {
        var foodSources = await _crudService.ReadAllAsync();
        var result = foodSources.Select(f => MapToResponseDto(f));
        return Ok(result);
    }

    /// <summary>
    /// Отримати джерело їжі за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор джерела їжі</param>
    /// <response code="200">Джерело їжі знайдене</response>
    /// <response code="404">Джерело їжі не знайдене</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodSourceResponseDto>> GetById(Guid id)
    {
        try
        {
            var foodSource = await _crudService.ReadAsync(id);
            return Ok(MapToResponseDto(foodSource));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Створити нове джерело їжі
    /// </summary>
    /// <param name="createDto">Дані для створення джерела їжі</param>
    /// <response code="201">Джерело їжі успішно створене</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FoodSourceResponseDto>> Create([FromBody] FoodSourceCreateDto createDto)
    {
        var foodSource = new FoodSourceModel
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Type = createDto.Type,
            NutritionalValue = createDto.NutritionalValue,
            IsAvailableYearRound = createDto.IsAvailableYearRound
        };

        var created = await _crudService.CreateAsync(foodSource);
        if (!created)
        {
            return BadRequest("Не вдалося створити джерело їжі");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return CreatedAtAction(nameof(GetById), new { id = foodSource.Id }, MapToResponseDto(foodSource));
    }

    /// <summary>
    /// Оновити джерело їжі
    /// </summary>
    /// <param name="id">Ідентифікатор джерела їжі</param>
    /// <param name="updateDto">Дані для оновлення джерела їжі</param>
    /// <response code="204">Джерело їжі успішно оновлене</response>
    /// <response code="404">Джерело їжі не знайдене</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FoodSourceUpdateDto updateDto)
    {
        FoodSourceModel foodSource;
        try
        {
            foodSource = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        foodSource.Name = updateDto.Name;
        foodSource.Type = updateDto.Type;
        foodSource.NutritionalValue = updateDto.NutritionalValue;
        foodSource.IsAvailableYearRound = updateDto.IsAvailableYearRound;

        var updated = await _crudService.UpdateAsync(foodSource);
        if (!updated)
        {
            return BadRequest("Не вдалося оновити джерело їжі");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити джерело їжі
    /// </summary>
    /// <param name="id">Ідентифікатор джерела їжі</param>
    /// <response code="204">Джерело їжі успішно видалене</response>
    /// <response code="404">Джерело їжі не знайдене</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        FoodSourceModel foodSource;
        try
        {
            foodSource = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        var removed = await _crudService.RemoveAsync(foodSource);
        if (!removed)
        {
            return BadRequest("Не вдалося видалити джерело їжі");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    private static FoodSourceResponseDto MapToResponseDto(FoodSourceModel foodSource)
    {
        return new FoodSourceResponseDto
        {
            Id = foodSource.Id,
            Name = foodSource.Name,
            Type = foodSource.Type,
            NutritionalValue = foodSource.NutritionalValue,
            IsAvailableYearRound = foodSource.IsAvailableYearRound
        };
    }
}

