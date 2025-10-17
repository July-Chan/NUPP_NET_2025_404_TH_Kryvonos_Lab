namespace PublicTransit.Common.App.Classes;

public static class InsectExtensions
{
    // === МЕТОД РОЗШИРЕННЯ ===
    public static void PrintInfo(this Insect insect)
    {
        Console.WriteLine("Інформація про комаху (метод розширення):");
        Console.WriteLine(insect.ToString());
    }
}
