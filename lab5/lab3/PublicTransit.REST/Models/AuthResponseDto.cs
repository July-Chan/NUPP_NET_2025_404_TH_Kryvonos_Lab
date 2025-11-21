namespace PublicTransit.REST.Models;

/// <summary>
/// DTO для відповіді після успішної аутентифікації
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT токен для авторизації
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Дата закінчення дії токена
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Ідентифікатор користувача
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Ім'я користувача
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Електронна пошта
    /// </summary>
    public string? Email { get; set; }
}

