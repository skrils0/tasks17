using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Класс для представления заявки
public class Request
{
    public int Priority { get; set; }     // Приоритет (1-5)
    public int RequestId { get; set; }    // Номер заявки (сквозная нумерация)
    public int StepAdded { get; set; }    // Шаг, на котором заявка добавлена

    public Request(int priority, int requestId, int stepAdded)
    {
        Priority = priority;
        RequestId = requestId;
        StepAdded = stepAdded;
    }

    // Для удобства вывода
    public override string ToString()
    {
        return $"Заявка {RequestId} (Приоритет: {Priority}, Добавлена на шаге: {StepAdded})";
    }
}

// Класс очереди с приоритетами (упрощенная реализация на основе списка)
public class MyPriorityQueue
{
    private List<Request> requests;

    public MyPriorityQueue()
    {
        requests = new List<Request>();
    }

    // Добавление заявки в очередь
    public void Add(Request request)
    {
        requests.Add(request);
        // Сортируем по убыванию приоритета для быстрого доступа к максимальному
        requests = requests.OrderByDescending(r => r.Priority).ThenBy(r => r.RequestId).ToList();
    }

    // Удаление заявки с наивысшим приоритетом
    public Request RemoveMax()
    {
        if (requests.Count == 0)
            return null;

        var maxRequest = requests[0];
        requests.RemoveAt(0);
        return maxRequest;
    }

    // Проверка пустоты очереди
    public bool IsEmpty()
    {
        return requests.Count == 0;
    }

    // Получение количества заявок
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
        Console.WriteLine("=== Симулятор приоритетной очереди заявок ===");

        // Ввод количества шагов
        Console.Write("Введите количество шагов добавления заявок (N): ");
        int N = int.Parse(Console.ReadLine());

        // Инициализация очереди
        MyPriorityQueue queue = new MyPriorityQueue();
        int currentRequestId = 1;  // Счетчик заявок
        Request maxWaitingRequest = null;  // Заявка с максимальным временем ожидания
        int maxWaitingTime = 0;    // Максимальное время ожидания

        // Открываем файл для логирования
        using (StreamWriter logFile = new StreamWriter("log.txt"))
        {
            // Фаза добавления заявок (N шагов)
            for (int step = 1; step <= N; step++)
            {
                Console.WriteLine($"\n--- Шаг {step} ---");

                // Генерация от 1 до 10 заявок
                int requestsToAdd = random.Next(1, 11);
                Console.WriteLine($"Добавляется заявок: {requestsToAdd}");

                // Добавление заявок
                for (int i = 0; i < requestsToAdd; i++)
                {
                    int priority = random.Next(1, 6);  // Приоритет от 1 до 5
                    Request newRequest = new Request(priority, currentRequestId, step);
                    queue.Add(newRequest);

                    // Логирование добавления
                    logFile.WriteLine($"ADD {currentRequestId} {priority} {step}");
                    Console.WriteLine($"  Добавлена: {newRequest}");

                    currentRequestId++;
                }

                // Удаление заявки с наивысшим приоритетом
                if (!queue.IsEmpty())
                {
                    Request removedRequest = queue.RemoveMax();
                    int waitingTime = step - removedRequest.StepAdded;  // Время ожидания

                    // Проверка на максимальное время ожидания
                    if (waitingTime > maxWaitingTime)
                    {
                        maxWaitingTime = waitingTime;
                        maxWaitingRequest = removedRequest;
                    }

                    // Логирование удаления
                    logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {step}");
                    Console.WriteLine($"  Удалена: {removedRequest} (Время ожидания: {waitingTime} шагов)");
                }
                else
                {
                    Console.WriteLine("  Очередь пуста - нечего удалять");
                }

                Console.WriteLine($"Заявок в очереди: {queue.Count()}");
            }

            // Фаза очистки очереди (после N шагов)
            Console.WriteLine($"\n--- Фаза очистки очереди ---");
            int cleanupStep = N + 1;

            while (!queue.IsEmpty())
            {
                Request removedRequest = queue.RemoveMax();
                int waitingTime = cleanupStep - removedRequest.StepAdded;

                // Проверка на максимальное время ожидания
                if (waitingTime > maxWaitingTime)
                {
                    maxWaitingTime = waitingTime;
                    maxWaitingRequest = removedRequest;
                }

                // Логирование удаления
                logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {cleanupStep}");
                Console.WriteLine($"Шаг {cleanupStep}: Удалена {removedRequest} (Время ожидания: {waitingTime} шагов)");

                cleanupStep++;
            }
        }

        // Вывод результата
        Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ===");
        if (maxWaitingRequest != null)
        {
            Console.WriteLine($"Заявка с максимальным временем ожидания:");
            Console.WriteLine($"{maxWaitingRequest}");
            Console.WriteLine($"Максимальное время ожидания: {maxWaitingTime} шагов");
        }
        else
        {
            Console.WriteLine("Не было обработано ни одной заявки");
        }

        Console.WriteLine($"Лог сохранен в файл: log.txt");
    }
}