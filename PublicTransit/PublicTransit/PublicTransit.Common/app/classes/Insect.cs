using System.Text.Json.Serialization;

[JsonDerivedType(typeof(Fly), typeDiscriminator: "fly")]
[JsonDerivedType(typeof(Spider), typeDiscriminator: "spider")]
public abstract class Insect
{
    // === ДЕЛЕГАТ ТА ПОДiЯ ===
    public delegate void InsectBornEventHandler(object sender, InsectEventArgs e);
    public static event InsectBornEventHandler? OnInsectBorn;

    // === СТАТИЧНi ПОЛЯ i КОНСТРУКТОРИ ===
    private static int TotalInsects;

    static Insect()
    {
        TotalInsects = 0;
    }

    // === ВЛАСТИВОСТi ===
    public string Name { get; set; }
    public int Legs { get; set; }

    // === КОНСТРУКТОР ===
    protected Insect(string name, int legs)
    {
        Name = name;
        Legs = legs;
        TotalInsects++;
        OnInsectBorn?.Invoke(this, new InsectEventArgs(Name));
    }

    // === АБСТРАКТНИЙ МЕТОД ===
    public abstract void Move();

    // === СТАТИЧНИЙ МЕТОД ===
    public static int GetTotalInsectsCount()
    {
        return TotalInsects;
    }

    // === ПОВЕРНЕННЯ iНФОРМАЦiЇ ===
    public override string ToString()
    {
        return $"Назва: {Name}, Кiлькiсть нiг: {Legs}";
    }
}

public class InsectEventArgs : EventArgs
{
    public string InsectName { get; }

    public InsectEventArgs(string insectName)
    {
        InsectName = insectName;
    }
}
