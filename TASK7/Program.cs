using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// класс для представления заявки
public class Request
{
    public int Priority { get; set; }     // приоритет (1-5)
    public int RequestId { get; set; }    // номер заявки (сквозная нумерация)
    public int StepAdded { get; set; }    // шаг, на котором заявка добавлена

    public Request(int priority, int requestId, int stepAdded)
    {
        Priority = priority;
        RequestId = requestId;
        StepAdded = stepAdded;
    }

    // для удобства вывода
    public override string ToString()
    {
        return $"Заявка {RequestId} (Приоритет: {Priority}, Добавлена на шаге: {StepAdded})";
    }
}

// класс очереди с приоритетами (упрощенная реализация на основе списка)
public class MyPriorityQueue
{
    private List<Request> requests;

    public MyPriorityQueue()
    {
        requests = new List<Request>();
    }

    // добавление заявки в очередь
    public void Add(Request request)
    {
        requests.Add(request);
        // сортируем по убыванию приоритета для быстрого доступа к максимальному
        requests = requests.OrderByDescending(r => r.Priority).ThenBy(r => r.RequestId).ToList();
    }

    // удаление заявки с наивысшим приоритетом
    public Request RemoveMax()
    {
        if (requests.Count == 0)
            return null;

        var maxRequest = requests[0];
        requests.RemoveAt(0);
        return maxRequest;
    }

    // проверка пустоты очереди
    public bool IsEmpty()
    {
        return requests.Count == 0;
    }

    // получение количества заявок
    public int Count()
    {
        return requests.Count;
    }
}

public class Program
{
    private static Random random = new Random();

    public static void Main(string[] args)
    {
        Console.WriteLine("=== симулятор приоритетной очереди заявок ===");

        // ввод количества шагов
        Console.Write("введите количество шагов добавления заявок (N): ");
        int N = int.Parse(Console.ReadLine());

        // инициализация очереди
        MyPriorityQueue queue = new MyPriorityQueue();
        int currentRequestId = 1;  // счетчик заявок
        Request maxWaitingRequest = null;  // заявка с максимальным временем ожидания
        int maxWaitingTime = 0;    // максимальное время ожидания

        // открываем файл для логирования
        using (StreamWriter logFile = new StreamWriter("log.txt"))
        {
            // фаза добавления заявок (N шагов)
            for (int step = 1; step <= N; step++)
            {
                Console.WriteLine($"\n--- шаг {step} ---");

                // генерация от 1 до 10 заявок
                int requestsToAdd = random.Next(1, 11);
                Console.WriteLine($"добавляется заявок: {requestsToAdd}");

                // добавление заявок
                for (int i = 0; i < requestsToAdd; i++)
                {
                    int priority = random.Next(1, 6);  // приоритет от 1 до 5
                    Request newRequest = new Request(priority, currentRequestId, step);
                    queue.Add(newRequest);

                    // логирование добавления
                    logFile.WriteLine($"ADD {currentRequestId} {priority} {step}");
                    Console.WriteLine($"  добавлена: {newRequest}");

                    currentRequestId++;
                }

                // удаление заявки с наивысшим приоритетом
                if (!queue.IsEmpty())
                {
                    Request removedRequest = queue.RemoveMax();
                    int waitingTime = step - removedRequest.StepAdded;  // время ожидания

                    // проверка на максимальное время ожидания
                    if (waitingTime > maxWaitingTime)
                    {
                        maxWaitingTime = waitingTime;
                        maxWaitingRequest = removedRequest;
                    }

                    // логирование удаления
                    logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {step}");
                    Console.WriteLine($"  удалена: {removedRequest} (время ожидания: {waitingTime} шагов)");
                }
                else
                {
                    Console.WriteLine("  очередь пуста - нечего удалять");
                }

                Console.WriteLine($"заявок в очереди: {queue.Count()}");
            }

            // фаза очистки очереди (после N шагов)
            Console.WriteLine($"\n--- фаза очистки очереди ---");
            int cleanupStep = N + 1;

            while (!queue.IsEmpty())
            {
                Request removedRequest = queue.RemoveMax();
                int waitingTime = cleanupStep - removedRequest.StepAdded;

                // проверка на максимальное время ожидания
                if (waitingTime > maxWaitingTime)
                {
                    maxWaitingTime = waitingTime;
                    maxWaitingRequest = removedRequest;
                }

                // логирование удаления
                logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {cleanupStep}");
                Console.WriteLine($"шаг {cleanupStep}: удалена {removedRequest} (время ожидания: {waitingTime} шагов)");

                cleanupStep++;
            }
        }

        // вывод результата
        Console.WriteLine("\n=== результаты ===");
        if (maxWaitingRequest != null)
        {
            Console.WriteLine($"заявка с максимальным временем ожидания:");
            Console.WriteLine($"{maxWaitingRequest}");
            Console.WriteLine($"максимальное время ожидания: {maxWaitingTime} шагов");
        }
        else
        {
            Console.WriteLine("не было обработано ни одной заявки");
        }

        Console.WriteLine($"лог сохранен в файл: log.txt");
    }
}
