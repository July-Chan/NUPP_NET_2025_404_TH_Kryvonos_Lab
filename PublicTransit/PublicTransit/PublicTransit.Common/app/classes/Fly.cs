public class Fly : Insect
{
    public double WingSpan { get; set; }

    // === КОНСТРУКТОР ===
    public Fly(string name, int legs, double wingSpan) : base(name, legs)
    {
        WingSpan = wingSpan;
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
