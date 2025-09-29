public static class InsectExtensions
{
    // === МЕТОД РОЗШИРЕННЯ ===
    public static void PrintInfo(this Insect insect)
    {
        Console.WriteLine("Iнформацiя про комаху (метод розширення):");
        Console.WriteLine(insect.ToString());
    }
}
