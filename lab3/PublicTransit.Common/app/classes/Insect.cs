using PublicTransit.Common.App.Crud;
using System.Text.Json.Serialization;

namespace PublicTransit.Common.App.Classes;

[JsonDerivedType(typeof(Fly), typeDiscriminator: "fly")]
[JsonDerivedType(typeof(Spider), typeDiscriminator: "spider")]
public abstract class Insect : IWithId
{
    public Guid Id { get; } = Guid.NewGuid();
    // === ДЕЛЕГАТ ТА ПОДІЯ ===
    public delegate void InsectBornEventHandler(object sender, InsectEventArgs e);
    public static event InsectBornEventHandler? OnInsectBorn;

    // === СТАТИЧНІ ПОЛЯ І КОНСТРУКТОРИ ===
    private static int TotalInsects;

    static Insect()
    {
        TotalInsects = 0;
    }

    // === ВЛАСТИВОСТІ ===
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

    // === ПОВЕРНЕННЯ ІНФОРМАЦІЇ ===
    public override string ToString()
    {
        return $"Назва: {Name}, Кількість ніг: {Legs}";
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
