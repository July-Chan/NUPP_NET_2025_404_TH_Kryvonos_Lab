using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PublicTransit.Infrastructure.Models;
using PublicTransit.REST.Constants;
using PublicTransit.REST.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PublicTransit.REST.Controllers;

/// <summary>
/// Контролер для аутентифікації та реєстрації користувачів
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<UserModel> userManager,
        SignInManager<UserModel> signInManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Реєстрація нового користувача
    /// </summary>
    /// <param name="registerDto">Дані для реєстрації</param>
    /// <returns>JWT токен при успішній реєстрації</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Перевірка, чи користувач з таким ім'ям вже існує
        if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
        {
            return BadRequest(new { message = "Користувач з таким ім'ям вже існує" });
        }

        // Перевірка, чи користувач з такою поштою вже існує
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            return BadRequest(new { message = "Користувач з такою електронною поштою вже існує" });
        }

        // Створення нового користувача
        var user = new UserModel
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Помилка при створенні користувача", errors });
        }

        // Призначення ролі User новому користувачу
        await _userManager.AddToRoleAsync(user, Roles.User);

        _logger.LogInformation("Користувач {UserName} успішно зареєстрований з роллю {Role}", registerDto.UserName, Roles.User);

        // Генерація JWT токена
        var token = await GenerateJwtTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes")),
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email
        });
    }

    /// <summary>
    /// Вхід користувача в систему
    /// </summary>
    /// <param name="loginDto">Дані для входу</param>
    /// <returns>JWT токен при успішному вході</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Пошук користувача за ім'ям або електронною поштою
        var user = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail)
            ?? await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);

        if (user == null)
        {
            _logger.LogWarning("Спроба входу з неіснуючим користувачем: {UserNameOrEmail}", loginDto.UserNameOrEmail);
            return Unauthorized(new { message = "Невірне ім'я користувача або пароль" });
        }

        // Перевірка пароля
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Невдала спроба входу для користувача: {UserName}", user.UserName);
            return Unauthorized(new { message = "Невірне ім'я користувача або пароль" });
        }

        _logger.LogInformation("Користувач {UserName} успішно увійшов у систему", user.UserName);

        // Генерація JWT токена
        var token = await GenerateJwtTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes")),
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email
        });
    }

    /// <summary>
    /// Генерація JWT токена для користувача
    /// </summary>
    private async Task<string> GenerateJwtTokenAsync(UserModel user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey не налаштовано");
        var issuer = jwtSettings["Issuer"] ?? "PublicTransitAPI";
        var audience = jwtSettings["Audience"] ?? "PublicTransitAPI";
        var expirationMinutes = jwtSettings.GetValue<int>("ExpirationInMinutes");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        // Додавання ролей користувача (якщо вони є)
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

