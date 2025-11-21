using System.ComponentModel.DataAnnotations;

namespace PublicTransit.REST.Models;

/// <summary>
/// DTO для реєстрації нового користувача
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Ім'я користувача (використовується для входу)
    /// </summary>
    [Required(ErrorMessage = "Ім'я користувача обов'язкове")]
    [MinLength(3, ErrorMessage = "Ім'я користувача має містити мінімум 3 символи")]
    [MaxLength(50, ErrorMessage = "Ім'я користувача не може перевищувати 50 символів")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Електронна пошта
    /// </summary>
    [Required(ErrorMessage = "Електронна пошта обов'язкова")]
    [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль
    /// </summary>
    [Required(ErrorMessage = "Пароль обов'язковий")]
    [MinLength(6, ErrorMessage = "Пароль має містити мінімум 6 символів")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Підтвердження пароля
    /// </summary>
    [Required(ErrorMessage = "Підтвердження пароля обов'язкове")]
    [Compare(nameof(Password), ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Ім'я користувача
    /// </summary>
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Прізвище користувача
    /// </summary>
    [MaxLength(100)]
    public string? LastName { get; set; }
}

