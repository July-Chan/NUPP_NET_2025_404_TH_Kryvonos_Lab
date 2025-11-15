using Microsoft.AspNetCore.Mvc;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure.Models;
using PublicTransit.Infrastructure.Repositories;
using PublicTransit.REST.Models;

namespace PublicTransit.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FliesController : ControllerBase
{
    private readonly ICrudServiceAsync<FlyModel> _crudService;
    private readonly IFlyRepository _flyRepository;

    public FliesController(ICrudServiceAsync<FlyModel> crudService, IFlyRepository flyRepository)
    {
        _crudService = crudService;
        _flyRepository = flyRepository;
    }

    /// <summary>
    /// Отримати всіх мух
    /// </summary>
    /// <response code="200">Повертає список мух</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlyResponseDto>>> GetAll()
    {
        var flies = await _crudService.ReadAllAsync();
        var result = flies.Select(f => MapToResponseDto(f));
        return Ok(result);
    }

    /// <summary>
    /// Отримати муху за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор мухи</param>
    /// <response code="200">Муха знайдена</response>
    /// <response code="404">Муха не знайдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlyResponseDto>> GetById(Guid id)
    {
        try
        {
            var fly = await _crudService.ReadAsync(id);
            // Отримуємо повну інформацію з деталями через репозиторій
            var flyWithDetails = await _flyRepository.GetByIdWithDetailsAsync(id);
            return Ok(MapToResponseDto(flyWithDetails ?? fly));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Створити нову муху
    /// </summary>
    /// <param name="createDto">Дані для створення мухи</param>
    /// <response code="201">Муха успішно створена</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FlyResponseDto>> Create([FromBody] FlyCreateDto createDto)
    {
        var fly = new FlyModel
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Legs = createDto.Legs,
            WingSpan = createDto.WingSpan,
            FlightSpeed = createDto.FlightSpeed,
            CanHover = createDto.CanHover,
            FoodSourceId = createDto.FoodSourceId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _crudService.CreateAsync(fly);
        if (!created)
        {
            return BadRequest("Не вдалося створити муху");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        var createdFly = await _flyRepository.GetByIdWithDetailsAsync(fly.Id);
        return CreatedAtAction(nameof(GetById), new { id = fly.Id }, MapToResponseDto(createdFly!));
    }

    /// <summary>
    /// Оновити муху
    /// </summary>
    /// <param name="id">Ідентифікатор мухи</param>
    /// <param name="updateDto">Дані для оновлення мухи</param>
    /// <response code="204">Муха успішно оновлена</response>
    /// <response code="404">Муха не знайдена</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] FlyUpdateDto updateDto)
    {
        FlyModel fly;
        try
        {
            fly = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        fly.Name = updateDto.Name;
        fly.Legs = updateDto.Legs;
        fly.WingSpan = updateDto.WingSpan;
        fly.FlightSpeed = updateDto.FlightSpeed;
        fly.CanHover = updateDto.CanHover;
        fly.FoodSourceId = updateDto.FoodSourceId;

        var updated = await _crudService.UpdateAsync(fly);
        if (!updated)
        {
            return BadRequest("Не вдалося оновити муху");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити муху
    /// </summary>
    /// <param name="id">Ідентифікатор мухи</param>
    /// <response code="204">Муха успішно видалена</response>
    /// <response code="404">Муха не знайдена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        FlyModel fly;
        try
        {
            fly = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        var removed = await _crudService.RemoveAsync(fly);
        if (!removed)
        {
            return BadRequest("Не вдалося видалити муху");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Отримати мух, які можуть зависати в повітрі
    /// </summary>
    /// <response code="200">Повертає список мух, які можуть зависати</response>
    [HttpGet("hovering")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlyResponseDto>>> GetHoveringFlies()
    {
        var flies = await _flyRepository.GetHoveringFliesAsync();
        var result = flies.Select(f => MapToResponseDto(f));
        return Ok(result);
    }

    /// <summary>
    /// Отримати мух за діапазоном швидкості польоту
    /// </summary>
    /// <param name="minSpeed">Мінімальна швидкість</param>
    /// <param name="maxSpeed">Максимальна швидкість</param>
    /// <response code="200">Повертає список мух у вказаному діапазоні швидкості</response>
    [HttpGet("speed-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FlyResponseDto>>> GetByFlightSpeedRange(
        [FromQuery] int minSpeed,
        [FromQuery] int maxSpeed)
    {
        var flies = await _flyRepository.GetByFlightSpeedRangeAsync(minSpeed, maxSpeed);
        var result = flies.Select(f => MapToResponseDto(f));
        return Ok(result);
    }

    private static FlyResponseDto MapToResponseDto(FlyModel fly)
    {
        return new FlyResponseDto
        {
            Id = fly.Id,
            Name = fly.Name,
            Legs = fly.Legs,
            CreatedAt = fly.CreatedAt,
            WingSpan = fly.WingSpan,
            FlightSpeed = fly.FlightSpeed,
            CanHover = fly.CanHover,
            FoodSourceId = fly.FoodSourceId,
            FoodSourceName = fly.FoodSource?.Name,
            HabitatId = fly.HabitatId,
            HabitatLocation = fly.Habitat?.Location
        };
    }
}

