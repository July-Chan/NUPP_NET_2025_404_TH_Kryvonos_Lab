using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class SynchronizationDemos
{
    public static void Run()
    {
        Console.WriteLine("\n\n--- Running Synchronization Primitives Demos ---");
        LockDemo();
        SemaphoreSlimDemo();
        AutoResetEventDemo();
        MutexDemo();
        MonitorDemo();
        Console.WriteLine("\n--- Synchronization Demos Finished ---\n");
    }

    private static void LockDemo()
    {
        Console.WriteLine("\n--- Lock Demo ---");
        var sharedList = new List<int>();
        var listLock = new object();
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 3; j++)
                {
                    lock (listLock)
                    {
                        sharedList.Add(threadId * 10 + j);
                        Console.WriteLine($"[Lock] Thread {threadId} added a value. List count is {sharedList.Count}");
                    }
                    Thread.Sleep(20);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"[Lock] Final list count: {sharedList.Count}");
    }

    private static void SemaphoreSlimDemo()
    {
        Console.WriteLine("\n--- SemaphoreSlim Demo ---");
        var semaphore = new SemaphoreSlim(2, 2);
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(async () =>
            {
                Console.WriteLine($"[Semaphore] Thread {threadId} is waiting...");
                await semaphore.WaitAsync();
                try
                {
                    Console.WriteLine($"[Semaphore] >> Thread {threadId} has entered.");
                    await Task.Delay(500);
                    Console.WriteLine($"[Semaphore] << Thread {threadId} is leaving.");
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }

    private static void AutoResetEventDemo()
    {
        Console.WriteLine("\n--- AutoResetEvent Demo ---");
        var autoResetEvent = new AutoResetEvent(false);

        var task = Task.Run(() =>
        {
            Console.WriteLine("[AutoResetEvent] Task is waiting for a signal...");
            autoResetEvent.WaitOne();
            Console.WriteLine("[AutoResetEvent] Task received signal and finished.");
        });

        Console.WriteLine("[AutoResetEvent] Main thread is doing some work...");
        Thread.Sleep(1000);
        Console.WriteLine("[AutoResetEvent] Main thread is sending a signal.");
        autoResetEvent.Set();

        task.Wait();
    }

    private static void MutexDemo()
    {
        Console.WriteLine("\n--- Mutex Demo ---");
        var mutex = new Mutex(false, "MyUniqueMutexName");
        var tasks = new List<Task>();

        for (int i = 0; i < 3; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                Console.WriteLine($"[Mutex] Thread {threadId} is requesting the mutex.");
                mutex.WaitOne();
                try
                {
                    Console.WriteLine($"[Mutex] >> Thread {threadId} has acquired the mutex.");
                    Thread.Sleep(500);
                    Console.WriteLine($"[Mutex] << Thread {threadId} is releasing the mutex.");
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }

    private static void MonitorDemo()
    {
        Console.WriteLine("\n--- Monitor (Producer/Consumer) Demo ---");
        var buffer = new Queue<int>();
        var bufferLock = new object();
        bool isProductionFinished = false;

        var producer = Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                lock (bufferLock)
                {
                    buffer.Enqueue(i);
                    Console.WriteLine($"[Monitor] Produced {i}");
                    Monitor.Pulse(bufferLock); // Signal consumer
                }
                Thread.Sleep(100);
            }
            lock(bufferLock)
            {
                isProductionFinished = true;
                Monitor.PulseAll(bufferLock); // Signal all consumers that production is done
            }
        });

        var consumer = Task.Run(() =>
        {
            while (true)
            {
                lock (bufferLock)
                {
                    while (buffer.Count == 0 && !isProductionFinished)
                    {
                        Console.WriteLine("[Monitor] Consumer is waiting...");
                        Monitor.Wait(bufferLock); // Wait for producer
                    }

                    if (buffer.Count > 0)
                    {
                        int item = buffer.Dequeue();
                        Console.WriteLine($"[Monitor] Consumed {item}");
                    }
                    else if (isProductionFinished)
                    {
                        Console.WriteLine("[Monitor] Consumer finished.");
                        break;
                    }
                }
            }
        });

        Task.WaitAll(producer, consumer);
    }
}
