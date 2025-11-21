namespace PublicTransit.REST.Constants;

/// <summary>
/// Константи для ролей користувачів
/// </summary>
public static class Roles
{
    /// <summary>
    /// Звичайний користувач - може переглядати та створювати записи
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Редактор - може переглядати, створювати та оновлювати записи
    /// </summary>
    public const string Editor = "Editor";

    /// <summary>
    /// Адміністратор - повний доступ до всіх операцій
    /// </summary>
    public const string Admin = "Admin";
}

