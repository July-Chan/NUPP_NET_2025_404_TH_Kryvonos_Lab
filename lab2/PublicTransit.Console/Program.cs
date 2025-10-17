using PublicTransit.Common.App.Classes;
using PublicTransit.Common.App.Crud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        const string filePath = "insects.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var crudService = new CrudServiceAsync<Insect>(filePath);
        var watch = Stopwatch.StartNew();

        Console.WriteLine("Creating 1000 insects in parallel...");

        var insectsToCreate = new List<Insect>();
        Parallel.For(0, 1000, i =>
        {
            Insect newInsect = i % 2 == 0 ? Fly.Create() : Spider.Create();
            lock (insectsToCreate)
            {
                insectsToCreate.Add(newInsect);
            }
        });

        var createTasks = insectsToCreate.Select(insect => crudService.CreateAsync(insect)).ToList();
        await Task.WhenAll(createTasks);

        watch.Stop();
        Console.WriteLine($"Creation finished in {watch.ElapsedMilliseconds} ms.");

        var allInsects = await crudService.ReadAllAsync();
        Console.WriteLine($"Total insects in service: {allInsects.Count()}");


        var flies = allInsects.OfType<Fly>().ToList();
        if (flies.Any())
        {
            var minWingSpan = flies.Min(f => f.WingSpan);
            var maxWingSpan = flies.Max(f => f.WingSpan);
            var avgWingSpan = flies.Average(f => f.WingSpan);

            Console.WriteLine("\n--- Fly Statistics ---");
            Console.WriteLine($"Min Wing Span: {minWingSpan:F2}");
            Console.WriteLine($"Max Wing Span: {maxWingSpan:F2}");
            Console.WriteLine($"Average Wing Span: {avgWingSpan:F2}");
        }
        else
        {
            Console.WriteLine("\nNo flies were created to generate statistics.");
        }

        if (allInsects.Any())
        {
            var minLegs = allInsects.Min(i => i.Legs);
            var maxLegs = allInsects.Max(i => i.Legs);
            var avgLegs = allInsects.Average(i => i.Legs);

            Console.WriteLine("\n--- General Insect Statistics ---");
            Console.WriteLine($"Min Legs: {minLegs}");
            Console.WriteLine($"Max Legs: {maxLegs}");
            Console.WriteLine($"Average Legs: {avgLegs:F2}");
        }
        else
        {
            Console.WriteLine("\nNo insects were created to generate statistics.");
        }


        Console.WriteLine("\nSaving collection to file...");
        bool saved = await crudService.SaveAsync();
        Console.WriteLine(saved ? "Successfully saved." : "Failed to save.");

        SynchronizationDemos.Run();
    }
}

