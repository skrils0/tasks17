using System;
using System.Collections.Generic;

public class MaxHeap<T> where T : IComparable<T>
{
    private List<T> data;

    // конструктор по умолчанию - создает пустую кучу
    public MaxHeap()
    {
        data = new List<T>();
    }

    // конструктор, создающий кучу из массива
    public MaxHeap(T[] array)
    {
        // копируем элементы из массива в список
        data = new List<T>(array);
        // построение кучи: начинаем с последнего нелистового узла и просеиваем вниз
        for (int i = data.Count / 2 - 1; i >= 0; i--)
        {
            HeapifyDown(i);
        }
    }

    // конструктор, создающий кучу из списка
    public MaxHeap(List<T> list)
    {
        // копируем элементы из переданного списка
        data = new List<T>(list);
        // аналогично предыдущему конструктору - строим кучу
        for (int i = data.Count / 2 - 1; i >= 0; i--)
        {
            HeapifyDown(i);
        }
    }

    // метод просеивания вверх
    private void HeapifyUp(int index)
    {
        // если дошли до корня - выходим
        if (index == 0) return;

        // вычисляем индекс родителя
        int parent = (index - 1) / 2;

        // если родитель меньше текущего элемента - нарушено свойство max-кучи
        if (data[parent].CompareTo(data[index]) < 0)
        {
            // меняем местами родителя и текущий элемент
            Swap(parent, index);
            // рекурсивно продолжаем просеивание вверх
            HeapifyUp(parent);
        }
    }

    // метод просеивания вниз
    private void HeapifyDown(int index)
    {
        // вычисляем индексы левого и правого потомков
        int left = 2 * index + 1;
        int right = 2 * index + 2;
        int largest = index;

        // если левый потомок существует и больше текущего наибольшего
        if (left < data.Count && data[left].CompareTo(data[largest]) > 0)
        {
            largest = left;
        }

        // если правый потомок существует и больше текущего наибольшего
        if (right < data.Count && data[right].CompareTo(data[largest]) > 0)
        {
            largest = right;
        }

        // если наибольший элемент не текущий, значит свойство кучи нарушено
        if (largest != index)
        {
            Swap(index, largest);
            
            HeapifyDown(largest);
        }
    }

    // вспомогательный метод для обмена двух элементов местами
    private void Swap(int i, int j)
    {
        T temp = data[i];
        data[i] = data[j];
        data[j] = temp;
    }

    // метод для нахождения максимального элемента (корня кучи)
    public T FindMax()
    {
        if (data.Count == 0)
        {
            throw new InvalidOperationException("Куча пуста");
        }
        return data[0]; // в max-куче максимальный элемент всегда в корне
    }

    // метод для удаления и возврата максимального элемента
    public T ExtractMax()
    {
        if (data.Count == 0)
        {
            throw new InvalidOperationException("Куча пуста");
        }

        // сохраняем максимальный элемент (корень)
        T max = data[0];



        //перемещаем последний элемент в корень
        data[0] = data[data.Count - 1];
        // удаляем последний элемент
        data.RemoveAt(data.Count - 1);

        // если в куче остались элементы, восстанавливаем свойство кучи
        if (data.Count > 0)
        {
            HeapifyDown(0);
        }

        return max;
    }

    // метод для увеличения значения элемента по указанному индексу
    public void IncreaseKey(int index, T newValue)
    {
        if (index < 0 || index >= data.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Индекс вне диапазона");
        }
        // проверяем, что новое значение действительно больше старого
        if (newValue.CompareTo(data[index]) < 0)
        {
            throw new ArgumentException("Новое значение меньше текущего");
        }

        // устанавливаем новое значение
        data[index] = newValue;
        // восстанавливаем свойство кучи (значение увеличилось - возможно нужно просеивание вверх)
        HeapifyUp(index);
    }

    // метод для добавления нового элемента в кучу
    public void Insert(T value)
    {
        // добавляем элемент в конец списка
        data.Add(value);
        // восстанавливаем свойство кучи просеиванием вверх
        HeapifyUp(data.Count - 1);
    }

    // метод для слияния текущей кучи с другой кучей
    public void Merge(MaxHeap<T> other)
    {
        // последовательно добавляем все элементы из другой кучи
        foreach (T element in other.data)
        {
            Insert(element);
        }
    }

    // метод для получения количества элементов в куче
    public int Size()
    {
        return data.Count;
    }

    // метод для проверки, пуста ли куча
    public bool IsEmpty()
    {
        return data.Count == 0;
    }

    // вспомогательный метод для вывода содержимого кучи (для отладки)
    public void Print()
    {
        Console.Write("Куча: ");
        foreach (T element in data)
        {
            Console.Write(element + " ");
        }
        Console.WriteLine();
    }
}

class Student
{
    public int Id { get; set; } 
}

// класс с примером использования кучи
class Program
{
    static void Main(string[] args)
    {
        try
        {
            // создание кучи из массива

            char[] initialData = { 'a', 'b', 'f', 't', 'y', 'i' };
            Console.Write("Исходный массив: ");
            foreach (int num in initialData)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine();

            Student[] students = { new Student() };

            MaxHeap<Student> = new MaxHeap<Student>(students);

            // создаем кучу из массива
            MaxHeap<char> heap = new MaxHeap<char>(initialData);
            heap.Print();

            //нахождение максимума
            Console.WriteLine("Максимальный элемент: " + heap.FindMax());



        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);


        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}