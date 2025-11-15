using Microsoft.AspNetCore.Mvc;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure.Models;
using PublicTransit.Infrastructure.Repositories;
using PublicTransit.REST.Models;

namespace PublicTransit.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpidersController : ControllerBase
{
    private readonly ICrudServiceAsync<SpiderModel> _crudService;
    private readonly ISpiderRepository _spiderRepository;

    public SpidersController(ICrudServiceAsync<SpiderModel> crudService, ISpiderRepository spiderRepository)
    {
        _crudService = crudService;
        _spiderRepository = spiderRepository;
    }

    /// <summary>
    /// Отримати всіх павуків
    /// </summary>
    /// <response code="200">Повертає список павуків</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SpiderResponseDto>>> GetAll()
    {
        var spiders = await _crudService.ReadAllAsync();
        var result = spiders.Select(s => MapToResponseDto(s));
        return Ok(result);
    }

    /// <summary>
    /// Отримати павука за ідентифікатором
    /// </summary>
    /// <param name="id">Ідентифікатор павука</param>
    /// <response code="200">Павук знайдений</response>
    /// <response code="404">Павук не знайдений</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpiderResponseDto>> GetById(Guid id)
    {
        try
        {
            var spider = await _crudService.ReadAsync(id);
            var spiderWithDetails = await _spiderRepository.GetByIdWithDetailsAsync(id);
            return Ok(MapToResponseDto(spiderWithDetails ?? spider));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Створити нового павука
    /// </summary>
    /// <param name="createDto">Дані для створення павука</param>
    /// <response code="201">Павук успішно створений</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpiderResponseDto>> Create([FromBody] SpiderCreateDto createDto)
    {
        var spider = new SpiderModel
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Legs = createDto.Legs,
            IsPoisonous = createDto.IsPoisonous,
            WebStrength = createDto.WebStrength,
            VenomPotency = createDto.VenomPotency,
            FoodSourceId = createDto.FoodSourceId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _crudService.CreateAsync(spider);
        if (!created)
        {
            return BadRequest("Не вдалося створити павука");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        var createdSpider = await _spiderRepository.GetByIdWithDetailsAsync(spider.Id);
        return CreatedAtAction(nameof(GetById), new { id = spider.Id }, MapToResponseDto(createdSpider!));
    }

    /// <summary>
    /// Оновити павука
    /// </summary>
    /// <param name="id">Ідентифікатор павука</param>
    /// <param name="updateDto">Дані для оновлення павука</param>
    /// <response code="204">Павук успішно оновлений</response>
    /// <response code="404">Павук не знайдений</response>
    /// <response code="400">Невірні дані запиту</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] SpiderUpdateDto updateDto)
    {
        SpiderModel spider;
        try
        {
            spider = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        spider.Name = updateDto.Name;
        spider.Legs = updateDto.Legs;
        spider.IsPoisonous = updateDto.IsPoisonous;
        spider.WebStrength = updateDto.WebStrength;
        spider.VenomPotency = updateDto.VenomPotency;
        spider.FoodSourceId = updateDto.FoodSourceId;

        var updated = await _crudService.UpdateAsync(spider);
        if (!updated)
        {
            return BadRequest("Не вдалося оновити павука");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Видалити павука
    /// </summary>
    /// <param name="id">Ідентифікатор павука</param>
    /// <response code="204">Павук успішно видалений</response>
    /// <response code="404">Павук не знайдений</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        SpiderModel spider;
        try
        {
            spider = await _crudService.ReadAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        var removed = await _crudService.RemoveAsync(spider);
        if (!removed)
        {
            return BadRequest("Не вдалося видалити павука");
        }

        var saved = await _crudService.SaveAsync();
        if (!saved)
        {
            return BadRequest("Не вдалося зберегти зміни");
        }

        return NoContent();
    }

    /// <summary>
    /// Отримати отруйних павуків
    /// </summary>
    /// <response code="200">Повертає список отруйних павуків</response>
    [HttpGet("poisonous")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SpiderResponseDto>>> GetPoisonousSpiders()
    {
        var spiders = await _spiderRepository.GetPoisonousSpidersAsync();
        var result = spiders.Select(s => MapToResponseDto(s));
        return Ok(result);
    }

    /// <summary>
    /// Отримати павуків за мінімальною міцністю павутини
    /// </summary>
    /// <param name="minStrength">Мінімальна міцність павутини</param>
    /// <response code="200">Повертає список павуків з міцністю павутини не менше вказаної</response>
    [HttpGet("web-strength")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SpiderResponseDto>>> GetByMinWebStrength(
        [FromQuery] double minStrength)
    {
        var spiders = await _spiderRepository.GetByMinWebStrengthAsync(minStrength);
        var result = spiders.Select(s => MapToResponseDto(s));
        return Ok(result);
    }

    private static SpiderResponseDto MapToResponseDto(SpiderModel spider)
    {
        return new SpiderResponseDto
        {
            Id = spider.Id,
            Name = spider.Name,
            Legs = spider.Legs,
            CreatedAt = spider.CreatedAt,
            IsPoisonous = spider.IsPoisonous,
            WebStrength = spider.WebStrength,
            VenomPotency = spider.VenomPotency,
            FoodSourceId = spider.FoodSourceId,
            FoodSourceName = spider.FoodSource?.Name,
            HabitatId = spider.HabitatId,
            HabitatLocation = spider.Habitat?.Location
        };
    }
}

