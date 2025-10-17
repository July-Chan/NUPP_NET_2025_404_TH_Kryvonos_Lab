namespace PublicTransit.Common.App.Classes;

public class Fly : Insect
{
    public double WingSpan { get; set; }

    // === КОНСТРУКТОР ===
    public Fly(string name, int legs, double wingSpan) : base(name, legs)
    {
        WingSpan = wingSpan;
    }

    public static Fly Create()
    {
        var random = new Random();
        return new Fly($"Fly-{random.Next(1, 100)}", 6, random.NextDouble() * 5);
    }

    // === РЕАЛІЗАЦІЯ АБСТРАКТНОГО МЕТОДУ ===
    public override void Move()
    {
        Console.WriteLine("Муха летить.");
    }

    // === ПОВЕРНЕННЯ ІНФОРМАЦІЇ ===
    public override string ToString()
    {
        return base.ToString() + $", Розмах крил: {WingSpan} см";
    }
}
