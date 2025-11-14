using System;
using System.Collections.Generic;

// класс очереди с приоритетами на основе кучи
public class MyPriorityQueue<T>
{
    private T[] queue;           // массив для хранения элементов
    private int size;           // текущее количество элементов
    private IComparer<T> comparer; // компаратор для сравнения элементов

    // конструктор 1: пустая очередь с начальной ёмкостью 11
    public MyPriorityQueue()
    {
        queue = new T[11];
        size = 0;
        comparer = Comparer<T>.Default;
        Console.WriteLine("Создана пустая очередь с начальной ёмкостью 11");
    }

    // конструктор 2: очередь из массива
    public MyPriorityQueue(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        queue = new T[a.Length + 1];
        size = a.Length;
        comparer = Comparer<T>.Default;

        Array.Copy(a, 0, queue, 1, a.Length);
        Console.WriteLine($"Создана очередь из массива с {size} элементами");

        BuildHeap();
        Console.WriteLine("Построена куча из массива");
        PrintHeap();
    }

    // конструктор 3: пустая очередь с указанной начальной ёмкостью
    public MyPriorityQueue(int initialCapacity)
    {
        if (initialCapacity < 1)
            throw new ArgumentException("начальная ёмкость должна быть положительной");

        queue = new T[initialCapacity + 1];
        size = 0;
        comparer = Comparer<T>.Default;
        Console.WriteLine($"Создана пустая очередь с ёмкостью {initialCapacity}");
    }

    // конструктор 4: пустая очередь с указанной ёмкостью и компаратором
    public MyPriorityQueue(int initialCapacity, IComparer<T> comparator)
    {
        if (initialCapacity < 1)
            throw new ArgumentException("начальная ёмкость должна быть положительной");

        queue = new T[initialCapacity + 1];
        size = 0;
        comparer = comparator ?? Comparer<T>.Default;
        Console.WriteLine($"Создана пустая очередь с ёмкостью {initialCapacity} и пользовательским компаратором");
    }

    // конструктор 5: очередь из другой очереди
    public MyPriorityQueue(MyPriorityQueue<T> c)
    {
        if (c == null)
            throw new ArgumentNullException(nameof(c));

        queue = new T[c.queue.Length];
        size = c.size;
        comparer = c.comparer;
        Array.Copy(c.queue, 0, queue, 0, c.queue.Length);
        Console.WriteLine("Создана очередь копированием из другой очереди");
        PrintHeap();
    }

    // добавление элемента в очередь
    public void Add(T e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        Console.WriteLine($"\nДобавление элемента: {e}");

        if (size >= queue.Length - 1)
        {
            Console.WriteLine($"Ёмкость увеличена с {queue.Length - 1} до {(queue.Length - 1 < 64 ? (queue.Length - 1) * 2 : (int)((queue.Length - 1) * 1.5))}");
            Resize();
        }

        size++;
        queue[size] = e;
        Console.WriteLine($"Элемент добавлен в позицию {size}");

        Swim(size);
        Console.WriteLine("Выполнено просеивание вверх");
        PrintHeap();
    }

