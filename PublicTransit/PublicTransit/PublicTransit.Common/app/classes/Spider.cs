public class Spider : Insect
{
    public bool IsPoisonous { get; set; }

    // === КОНСТРУКТОР ===
    public Spider(string name, int legs, bool isPoisonous) : base(name, legs)
    {
        IsPoisonous = isPoisonous;
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
