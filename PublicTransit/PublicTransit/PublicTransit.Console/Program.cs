using System;

namespace Program
{
    public static class EntryPoint
    {
        public static void Main(string[] args)
        {
            // Пiдписка на подiю
            Insect.OnInsectBorn += (sender, e) =>
            {
                Console.WriteLine($"Народилася нова комаха: {e.InsectName}");
            };

            Console.WriteLine("Створення об'єктiв:");
            var fly = new Fly("Муха звичайна", 6, 1.5);
            var spider = new Spider("Павук-хрестовик", 8, false);

            Console.WriteLine("\niнформацiя про об'єкти:");
            Console.WriteLine(fly);
            Console.WriteLine(spider);

            Console.WriteLine("\nДемонстрацiя руху:");
            fly.Move();
            spider.Move();

            Console.WriteLine($"\nЗагальна кiлькiсть комах: {Insect.GetTotalInsectsCount()}");

            Console.WriteLine("\nВиклик методу розширення:");
            fly.PrintInfo();
            spider.PrintInfo();

            Console.WriteLine("\n\n--- CRUD ТЕСТИ ---");
            var insectCrud = new CrudMap<Insect>();
            
            // 1. Створення
            var flyId = insectCrud.Create(new Fly("Домашня муха", 6, 1.2));
            var spiderId = insectCrud.Create(new Spider("Чорна вдова", 8, true));
            Console.WriteLine("\n1. Створено 2 комахи.");

            // 2. Читання всiх
            Console.WriteLine("\n2. Список всiх комах:");
            foreach (var insect in insectCrud.ReadAll())
            {
                Console.WriteLine($"- {insect}");
            }

            // 3. Читання одного
            Console.WriteLine("\n3. Читання однiєї комахи (муха):");
            var readFly = insectCrud.Read(flyId);
            Console.WriteLine($"- {readFly}");

            // 4. Оновлення
            Console.WriteLine("\n4. Оновлення павука:");
            var updatedSpider = new Spider("Оновлений павук", 8, false);
            insectCrud.Update(spiderId, updatedSpider);
            var readSpider = insectCrud.Read(spiderId);
            Console.WriteLine($"- {readSpider}");

            // 5. Видалення
            Console.WriteLine("\n5. Видалення мухи.");
            insectCrud.Remove(flyId);
            Console.WriteLine("   Список комах пiсля видалення:");
            foreach (var insect in insectCrud.ReadAll())
            {
                Console.WriteLine($"- {insect}");
            }

            // 6. Збереження та завантаження
            Console.WriteLine("\n6. Збереження та завантаження.");
            string filePath = "insects.json";
            insectCrud.Save(filePath);
            Console.WriteLine($"   Збережено у файл {filePath}");

            var loadedInsectCrud = CrudMap<Insect>.Load(filePath);
            Console.WriteLine("   Завантажено з файлу. Список комах:");
            foreach (var insect in loadedInsectCrud.ReadAll())
            {
                Console.WriteLine($"- {insect}");
            }
        }
    }
}