    // добавление всех элементов из массива
    public void AddAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        Console.WriteLine($"\nДобавление {a.Length} элементов из массива");
        foreach (T item in a)
        {
            Add(item);
        }
    }

    // очистка очереди
    public void Clear()
    {
        Console.WriteLine("\nОчистка очереди");
        for (int i = 1; i <= size; i++)
        {
            queue[i] = default(T);
        }
        size = 0;
        PrintHeap();
    }

    // проверка наличия элемента в очереди
    public bool Contains(object obj)
    {
        if (obj == null) return false;

        Console.WriteLine($"\nПоиск элемента: {obj}");
        for (int i = 1; i <= size; i++)
        {
            if (obj.Equals(queue[i]))
            {
                Console.WriteLine($"Элемент найден в позиции {i}");
                return true;
            }
        }
        Console.WriteLine("Элемент не найден");
        return false;
    }

    // проверка наличия всех элементов из массива
    public bool ContainsAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        Console.WriteLine($"\nПроверка наличия всех {a.Length} элементов");
        foreach (T item in a)
        {
            if (!Contains(item))
            {
                Console.WriteLine("Не все элементы найдены");
                return false;
            }
        }
        Console.WriteLine("Все элементы найдены");
        return true;
    }

    // проверка пустоты очереди
    public bool IsEmpty()
    {
        bool empty = size == 0;
        Console.WriteLine($"Проверка пустоты: {(empty ? "очередь пуста" : "очередь не пуста")}");
        return empty;
    }

    // удаление указанного элемента
    public bool Remove(object obj)
    {
        if (obj == null) return false;

        Console.WriteLine($"\nУдаление элемента: {obj}");

        for (int i = 1; i <= size; i++)
        {
            if (obj.Equals(queue[i]))
            {
                Console.WriteLine($"Элемент найден в позиции {i}");

                queue[i] = queue[size];
                queue[size] = default(T);
                size--;

                Console.WriteLine($"Элемент из позиции {size + 1} перемещён в позицию {i}");

                if (i <= size)
                {
                    Swim(i);
                    Sink(i);
                    Console.WriteLine("Выполнено восстановление кучи");
                }

                PrintHeap();
                return true;
            }
        }

        Console.WriteLine("Элемент не найден для удаления");
        return false;
    }

    // удаление всех указанных элементов
    public bool RemoveAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        Console.WriteLine($"\nУдаление {a.Length} указанных элементов");
        bool modified = false;
        foreach (T item in a)
        {
            if (Remove(item))
                modified = true;
        }
        Console.WriteLine($"Результат удаления: {(modified ? "были удалены элементы" : "элементы не найдены")}");
        return modified;
    }

    // оставление только указанных элементов
    public bool RetainAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        Console.WriteLine($"\nОставление только {a.Length} указанных элементов");
        bool modified = false;
        List<T> toRetain = new List<T>(a);

        for (int i = size; i >= 1; i--)
        {
            if (!toRetain.Contains(queue[i]))
            {
                Console.WriteLine($"Удаление элемента {queue[i]} из позиции {i}");
                Remove(queue[i]);
                modified = true;
            }
        }
        Console.WriteLine($"Результат операции: {(modified ? "очередь изменена" : "очередь не изменена")}");
        return modified;
    }

    // получение размера очереди
    public int Size()
    {
        Console.WriteLine($"Текущий размер очереди: {size}");
        return size;
    }

    // преобразование в массив
    public T[] ToArray()
    {
        Console.WriteLine("\nПреобразование очереди в массив");
        T[] result = new T[size];
        Array.Copy(queue, 1, result, 0, size);
        Console.WriteLine($"Создан массив из {result.Length} элементов");
        return result;
    }

    // преобразование в массив с указанным типом
    public T[] ToArray(T[] a)
    {
        Console.WriteLine("\nПреобразование очереди в существующий массив");
        if (a == null)
            return ToArray();

        if (a.Length < size)
            return ToArray();

        Array.Copy(queue, 1, a, 0, size);
        if (a.Length > size)
            a[size] = default(T);

        Console.WriteLine($"Массив заполнен {size} элементами");
        return a;
    }

    // получение элемента из головы очереди без удаления
    public T Element()
    {
        if (IsEmpty())
            throw new InvalidOperationException("очередь пуста");

        T element = queue[1];
        Console.WriteLine($"Получен элемент из головы очереди: {element}");
        return element;
    }

    // попытка добавления элемента
    public bool Offer(T obj)
    {
        Console.WriteLine($"\nПопытка добавления элемента: {obj}");
        try
        {
            Add(obj);
            Console.WriteLine("Элемент успешно добавлен");
            return true;
        }
        catch
        {
            Console.WriteLine("Не удалось добавить элемент");
            return false;
        }
    }

    // получение элемента из головы без удаления
    public T Peek()
    {
        if (IsEmpty())
        {
            Console.WriteLine("Попытка чтения из пустой очереди - возвращено значение по умолчанию");
            return default(T);
        }

        T element = queue[1];
        Console.WriteLine($"Просмотр головного элемента: {element}");
        return element;
    }

    // удаление и возврат элемента из головы очереди
    public T Poll()
    {
        if (IsEmpty())
        {
            Console.WriteLine("Попытка извлечения из пустой очереди - возвращено значение по умолчанию");
            return default(T);
        }

        T result = queue[1];
        Console.WriteLine($"\nИзвлечение элемента: {result}");

        queue[1] = queue[size];
        queue[size] = default(T);
        size--;

        Console.WriteLine($"Элемент из позиции {size + 1} перемещён в корень");

        if (size > 0)
        {
            Sink(1);
            Console.WriteLine("Выполнено просеивание вниз");
        }

        PrintHeap();
        return result;
    }

    // вспомогательные методы для работы с кучей

    // увеличение ёмкости при необходимости
    private void Resize()
    {
        int newCapacity;
        if (queue.Length - 1 < 64)
            newCapacity = (queue.Length - 1) * 2;
        else
            newCapacity = (int)((queue.Length - 1) * 1.5);

        T[] newQueue = new T[newCapacity + 1];
        Array.Copy(queue, 1, newQueue, 1, size);
        queue = newQueue;
    }

    // просеивание вверх
    private void Swim(int k)
    {
        Console.WriteLine($"Просеивание вверх элемента из позиции {k}");
        while (k > 1 && Compare(k / 2, k) < 0)
        {
            Console.WriteLine($"Обмен элементов в позициях {k / 2} и {k}");
            Swap(k, k / 2);
            k = k / 2;
        }
    }

    // просеивание вниз
    private void Sink(int k)
    {
        Console.WriteLine($"Просеивание вниз элемента из позиции {k}");
        while (2 * k <= size)
        {
            int j = 2 * k;
            if (j < size && Compare(j, j + 1) < 0)
                j++;

            if (Compare(k, j) >= 0)
                break;

            Console.WriteLine($"Обмен элементов в позициях {k} и {j}");
            Swap(k, j);
            k = j;
        }
    }

    // построение кучи из всех элементов
    private void BuildHeap()
    {
        Console.WriteLine("Начало построения кучи");
        for (int i = size / 2; i >= 1; i--)
        {
            Console.WriteLine($"Обработка узла {i}");
            Sink(i);
        }
        Console.WriteLine("Построение кучи завершено");
    }

    // сравнение двух элементов
    private int Compare(int i, int j)
    {
        return comparer.Compare(queue[i], queue[j]);
    }

    // обмен двух элементов
    private void Swap(int i, int j)
    {
        T temp = queue[i];
        queue[i] = queue[j];
        queue[j] = temp;
    }

    // вывод текущего состояния кучи в консоль
    public void PrintHeap()
    {
        Console.Write("Текущее состояние кучи: [");
        for (int i = 1; i <= size; i++)
        {
            Console.Write(queue[i]);
            if (i < size) Console.Write(", ");
        }
        Console.WriteLine("]");
    }
}

