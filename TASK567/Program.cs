using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ====================================================================================
//  РЕГИОН 1: БАЗОВЫЕ КЛАССЫ, НЕОБХОДИМЫЕ ДЛЯ ВСЕХ ЗАДАЧ
//  (Heap, MyPriorityQueue, Request, Comparers)
// ====================================================================================

#region Core Classes (Heap, PriorityQueue, Request, Comparers)

/// <summary>
/// ЗАДАЧА 5: Обобщенная реализация двоичной кучи (базовый класс).
/// </summary>
public class Heap<T>
{
    protected List<T> data;
    protected readonly IComparer<T> comparer;

    public Heap() : this(Comparer<T>.Default) { }
    public Heap(IComparer<T> comparer) { this.data = new List<T>(); this.comparer = comparer ?? Comparer<T>.Default; }
    public Heap(IEnumerable<T> collection, IComparer<T> comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
        this.data = new List<T>(collection);
        if (!IsEmpty) for (int i = (data.Count / 2) - 1; i >= 0; i--) HeapifyDown(i);
    }

    public int Size => data.Count;
    public bool IsEmpty => data.Count == 0;

    public int GetCapacity() => data.Capacity;

    public T FindTop() { if (IsEmpty) throw new InvalidOperationException("Куча пуста."); return data[0]; }
    public T ExtractTop()
    {
        if (IsEmpty) throw new InvalidOperationException("Куча пуста.");
        T top = data[0]; data[0] = data[data.Count - 1]; data.RemoveAt(data.Count - 1);
        if (!IsEmpty) HeapifyDown(0);
        return top;
    }
    public virtual void Insert(T value) { data.Add(value); HeapifyUp(data.Count - 1); }
    public void UpdateKey(int index, T newValue)
    {
        if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне диапазона.");
        if (comparer.Compare(newValue, data[index]) < 0) throw new ArgumentException("Новое значение имеет более низкий приоритет, чем текущее.");
        data[index] = newValue; HeapifyUp(index);
    }
    public void Merge(Heap<T> otherHeap)
    {
        if (otherHeap == null) throw new ArgumentNullException(nameof(otherHeap));
        foreach (var item in otherHeap.data) this.Insert(item);
    }
    public void Clear() => data.Clear();
    protected void HeapifyUp(int index)
    {
        if (index == 0) return; int parent = (index - 1) / 2;
        if (comparer.Compare(data[index], data[parent]) > 0) { Swap(parent, index); HeapifyUp(parent); }
    }
    protected void HeapifyDown(int index)
    {
        int left = 2 * index + 1, right = 2 * index + 2, largest = index;
        if (left < Size && comparer.Compare(data[left], data[largest]) > 0) largest = left;
        if (right < Size && comparer.Compare(data[right], data[largest]) > 0) largest = right;
        if (largest != index) { Swap(index, largest); HeapifyDown(largest); }
    }
    protected void Swap(int i, int j) { T temp = data[i]; data[i] = data[j]; data[j] = temp; }
    public override string ToString() => $"[{string.Join(", ", data)}]";
}

/// <summary>
/// ЗАДАЧА 6: Реализация очереди с приоритетами, наследующаяся от Heap.
/// </summary>
public class MyPriorityQueue<T> : Heap<T>
{
    public MyPriorityQueue() : this(11, null) { }
    public MyPriorityQueue(T[] a) : base(a, null) { }
    public MyPriorityQueue(int initialCapacity) : this(initialCapacity, null) { }
    public MyPriorityQueue(IComparer<T> comparator) : this(11, comparator) { }
    public MyPriorityQueue(int initialCapacity, IComparer<T> comparator) : base(comparator)
    {
        if (initialCapacity < 0) throw new ArgumentException("Начальная ёмкость не может быть отрицательной.");
        data.Capacity = initialCapacity;
    }
    public MyPriorityQueue(MyPriorityQueue<T> c) : base(c.data, c.comparer) { }

