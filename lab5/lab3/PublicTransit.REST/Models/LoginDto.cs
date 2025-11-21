using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// DTO для входу користувача
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Ім'я користувача або електронна пошта
    /// </summary>
    [Required(ErrorMessage = "Ім'я користувача або електронна пошта обов'язкові")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// Пароль
    /// </summary>
    [Required(ErrorMessage = "Пароль обов'язковий")]
    public string Password { get; set; } = string.Empty;
}

