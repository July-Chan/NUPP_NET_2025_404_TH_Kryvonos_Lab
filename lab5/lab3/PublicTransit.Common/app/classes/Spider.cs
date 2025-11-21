namespace PublicTransit.Common.App.Classes;

public class Spider : Insect
{
    public bool IsPoisonous { get; set; }

    // === КОНСТРУКТОР ===
    public Spider(string name, int legs, bool isPoisonous) : base(name, legs)
    {
        IsPoisonous = isPoisonous;
    }

    public static Spider Create()
    {
        var random = new Random();
        return new Spider($"Spider-{random.Next(1, 100)}", 8, random.Next(0, 2) == 1);
    }

    // === РЕАЛІЗАЦІЯ АБСТРАКТНОГО МЕТОДУ ===
    public override void Move()
    {
        Console.WriteLine("Павук повзе.");
    }

    // === ПОВЕРНЕННЯ ІНФОРМАЦІЇ ===
    public override string ToString()
    {
        return base.ToString() + $", Отруйний: {IsPoisonous}";
    }
}
