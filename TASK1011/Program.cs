using System;
using System.IO;

namespace VectorTasks
{

    public class MyVector<T>
    {
        protected T[] elementData;
        protected int elementCount;
        protected int capacityIncrement;

        // 1) Конструктор с полной настройкой
        public MyVector(int initialCapacity, int capacityIncrement)
        {
            if (initialCapacity < 0) throw new ArgumentException("Емкость не может быть отрицательной");
            this.elementData = new T[initialCapacity];
            this.capacityIncrement = capacityIncrement;
            this.elementCount = 0;
        }

        // 2) Конструктор только с емкостью
        public MyVector(int initialCapacity) : this(initialCapacity, 0) { }

        // 3) Конструктор по умолчанию
        public MyVector() : this(10, 0) { }

        // 4) Конструктор из массива
        public MyVector(T[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            elementCount = a.Length;
            elementData = new T[elementCount];
            capacityIncrement = 0;
            Array.Copy(a, elementData, elementCount);
        }


        private void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > elementData.Length)
            {
                int oldCapacity = elementData.Length;
                int newCapacity;

                if (capacityIncrement > 0)
                    newCapacity = oldCapacity + capacityIncrement;
                else
                    newCapacity = oldCapacity * 2;

               
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                
                if (newCapacity == 0) newCapacity = Math.Max(10, minCapacity);

                T[] newArray = new T[newCapacity];
                if (elementCount > 0)
                {
                    Array.Copy(elementData, newArray, elementCount);
                }
                elementData = newArray;
            }
        }

        // 5) add(T e)
        public void Add(T e)
        {
            EnsureCapacity(elementCount + 1);
            elementData[elementCount++] = e;
        }

        // 6) addAll(T[] a)
        public void AddAll(T[] a)
        {
            if (a == null || a.Length == 0) return;
            EnsureCapacity(elementCount + a.Length);
            Array.Copy(a, 0, elementData, elementCount, a.Length);
            elementCount += a.Length;
        }

        // 7) clear()
        public void Clear()
        {
            for (int i = 0; i < elementCount; i++)
                elementData[i] = default(T);
            elementCount = 0;
        }

        // 8) contains(object o)
        public bool Contains(object o)
        {
            return IndexOf(o) >= 0;
        }

        // 9) containsAll(T[] a)
        public bool ContainsAll(T[] a)
        {
            if (a == null) return false;
            foreach (var item in a)
                if (!Contains(item)) return false;
            return true;
        }

        // 10) isEmpty()
        public bool IsEmpty()
        {
            return elementCount == 0;
        }

        // 11) remove(object o)
        public bool Remove(object o)
        {
            int i = IndexOf(o);
            if (i >= 0)
            {
                RemoveElementAt(i);
                return true;
            }
            return false;
        }

