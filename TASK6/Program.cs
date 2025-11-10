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
        comparer = Comparer<T>.Default; // используем компаратор по умолчанию
    }

    // конструктор 2: очередь из массива
    public MyPriorityQueue(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        queue = new T[a.Length + 1];
        size = a.Length;
        comparer = Comparer<T>.Default;

        // копируем элементы из входного массива
        Array.Copy(a, 0, queue, 1, a.Length);

        // строим кучу из всех элементов
        BuildHeap();
    }

    // конструктор 3: пустая очередь с указанной начальной ёмкостью
    public MyPriorityQueue(int initialCapacity)
    {
        if (initialCapacity < 1)
            throw new ArgumentException("начальная ёмкость должна быть положительной");

        queue = new T[initialCapacity + 1];
        size = 0;
        comparer = Comparer<T>.Default;
    }

    // конструктор 4: пустая очередь с указанной ёмкостью и компаратором
    public MyPriorityQueue(int initialCapacity, IComparer<T> comparator)
    {
        if (initialCapacity < 1)
            throw new ArgumentException("начальная ёмкость должна быть положительной");

        queue = new T[initialCapacity + 1];
        size = 0;
        comparer = comparator ?? Comparer<T>.Default;
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
    }

    // добавление элемента в очередь
    public void Add(T e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        // проверяем, нужно ли увеличить ёмкость
        if (size >= queue.Length - 1)
            Resize();

        // увеличиваем размер и добавляем элемент в конец
        size++;
        queue[size] = e;

        // поднимаем элемент на нужную позицию в куче
        Swim(size);
    }

    // добавление всех элементов из массива
    public void AddAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        foreach (T item in a)
        {
            Add(item);
        }
    }

    // очистка очереди
    public void Clear()
    {
        for (int i = 1; i <= size; i++)
        {
            queue[i] = default(T);
        }
        size = 0;
    }

    // проверка наличия элемента в очереди
    public bool Contains(object obj)
    {
        if (obj == null) return false;

        for (int i = 1; i <= size; i++)
        {
            if (obj.Equals(queue[i]))
                return true;
        }
        return false;
    }

    // проверка наличия всех элементов из массива
    public bool ContainsAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        foreach (T item in a)
        {
            if (!Contains(item))
                return false;
        }
        return true;
    }

    // проверка пустоты очереди
    public bool IsEmpty()
    {
        return size == 0;
    }

    // удаление указанного элемента
    public bool Remove(object obj)
    {
        if (obj == null) return false;

        // ищем элемент в очереди
        for (int i = 1; i <= size; i++)
        {
            if (obj.Equals(queue[i]))
            {
                // заменяем удаляемый элемент последним
                queue[i] = queue[size];
                queue[size] = default(T);
                size--;

                // восстанавливаем свойства кучи
                if (i <= size)
                {
                    Swim(i);
                    Sink(i);
                }
                return true;
            }
        }
        return false;
    }

    // удаление всех указанных элементов
    public bool RemoveAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        bool modified = false;
        foreach (T item in a)
        {
            if (Remove(item))
                modified = true;
        }
        return modified;
    }

    // оставление только указанных элементов
    public bool RetainAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        bool modified = false;
        // создаем временный список для элементов, которые нужно сохранить
        List<T> toRetain = new List<T>(a);

        for (int i = size; i >= 1; i--)
        {
            if (!toRetain.Contains(queue[i]))
            {
                Remove(queue[i]);
                modified = true;
            }
        }
        return modified;
    }

    // получение размера очереди
    public int Size()
    {
        return size;
    }

    // преобразование в массив
    public T[] ToArray()
    {
        T[] result = new T[size];
        Array.Copy(queue, 1, result, 0, size);
        return result;
    }

    // преобразование в массив с указанным типом
    public T[] ToArray(T[] a)
    {
        if (a == null)
            return ToArray();

        if (a.Length < size)
            return ToArray();

        Array.Copy(queue, 1, a, 0, size);
        if (a.Length > size)
            a[size] = default(T);

        return a;
    }

    // получение элемента из головы очереди без удаления
    public T Element()
    {
        if (IsEmpty())
            throw new InvalidOperationException("очередь пуста");

        return queue[1];
    }

    // попытка добавления элемента (всегда true для неограниченной очереди)
    public bool Offer(T obj)
    {
        try
        {
            Add(obj);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // получение элемента из головы без удаления (возвращает null если пусто)
    public T Peek()
    {
        if (IsEmpty())
            return default(T);

        return queue[1];
    }

    // удаление и возврат элемента из головы очереди
    public T Poll()
    {
        if (IsEmpty())
            return default(T);

        // сохраняем корневой элемент (максимальный/минимальный)
        T result = queue[1];

        // заменяем корень последним элементом
        queue[1] = queue[size];
        queue[size] = default(T);
        size--;

        // восстанавливаем свойства кучи
        if (size > 0)
            Sink(1);

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

    // просеивание вверх (подъем элемента)
    private void Swim(int k)
    {
        while (k > 1 && Compare(k / 2, k) < 0)
        {
            Swap(k, k / 2);
            k = k / 2;
        }
    }

    // просеивание вниз (опускание элемента)
    private void Sink(int k)
    {
        while (2 * k <= size)
        {
            int j = 2 * k;
            if (j < size && Compare(j, j + 1) < 0)
                j++;

            if (Compare(k, j) >= 0)
                break;

            Swap(k, j);
            k = j;
        }
    }

    // построение кучи из всех элементов
    private void BuildHeap()
    {
        for (int i = size / 2; i >= 1; i--)
        {
            Sink(i);
        }
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
}

// пример использования
public class Program
{
    public static void Main()
    {
        // создаем очередь с приоритетами для целых чисел
        MyPriorityQueue<int> queue = new MyPriorityQueue<int>();

        // добавляем элементы
        queue.Add(5);
        queue.Add(10);
        queue.Add(3);
        queue.Add(8);

        Console.WriteLine("размер очереди: " + queue.Size());
        Console.WriteLine("первый элемент: " + queue.Peek());

        // извлекаем элементы в порядке приоритета
        Console.WriteLine("извлеченные элементы:");
        while (!queue.IsEmpty())
        {
            Console.Write(queue.Poll() + " ");
        }
    }
}