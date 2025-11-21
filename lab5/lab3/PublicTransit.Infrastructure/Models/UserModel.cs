using Microsoft.AspNetCore.Identity;

namespace PublicTransit.Infrastructure.Models;

/// <summary>
/// Модель користувача системи PublicTransit
/// Наслідується від IdentityUser для інтеграції з ASP.NET Core Identity
/// </summary>
public class UserModel : IdentityUser
{
    /// <summary>
    /// Ім'я користувача
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Прізвище користувача
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Дата реєстрації користувача
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата останнього оновлення профілю
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