        // 12) removeAll(T[] a)
        public bool RemoveAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;
            foreach (var item in a)
            {
                // Удаляем все вхождения элемента
                while (Contains(item))
                {
                    Remove(item);
                    modified = true;
                }
            }
            return modified;
        }

        // 13) retainAll(T[] a)
        public bool RetainAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;

            // Идем с конца, чтобы безопасно удалять
            for (int i = elementCount - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (var item in a)
                {
                    if (object.Equals(elementData[i], item))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    RemoveElementAt(i);
                    modified = true;
                }
            }
            return modified;
        }

        // 14) size()
        public int Size()
        {
            return elementCount;
        }

        // 15) toArray() -> object[]
        public object[] ToArray()
        {
            object[] result = new object[elementCount];
            Array.Copy(elementData, result, elementCount);
            return result;
        }

        // 16) toArray(T[] a)
        public T[] ToArray(T[] a)
        {
            if (a == null || a.Length < elementCount)
            {
                a = new T[elementCount];
            }
            Array.Copy(elementData, a, elementCount);
            return a;
        }

        // 17) add(int index, T e)
        public void Add(int index, T e)
        {
            if (index > elementCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            EnsureCapacity(elementCount + 1);
            Array.Copy(elementData, index, elementData, index + 1, elementCount - index);
            elementData[index] = e;
            elementCount++;
        }

        // 18) addAll(int index, T[] a)
        public void AddAll(int index, T[] a)
        {
            if (index > elementCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (a == null || a.Length == 0) return;

            EnsureCapacity(elementCount + a.Length);

            int numMoved = elementCount - index;
            if (numMoved > 0)
                Array.Copy(elementData, index, elementData, index + a.Length, numMoved);

            Array.Copy(a, 0, elementData, index, a.Length);
            elementCount += a.Length;
        }

        // 19) get(int index)
        public T Get(int index)
        {
            if (index >= elementCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            return elementData[index];
        }

        // 20) indexOf(object o)
        public int IndexOf(object o)
        {
            if (o == null)
            {
                for (int i = 0; i < elementCount; i++)
                    if (elementData[i] == null) return i;
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                    if (o.Equals(elementData[i])) return i;
            }
            return -1;
        }

        // 21) lastIndexOf(object o)
        public int LastIndexOf(object o)
        {
            if (o == null)
            {
                for (int i = elementCount - 1; i >= 0; i--)
                    if (elementData[i] == null) return i;
            }
            else
            {
                for (int i = elementCount - 1; i >= 0; i--)
                    if (o.Equals(elementData[i])) return i;
            }
            return -1;
        }

        // 22) remove(int index) -> возвращает удаленный элемент
        public T Remove(int index)
        {
            if (index >= elementCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            T oldValue = elementData[index];
            RemoveElementAt(index);
            return oldValue;
        }

        // 23) set(int index, T e)
        public T Set(int index, T e)
        {
            if (index >= elementCount || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            T oldValue = elementData[index];
            elementData[index] = e;
            return oldValue;
        }

        // 24) subList(int fromIndex, int toIndex)
        public MyVector<T> SubList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex > elementCount || fromIndex > toIndex)
                throw new ArgumentOutOfRangeException();

            int newSize = toIndex - fromIndex;
            MyVector<T> sub = new MyVector<T>(newSize);
            for (int i = fromIndex; i < toIndex; i++)
            {
                sub.Add(elementData[i]);
            }
            return sub;
        }

        // 25) firstElement()
        public T FirstElement()
        {
            if (elementCount == 0) throw new InvalidOperationException("Вектор пуст");
            return elementData[0];
        }

        // 26) lastElement()
        public T LastElement()
        {
            if (elementCount == 0) throw new InvalidOperationException("Вектор пуст");
            return elementData[elementCount - 1];
        }

        // 27) removeElementAt(int pos)
        public void RemoveElementAt(int pos)
        {
            if (pos >= elementCount || pos < 0)
                throw new ArgumentOutOfRangeException(nameof(pos));

            int numMoved = elementCount - pos - 1;
            if (numMoved > 0)
                Array.Copy(elementData, pos + 1, elementData, pos, numMoved);

            elementData[--elementCount] = default(T);
        }

        // 28) removeRange(int begin, int end)
        public void RemoveRange(int begin, int end)
        {
            if (begin < 0 || end > elementCount || begin > end)
                throw new ArgumentOutOfRangeException();

            int numMoved = elementCount - end;
            
            if (numMoved > 0)
            {
                Array.Copy(elementData, end, elementData, begin, numMoved);
            }

            int newCount = elementCount - (end - begin);
            
            while (elementCount != newCount)
            {
                elementData[--elementCount] = default(T);
            }
        }

        
        public override string ToString()
        {
            if (elementCount == 0) return "[]";
            string s = "[";
            for (int i = 0; i < elementCount; i++)
            {
                s += elementData[i];
                if (i < elementCount - 1) s += ", ";
            }
            s += "]";
            return s;
        }

        internal void AddAll(object[] objects)
        {
            throw new NotImplementedException();
        }
    }

    public class IpVector : MyVector<string>
    {
        public void ProcessFiles(string inputPath, string outputPath)
        {
            Console.WriteLine($"Чтение из файла: {inputPath}");
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Ошибка: Файл не найден.");
                return;
            }

            
            string[] lines = File.ReadAllLines(inputPath);
            this.Clear();
            this.AddAll(lines);

            
            MyVector<string> validIps = new MyVector<string>();

            for (int i = 0; i < this.Size(); i++)
            {
                string line = this.Get(i);

               
                string currentToken = "";

                string lineWithEnd = line + " ";

                foreach (char c in lineWithEnd)
                {
                    
                    bool isIpChar = char.IsDigit(c) || c == '.';

                    if (isIpChar)
                    {
                        currentToken += c;
                    }
                    else
                    {
                        
                        if (currentToken.Length > 0)
                        {
                            if (IsValidIp(currentToken))
                            {
                                validIps.Add(currentToken);
                            }
                            currentToken = "";
                        }
                    }
                }
            }

            
            Console.WriteLine($"Найдено IP адресов: {validIps.Size()}");
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                for (int i = 0; i < validIps.Size(); i++)
                {
                    writer.WriteLine(validIps.Get(i));
                }
            }
            Console.WriteLine($"Результат записан в: {outputPath}");
        }

        private bool IsValidIp(string token)
        {
           
            if (token.StartsWith(".") || token.EndsWith(".")) return false;

            string[] parts = token.Split('.');
            
            if (parts.Length != 4) return false;

            foreach (string part in parts)
            {
                if (part.Length == 0) return false;

                if (part.Length > 1 && part.StartsWith("0")) return false;

                if (int.TryParse(part, out int num))
                {
                    if (num < 0 || num > 255) return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n================ МЕНЮ ================");
                Console.WriteLine("1. Тест MyVector (Задача 10)");
                Console.WriteLine("2. Поиск IP адресов в файле (Задача 11)");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunTask10();
                        break;
                    case "2":
                        RunTask11();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод.");
                        break;
                }
            }
        }

        static void RunTask10()
        {
            Console.WriteLine("\n--- Демонстрация Задачи 10 ---");

            try
            {
                // 1-3 Конструкторы
                MyVector<int> vec = new MyVector<int>(5, 2);
                Console.WriteLine($"Создан вектор. Empty: {vec.IsEmpty()}, Size: {vec.Size()}");

                // 5. Add
                for (int i = 1; i <= 5; i++) vec.Add(i * 10);
                Console.WriteLine($"Заполнили элементами: {vec}"); // [10, 20, 30, 40, 50]

                // Расширение (был размер 5, добавляем 6-й, increment=2 -> new capacity 7)
                vec.Add(60);
                Console.WriteLine($"После добавления 60: {vec}");

                // 17. Add по индексу
                vec.Add(0, 5);
                Console.WriteLine($"Вставка 5 в начало: {vec}");

                // 19. Get
                Console.WriteLine($"Элемент с индексом 2: {vec.Get(2)}");

                // 23. Set
                vec.Set(2, 999);
                Console.WriteLine($"Set(2, 999): {vec}");

                // 20. IndexOf
                Console.WriteLine($"Индекс числа 999: {vec.IndexOf(999)}");

                // 22. Remove(index)
                int removed = vec.Remove(2);
                Console.WriteLine($"Удалили элемент по индексу 2 ({removed}): {vec}");

                // 11. Remove(object)
                vec.Remove((object)60);
                Console.WriteLine($"Удалили объект 60: {vec}");

                // 24. SubList
                var sub = vec.SubList(1, 3);
                Console.WriteLine($"SubList(1, 3): {sub}");

                // 25, 26 First/Last
                Console.WriteLine($"Первый: {vec.FirstElement()}, Последний: {vec.LastElement()}");

                // 28. RemoveRange
                vec.RemoveRange(0, 2);
                Console.WriteLine($"RemoveRange(0, 2): {vec}");

                // 4. Конструктор из массива и AddAll
                MyVector<int> vec2 = new MyVector<int>(new int[] { 1, 2, 3 });
                vec.AddAll(vec2.ToArray());
                Console.WriteLine($"После AddAll([1,2,3]): {vec}");

                // 13. RetainAll
                vec.RetainAll(new int[] { 1, 2, 50 });
                Console.WriteLine($"RetainAll([1, 2, 50]): {vec} (оставили только эти)");

                // 7. Clear
                vec.Clear();
                Console.WriteLine($"После Clear: {vec}, Empty: {vec.IsEmpty()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при тесте: {ex.Message}");
            }
        }

        static void RunTask11()
        {
            Console.WriteLine("\n--- Демонстрация Задачи 11 ---");
            string inFile = "input.txt";
            string outFile = "output.txt";

            // Создадим файл для теста
            string testContent =
                "Valid: 192.168.1.1 and 10.0.0.5\n" +
                "Invalid: 192.168.1.1.5 and 300.1.1.1\n" +
                "Overlap: 123.192.168.1.1\n" +
                "Valid 2: 1.1.1.1";

            File.WriteAllText(inFile, testContent);
            Console.WriteLine("Создан тестовый файл input.txt с содержимым:");
            Console.WriteLine("--------------------------------");
            Console.WriteLine(testContent);
            Console.WriteLine("--------------------------------");

            IpVector ipProcessor = new IpVector();
            ipProcessor.ProcessFiles(inFile, outFile);

            if (File.Exists(outFile))
            {
                Console.WriteLine("\nСодержимое output.txt:");
                Console.WriteLine(File.ReadAllText(outFile));
            }
        }
    }
}