// пример использования
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== ДЕМОНСТРАЦИЯ РАБОТЫ ПРИОРИТЕТНОЙ ОЧЕРЕДИ ===\n");

        // создаем очередь с приоритетами для целых чисел
        Console.WriteLine("1. СОЗДАНИЕ ОЧЕРЕДИ");
        MyPriorityQueue<int> queue = new MyPriorityQueue<int>();

        // добавляем элементы
        Console.WriteLine("\n2. ДОБАВЛЕНИЕ ЭЛЕМЕНТОВ");
        queue.Add(5);
        queue.Add(10);
        queue.Add(3);
        queue.Add(8);

        Console.WriteLine("\n3. ПРОВЕРКА ОПЕРАЦИЙ");
        queue.Size();
        queue.IsEmpty();
        queue.Contains(8);
        queue.Contains(15);

        Console.WriteLine("\n4. ПРОСМОТР ЭЛЕМЕНТОВ");
        queue.Peek();
        queue.Element();

        Console.WriteLine("\n5. ИЗВЛЕЧЕНИЕ ЭЛЕМЕНТОВ");
        Console.WriteLine("Извлеченные элементы:");
        while (!queue.IsEmpty())
        {
            int element = queue.Poll();
            Console.WriteLine($"Извлечён: {element}");
        }

        Console.WriteLine("\n6. РАБОТА С МАССИВАМИ");
        int[] testArray = new int[] { 20, 5, 15, 25, 10 };
        MyPriorityQueue<int> queueFromArray = new MyPriorityQueue<int>(testArray);

        Console.WriteLine("\n7. ТЕСТИРОВАНИЕ РАЗЛИЧНЫХ ОПЕРАЦИЙ");
        queueFromArray.Offer(30);
        queueFromArray.Remove(15);
        
        int[] retainArray = new int[] { 10, 20, 30 };
        queueFromArray.RetainAll(retainArray);

        Console.WriteLine("\n8. ПРЕОБРАЗОВАНИЕ В МАССИВ");
        int[] resultArray = queueFromArray.ToArray();
        Console.Write("Результирующий массив: [");
        foreach (var item in resultArray)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine("]");

        Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА ===");
    }
}