    public void Add(T e)
    {
        if (e == null) throw new ArgumentNullException(nameof(e));
        if (Size == data.Capacity)
        {
            int oldCapacity = data.Capacity, newCapacity = oldCapacity < 64 ? (oldCapacity == 0 ? 4 : oldCapacity * 2) : oldCapacity + oldCapacity / 2;
            data.Capacity = newCapacity;
        }
        base.Insert(e);
    }
    public void AddAll(T[] a) { if (a == null) throw new ArgumentNullException(nameof(a)); foreach (var item in a) Add(item); }
    public bool Contains(T item) => data.Contains(item);
    public bool ContainsAll(T[] a)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        var hashSet = new HashSet<T>(data); return a.All(item => hashSet.Contains(item));
    }
    public bool Remove(T item)
    {
        int index = data.IndexOf(item); if (index == -1) return false;
        data[index] = data[Size - 1]; data.RemoveAt(Size - 1);
        if (index < Size) { HeapifyUp(index); HeapifyDown(index); }
        return true;
    }
    public bool RemoveAll(T[] a)
    {
        if (a == null) throw new ArgumentNullException(nameof(a)); bool modified = false;
        foreach (var item in a) while (Remove(item)) modified = true; return modified;
    }
    public bool RetainAll(T[] a)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        var toRetain = new HashSet<T>(a); var removed = data.RemoveAll(item => !toRetain.Contains(item));
        if (removed > 0) for (int i = (data.Count / 2) - 1; i >= 0; i--) HeapifyDown(i);
        return removed > 0;
    }
    public int GetSize() => base.Size;
    public T[] ToArray() => data.ToArray();
    public T[] ToArray(T[] a)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (a.Length < Size) return data.ToArray();
        data.CopyTo(a, 0);
        if (a.Length > Size) a[Size] = default(T); return a;
    }
    public T Element() => base.FindTop();
    public bool Offer(T obj) { try { Add(obj); return true; } catch { return false; } }
    public T Peek() => IsEmpty ? default(T) : data[0];
    public T Poll() => IsEmpty ? default(T) : base.ExtractTop();
}

/// <summary>
/// Класс для Задачи 7.
/// </summary>
public class Request
{
    public int Priority { get; }
    public int RequestId { get; }
    public int StepAdded { get; }
    public Request(int priority, int requestId, int stepAdded) { Priority = priority; RequestId = requestId; StepAdded = stepAdded; }
    public override string ToString() => $"Заявка#{RequestId} (Приоритет: {Priority}, Добавлена на шаге: {StepAdded})";
}

/// <summary>
/// Компаратор для Задачи 7.
/// </summary>
public class RequestComparer : IComparer<Request>
{
    public int Compare(Request x, Request y)
    {
        if (x == null || y == null) return 0;
        int priorityComparison = x.Priority.CompareTo(y.Priority);
        if (priorityComparison != 0) return priorityComparison;
        return y.RequestId.CompareTo(x.RequestId);
    }
}

/// <summary>
/// Компаратор для демонстрации Задачи 6 (создает min-кучу).
/// </summary>
public class ReverseIntComparer : IComparer<int>
{
    public int Compare(int x, int y) => y.CompareTo(x);
}

#endregion

// ====================================================================================
//  РЕГИОН 2: ГЛАВНЫЙ КЛАСС ПРОГРАММЫ С МЕНЮ И ДЕМОНСТРАЦИЯМИ
// ====================================================================================

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n--- МЕНЮ ---");
            Console.WriteLine("1. Задача 5 (Heap)");
            Console.WriteLine("2. Задача 6 (MyPriorityQueue)");
            Console.WriteLine("3. Задача 7 (Симуляция)");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": DemonstrateTask5(); break;
                case "2": DemonstrateTask6(); break;
                case "3": RunTask7_Simulation(); break;
                case "0": Console.WriteLine("Программа завершена."); return;
                default: Console.WriteLine("Неверный ввод."); break;
            }
        }
    }

    #region Task 5 Demonstration (Manual Input, Simple UI)
    public static void DemonstrateTask5()
    {
        Console.WriteLine("-- Задача 5: Демонстрация Heap --\n");

        int[] initialData = ReadIntArrayFromConsole("Введите числа для создания кучи: ");
        if (initialData.Length == 0) { Console.WriteLine("Данные не введены."); return; }

        var heap = new Heap<int>(initialData);
        Console.WriteLine($"Создана куча: {heap}");

        Console.WriteLine($"Максимум (FindTop): {heap.FindTop()}");

        Console.WriteLine($"Удален максимум (ExtractTop): {heap.ExtractTop()}");
        Console.WriteLine($"Куча после удаления: {heap}");

        int toInsert = ReadIntFromConsole("Введите число для добавления: ");
        heap.Insert(toInsert);
        Console.WriteLine($"Куча после добавления: {heap}");

        if (!heap.IsEmpty)
        {
            int indexToUpdate = ReadIntFromConsole($"Введите индекс для обновления (0-{heap.Size - 1}): ", 0, heap.Size - 1);
            int newValue = ReadIntFromConsole("Введите новое, БОЛЬШЕЕ значение: ");
            try
            {
                heap.UpdateKey(indexToUpdate, newValue);
                Console.WriteLine($"Куча после обновления: {heap}");
            }
            catch (Exception ex) { Console.WriteLine($"ОШИБКА: {ex.Message}"); }
        }

        int[] otherData = ReadIntArrayFromConsole("Введите числа для ВТОРОЙ кучи (для слияния): ");
        var otherHeap = new Heap<int>(otherData);
        heap.Merge(otherHeap);
        Console.WriteLine($"Результат слияния: {heap}");
    }
    #endregion

    #region Task 6 Demonstration (Scripted Showcase with Numbering, Simple UI)
    public static void DemonstrateTask6()
    {
        Console.WriteLine("-- Задача 6: Обзор всех 21 методов MyPriorityQueue --\n");

        // 1. MyPriorityQueue(): Создает пустую очередь с начальной ёмкостью 11.
        var pq = new MyPriorityQueue<int>();
        Console.WriteLine($"1. MyPriorityQueue(): Создана пустая очередь. Емкость: {pq.GetCapacity()}");

        // 2. MyPriorityQueue(T[] a): Создает очередь из массива.
        var pqFromArray = new MyPriorityQueue<int>(new[] { 50, 20 });
        Console.WriteLine($"2. MyPriorityQueue(T[] a): Создана очередь из массива: {pqFromArray}");

        // 3. MyPriorityQueue(int initialCapacity): Создает очередь с указанной ёмкостью.
        var pqWithCapacity = new MyPriorityQueue<int>(5);
        Console.WriteLine($"3. MyPriorityQueue(int capacity): Создана очередь с емкостью {pqWithCapacity.GetCapacity()}");

        // 4. MyPriorityQueue(capacity, comparator): Создает очередь с ёмкостью и компаратором.
        var minHeap = new MyPriorityQueue<int>(10, new ReverseIntComparer());
        Console.WriteLine($"4. MyPriorityQueue(cap, comp): Создана min-куча.");

        // 5. MyPriorityQueue(MyPriorityQueue c): Создает очередь из другой очереди.
        var pqCopy = new MyPriorityQueue<int>(pqFromArray);
        Console.WriteLine($"5. MyPriorityQueue(MyPriorityQueue c): Создана копия: {pqCopy}\n");

        Console.WriteLine("--- Демонстрация методов на одном экземпляре ---");
        Console.WriteLine($"Начальная очередь: {pq}");

        // 6. add(T e): Добавляет элемент в конец очереди с приоритетами.
        pq.Add(10);
        pq.Add(20);
        Console.WriteLine($"6. add(T e): Добавлены 10 и 20. Результат: {pq}");

        // 7. addAll(T[] a): Добавляет элементы из массива.
        pq.AddAll(new[] { 5, 15 });
        Console.WriteLine($"7. addAll(T[] a): Добавлены 5 и 15. Результат: {pq}");

        // 8. clear(): Удаляет все элементы из очереди. (Будет продемонстрирован в конце).

        // 9. contains(object o): Проверяет, находится ли объект в очереди.
        Console.WriteLine($"9. contains(object o): Проверка на 10 -> {pq.Contains(10)}, на 99 -> {pq.Contains(99)}");

        // 10. containsAll(T[] a): Проверяет, содержатся ли все объекты в очереди.
        Console.WriteLine($"10. containsAll(T[] a): Проверка на [5, 10] -> {pq.ContainsAll(new[] { 5, 10 })}");

        // 11. isEmpty(): Проверяет, является ли очередь пустой.
        Console.WriteLine($"11. isEmpty(): Очередь пуста? -> {pq.IsEmpty}");

        // 12. remove(object o): Удаляет указанный объект из очереди.
        pq.Remove(10);
        Console.WriteLine($"12. remove(object o): Удален элемент 10. Результат: {pq}");

        // 13. removeAll(T[] a): Удаляет указанные объекты из очереди.
        pq.Add(5);
        Console.WriteLine($"    Добавлен дубликат '5'. Очередь: {pq}");
        pq.RemoveAll(new[] { 5, 99 });
        Console.WriteLine($"13. removeAll(T[] a): Удалены все '5' и '99'. Результат: {pq}");

        // 14. retainAll(T[] a): Оставляет в очереди только указанные объекты.
        pq.RetainAll(new[] { 20 });
        Console.WriteLine($"14. retainAll(T[] a): Оставлен только '20'. Результат: {pq}");

        // 15. size(): Получает размер очереди.
        Console.WriteLine($"15. size(): Текущий размер -> {pq.GetSize()}");

        // 16. toArray(): Возвращает массив, содержащий все элементы.
        pq.Add(30);
        int[] arr = pq.ToArray();
        Console.WriteLine($"16. toArray(): Преобразовано в массив: [{string.Join(", ", arr)}]");

        // 17. toArray(T[] a): Возвращает массив, содержащий все элементы (в существующий массив).
        int[] bigArr = new int[5];
        pq.ToArray(bigArr);
        Console.WriteLine($"17. toArray(T[] a): Скопировано в массив большего размера: [{string.Join(", ", bigArr)}]");

        // 18. element(): Возвращает элемент из головы без удаления (бросает исключение если пусто).
        Console.WriteLine($"18. element(): Элемент в голове -> {pq.Element()}");

        // 19. offer(T obj): Пытается добавить элемент.
        bool offered = pq.Offer(40);
        Console.WriteLine($"19. offer(T obj): Попытка добавить 40 -> {offered}. Результат: {pq}");

        // 20. peek(): Возвращает элемент из головы без удаления (null если пусто).
        Console.WriteLine($"20. peek(): Элемент в голове -> {pq.Peek()}");

        // 21. poll(): Удаляет и возвращает элемент из головы (null если пусто).
        Console.WriteLine($"21. poll(): Извлечен элемент -> {pq.Poll()}. Результат: {pq}");

        // 8. clear(): Демонстрация в конце.
        pq.Clear();
        Console.WriteLine($"8. clear(): Очередь очищена. Очередь пуста? -> {pq.IsEmpty}");
    }
    #endregion

    #region Task 7 Simulation (Manual Input, Simple UI)
    public static void RunTask7_Simulation()
    {
        Console.WriteLine("-- Задача 7: Симуляция --\n");
        int N = ReadIntFromConsole("Введите количество шагов N: ", 1);

        var queue = new MyPriorityQueue<Request>(new RequestComparer());
        int currentRequestId = 1, maxWaitingTime = -1;
        Request maxWaitingRequest = null;
        string logFileName = "log.txt";

        using (StreamWriter logFile = new StreamWriter(logFileName))
        {
            // Фаза 1
            for (int step = 1; step <= N; step++)
            {
                Console.WriteLine($"\nШаг {step}:");
                int requestsToAdd = ReadIntFromConsole($"Сколько заявок добавить? ", 0);

                for (int i = 0; i < requestsToAdd; i++)
                {
                    int priority = ReadIntFromConsole($"Приоритет для заявки #{currentRequestId} (1-5): ", 1, 5);
                    var newRequest = new Request(priority, currentRequestId, step);
                    queue.Add(newRequest);
                    logFile.WriteLine($"ADD {newRequest.RequestId} {newRequest.Priority} {step}");
                    Console.WriteLine($"Добавлена: {newRequest}");
                    currentRequestId++;
                }

                if (!queue.IsEmpty)
                {
                    var removedRequest = queue.Poll();
                    int waitingTime = step - removedRequest.StepAdded;
                    if (waitingTime > maxWaitingTime) { maxWaitingTime = waitingTime; maxWaitingRequest = removedRequest; }
                    logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {step}");
                    Console.WriteLine($"Удалена: {removedRequest} (ожидание: {waitingTime})");
                }
                else Console.WriteLine("Очередь пуста.");
                Console.WriteLine($"Заявок в очереди: {queue.GetSize()}");
            }

            // Фаза 2
            Console.WriteLine($"\nФаза очистки очереди:");
            int cleanupStep = N;
            while (!queue.IsEmpty)
            {
                cleanupStep++;
                var removedRequest = queue.Poll();
                int waitingTime = cleanupStep - removedRequest.StepAdded;
                if (waitingTime > maxWaitingTime) { maxWaitingTime = waitingTime; maxWaitingRequest = removedRequest; }
                logFile.WriteLine($"REMOVE {removedRequest.RequestId} {removedRequest.Priority} {cleanupStep}");
                Console.WriteLine($"Шаг {cleanupStep}: удалена {removedRequest} (ожидание: {waitingTime})");
            }
        }

        // Результаты
        Console.WriteLine("\nРезультаты симуляции:");
        if (maxWaitingRequest != null)
        {
            Console.WriteLine("Заявка с максимальным временем ожидания:");
            Console.WriteLine(maxWaitingRequest);
            Console.WriteLine($"Максимальное время ожидания: {maxWaitingTime} шагов");
        }
        else Console.WriteLine("Не было обработано ни одной заявки.");
        Console.WriteLine($"Лог сохранен в файл: {logFileName}");
    }
    #endregion

    #region Helper Methods for Input
    private static int[] ReadIntArrayFromConsole(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<int>();
        return input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.TryParse(s, out int n) ? n : (int?)null).Where(n => n.HasValue)
                    .Select(n => n.Value).ToArray();
    }

    private static int ReadIntFromConsole(string prompt, int? min = null, int? max = null)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out result) && (!min.HasValue || result >= min.Value) && (!max.HasValue || result <= max.Value)) return result;
            Console.WriteLine("Неверный ввод. Повторите.");
        }
    }
    #endregion
}