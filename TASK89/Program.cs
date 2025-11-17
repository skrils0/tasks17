using System;
using System.IO;

namespace Tasks8and9
{
    // Задача 8: Обобщённый динамический массив
    // Реализация без List<T>, с простыми циклами.
    public class MyArrayList<T>
    {
        // Массив для хранения элементов
        protected T[] elementData;

        // Текущее число элементов
        protected int _size;

        // 1) Конструктор MyArrayList() — создаёт пустой список
        public MyArrayList()
        {
            elementData = new T[0];
            _size = 0;
        }

        // 2) Конструктор MyArrayList(T[] a) — копируем элементы из массива
        public MyArrayList(T[] a)
        {
            if (a == null)
            {
                elementData = new T[0];
                _size = 0;
            }
            else
            {
                elementData = new T[a.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    elementData[i] = a[i];
                }
                _size = a.Length;
            }
        }

        // 3) Конструктор MyArrayList(int capacity) — создаём пустой список с заданной ёмкостью
        public MyArrayList(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            elementData = new T[capacity];
            _size = 0;
        }

        // Расширяем массив, если не хватает места
        private void EnsureCapacity(int minCapacity)
        {
            if (elementData.Length >= minCapacity) return;

            int oldCap = elementData.Length;
            int newCap = oldCap + (oldCap / 2) + 1; // рост в 1.5x + 1
            if (newCap < minCapacity) newCap = minCapacity;
            if (newCap < 0) newCap = int.MaxValue;

            T[] newArr = new T[newCap];
            for (int i = 0; i < _size; i++)
            {
                newArr[i] = elementData[i];
            }
            elementData = newArr;
        }

        // 4) Добавить элемент в конец
        public void add(T e)
        {
            EnsureCapacity(_size + 1);
            elementData[_size] = e;
            _size++;
        }

        // 5) Добавить все элементы из массива
        public void addAll(T[] a)
        {
            if (a == null || a.Length == 0) return;
            EnsureCapacity(_size + a.Length);
            for (int i = 0; i < a.Length; i++)
            {
                elementData[_size] = a[i];
                _size++;
            }
        }

        // 6) Удалить все элементы
        public void clear()
        {
            for (int i = 0; i < _size; i++)
            {
                elementData[i] = default(T);
            }
            _size = 0;
        }

        // 7) Есть ли указанный объект
        public bool contains(object o)
        {
            return indexOf(o) >= 0;
        }

        // 8) Содержатся ли все элементы массива
        public bool containsAll(T[] a)
        {
            if (a == null) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (indexOf(a[i]) == -1) return false;
            }
            return true;
        }

        // 9) Пустой ли список
        public bool isEmpty()
        {
            return _size == 0;
        }

        // 10) Удалить объект (первое вхождение)
        public bool remove(object o)
        {
            if (_size == 0) return false;

            if (o == null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if (object.Equals(elementData[i], null))
                    {
                        RemoveAtIndex(i);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (!(o is T)) return false;
                T value = (T)o;
                for (int i = 0; i < _size; i++)
                {
                    if (object.Equals(elementData[i], value))
                    {
                        RemoveAtIndex(i);
                        return true;
                    }
                }
                return false;
            }
        }

        // 11) Удалить все указанные элементы
        public bool removeAll(T[] a)
        {
            if (a == null || a.Length == 0 || _size == 0) return false;

            int write = 0;
            bool changed = false;

            for (int i = 0; i < _size; i++)
            {
                T item = elementData[i];
                if (!ContainsInArray(item, a))
                {
                    elementData[write] = elementData[i];
                    write++;
                }
                else
                {
                    changed = true;
                }
            }

            for (int k = write; k < _size; k++)
            {
                elementData[k] = default(T);
            }
            _size = write;

            return changed;
        }

        // 12) Оставить только указанные элементы
        public bool retainAll(T[] a)
        {
            if (_size == 0) return false;

            if (a == null || a.Length == 0)
            {
                bool changed = _size != 0;
                clear();
                return changed;
            }

            int write = 0;
            bool changed2 = false;

            for (int i = 0; i < _size; i++)
            {
                T item = elementData[i];
                if (ContainsInArray(item, a))
                {
                    elementData[write] = item;
                    write++;
                }
                else
                {
                    changed2 = true;
                }
            }

            for (int k = write; k < _size; k++)
            {
                elementData[k] = default(T);
            }
            _size = write;

            return changed2;
        }

        // 13) Текущий размер
        public int size()
        {
            return _size;
        }

        // 14) Вернуть массив объектов
        public object[] toArray()
        {
            object[] arr = new object[_size];
            for (int i = 0; i < _size; i++)
            {
                arr[i] = elementData[i];
            }
            return arr;
        }

        // 15) Вернуть массив T[]
        public T[] toArray(T[] a)
        {
            if (a == null || a.Length < _size)
            {
                T[] result = new T[_size];
                for (int i = 0; i < _size; i++)
                {
                    result[i] = elementData[i];
                }
                return result;
            }

            for (int i = 0; i < _size; i++)
            {
                a[i] = elementData[i];
            }
            if (a.Length > _size)
            {
                a[_size] = default(T); // просто "маркер конца"
            }
            return a;
        }

        // 16) Вставка по индексу
        public void add(int index, T e)
        {
            if (index < 0 || index > _size) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureCapacity(_size + 1);
            for (int i = _size; i > index; i--)
            {
                elementData[i] = elementData[i - 1];
            }
            elementData[index] = e;
            _size++;
        }

        // 17) Вставка массива по индексу
        public void addAll(int index, T[] a)
        {
            if (index < 0 || index > _size) throw new ArgumentOutOfRangeException(nameof(index));
            if (a == null || a.Length == 0) return;

            EnsureCapacity(_size + a.Length);
            for (int i = _size - 1; i >= index; i--)
            {
                elementData[i + a.Length] = elementData[i];
            }
            for (int i = 0; i < a.Length; i++)
            {
                elementData[index + i] = a[i];
            }
            _size += a.Length;
        }

        // 18) Получить элемент по индексу
        public T get(int index)
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            return elementData[index];
        }

        // 19) Индекс первого вхождения
        public int indexOf(object o)
        {
            if (o == null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if (object.Equals(elementData[i], null)) return i;
                }
                return -1;
            }
            else
            {
                if (!(o is T)) return -1;
                T value = (T)o;
                for (int i = 0; i < _size; i++)
                {
                    if (object.Equals(elementData[i], value)) return i;
                }
                return -1;
            }
        }

        // 20) Индекс последнего вхождения
        public int lastIndexOf(object o)
        {
            if (o == null)
            {
                for (int i = _size - 1; i >= 0; i--)
                {
                    if (object.Equals(elementData[i], null)) return i;
                }
                return -1;
            }
            else
            {
                if (!(o is T)) return -1;
                T value = (T)o;
                for (int i = _size - 1; i >= 0; i--)
                {
                    if (object.Equals(elementData[i], value)) return i;
                }
                return -1;
            }
        }

        // 21) Удалить по индексу и вернуть элемент
        public T remove(int index)
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            T removed = elementData[index];
            RemoveAtIndex(index);
            return removed;
        }

        // 22) Заменить по индексу и вернуть старое значение
        public T set(int index, T e)
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            T old = elementData[index];
            elementData[index] = e;
            return old;
        }

        // 23) Вернуть подсписок [fromIndex; toIndex)
        public MyArrayList<T> subList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex > _size || fromIndex > toIndex)
                throw new ArgumentOutOfRangeException("Диапазон неправильный");

            MyArrayList<T> result = new MyArrayList<T>(toIndex - fromIndex);
            for (int i = fromIndex; i < toIndex; i++)
            {
                result.add(elementData[i]);
            }
            return result;
        }

        // Внутренние помощники
        private void RemoveAtIndex(int index)
        {
            for (int i = index; i < _size - 1; i++)
            {
                elementData[i] = elementData[i + 1];
            }
            elementData[_size - 1] = default(T);
            _size--;
        }

        private bool ContainsInArray(T value, T[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (object.Equals(value, a[i])) return true;
            }
            return false;
        }
    }

    // Задача 9: список тегов — наследник от MyArrayList<string>
    public class TagArrayList : MyArrayList<string>
    {
        public TagArrayList() : base() { }

        // Проверка корректности тега
        public static bool IsValidTag(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            if (s[0] != '<') return false;
            if (s[s.Length - 1] != '>') return false;

            int pos = 1;
            if (pos < s.Length - 1 && s[pos] == '/') pos++;
            if (pos >= s.Length - 1) return false;

            char first = s[pos];
            if (!char.IsLetter(first)) return false;

            for (int i = pos + 1; i < s.Length - 1; i++)
            {
                char c = s[i];
                if (!char.IsLetter(c) && !char.IsDigit(c)) return false;
            }
            return true;
        }

        // Нормализация для сравнения (без <, >, / и в нижнем регистре)
        public static string NormalizeTag(string s)
        {
            string inner = s.Substring(1, s.Length - 2);
            if (inner.StartsWith("/")) inner = inner.Substring(1);
            inner = inner.ToLowerInvariant();
            return inner;
        }

        // Удалить дубликаты без учёта '/' и регистра (оставляем первое вхождение)
        public bool removeDuplicatesIgnoreSlashAndCase()
        {
            if (_size <= 1) return false;

            MyArrayList<string> seen = new MyArrayList<string>();
            int write = 0;
            bool changed = false;

            for (int i = 0; i < _size; i++)
            {
                string tag = elementData[i];
                if (tag == null) continue;
                string norm = NormalizeTag(tag);

                bool already = false;
                for (int j = 0; j < seen.size(); j++)
                {
                    if (seen.get(j) == norm)
                    {
                        already = true;
                        break;
                    }
                }

                if (!already)
                {
                    seen.add(norm);
                    elementData[write] = tag;
                    write++;
                }
                else
                {
                    changed = true;
                }
            }

            for (int k = write; k < _size; k++)
            {
                elementData[k] = null;
            }
            _size = write;

            return changed;
        }
    }

    class Program
    {
        // Печать списка (для наглядности)
        static void PrintList<T>(string title, MyArrayList<T> list)
        {
            Console.Write(title + " [size=" + list.size() + "]: ");
            for (int i = 0; i < list.size(); i++)
            {
                Console.Write(list.get(i) + " ");
            }
            Console.WriteLine();
        }

        // Извлекаем теги из строки и добавляем в список
        static void ExtractTagsFromLine(string line, TagArrayList list)
        {
            if (string.IsNullOrEmpty(line)) return;

            int i = 0;
            while (i < line.Length)
            {
                if (line[i] == '<')
                {
                    int j = i + 1;
                    while (j < line.Length && line[j] != '>') j++;

                    if (j < line.Length && line[j] == '>')
                    {
                        string candidate = line.Substring(i, j - i + 1);
                        if (TagArrayList.IsValidTag(candidate))
                        {
                            list.add(candidate);
                        }
                        i = j + 1;
                    }
                    else
                    {
                        break; // незакрытый тег — прерываем
                    }
                }
                else
                {
                    i++;
                }
            }
        }

        // Демонстрация всех 23 пунктов для задания 8 (на int)
        static void DemoTask8_All()
        {
            Console.WriteLine("===== Задание 8: Демонстрация 23 пунктов =====");

            Console.WriteLine("1) Конструктор MyArrayList()");
            MyArrayList<int> list = new MyArrayList<int>();
            PrintList("После конструктора по умолчанию", list);

            Console.WriteLine("2) Конструктор MyArrayList(T[] a)");
            MyArrayList<int> listFromArray = new MyArrayList<int>(new int[] { 10, 20, 30 });
            PrintList("Создан из массива {10,20,30}", listFromArray);

            Console.WriteLine("3) Конструктор MyArrayList(int capacity)");
            MyArrayList<int> listWithCap = new MyArrayList<int>(5);
            PrintList("Создан с ёмкостью 5 (пока пустой)", listWithCap);

            // 4) add
            Console.WriteLine("4) add(T e)");
            list.add(1);
            list.add(2);
            list.add(3);
            PrintList("После add 1,2,3", list);

            // 5) addAll
            Console.WriteLine("5) addAll(T[] a)");
            list.addAll(new int[] { 4, 5 });
            PrintList("После addAll {4,5}", list);

            // 9) isEmpty
            Console.WriteLine("9) isEmpty() -> " + list.isEmpty());

            // 13) size
            Console.WriteLine("13) size() -> " + list.size());

            // 18) get
            Console.WriteLine("18) get(2) -> " + list.get(2));

            // 16) add(index, e)
            Console.WriteLine("16) add(2, 99)");
            list.add(2, 99);
            PrintList("После add(2,99)", list);

            // 17) addAll(index, arr)
            Console.WriteLine("17) addAll(1, {7,7})");
            list.addAll(1, new int[] { 7, 7 });
            PrintList("После addAll(1,{7,7})", list);

            // 22) set
            Console.WriteLine("22) set(3, 100) (старое значение вернётся)");
            int oldVal = list.set(3, 100);
            Console.WriteLine("Старое значение было: " + oldVal);
            PrintList("После set(3,100)", list);

            // 19) indexOf
            Console.WriteLine("19) indexOf(7) -> " + list.indexOf(7));

            // 20) lastIndexOf
            Console.WriteLine("20) lastIndexOf(7) -> " + list.lastIndexOf(7));

            // 7) contains
            Console.WriteLine("7) contains(100) -> " + list.contains(100));
            Console.WriteLine("7) contains(42) -> " + list.contains(42));

            // 8) containsAll
            Console.WriteLine("8) containsAll({99,4}) -> " + list.containsAll(new int[] { 99, 4 }));

            // 10) remove(object o)
            Console.WriteLine("10) remove(7) (первое вхождение)");
            list.remove((object)7);
            PrintList("После remove(7)", list);

            // 21) remove(index)
            Console.WriteLine("21) remove(0) -> удалён: " + list.remove(0));
            PrintList("После remove(0)", list);

            // 11) removeAll
            Console.WriteLine("11) removeAll({4,5})");
            list.removeAll(new int[] { 4, 5 });
            PrintList("После removeAll({4,5})", list);

            // 12) retainAll
            Console.WriteLine("12) retainAll({2,99,100})");
            list.retainAll(new int[] { 2, 99, 100 });
            PrintList("После retainAll({2,99,100})", list);

            // 14) toArray()
            Console.WriteLine("14) toArray()");
            object[] arrObj = list.toArray();
            Console.Write("Содержимое object[]: ");
            for (int i = 0; i < arrObj.Length; i++) Console.Write(arrObj[i] + " ");
            Console.WriteLine();

            // 15) toArray(T[] a) — с большим массивом
            Console.WriteLine("15) toArray(T[] a) (передаём массив больше по размеру)");
            int[] arrInt = list.toArray(new int[list.size() + 2]);
            Console.Write("Содержимое int[]: ");
            for (int i = 0; i < list.size(); i++) Console.Write(arrInt[i] + " ");
            Console.WriteLine();

            // 15) toArray(T[] a) — с null (создаст новый массив)
            Console.WriteLine("15) toArray(null)");
            int[] arrInt2 = list.toArray((int[])null);
            Console.Write("Содержимое, созданное toArray(null): ");
            for (int i = 0; i < arrInt2.Length; i++) Console.Write(arrInt2[i] + " ");
            Console.WriteLine();

            // 23) subList
            Console.WriteLine("23) subList(0, Math.Min(2, size))");
            int to = list.size() < 2 ? list.size() : 2;
            MyArrayList<int> sub = list.subList(0, to);
            PrintList("Подсписок", sub);

            // 6) clear
            Console.WriteLine("6) clear()");
            list.clear();
            PrintList("После clear()", list);
            Console.WriteLine("9) isEmpty() -> " + list.isEmpty());
            Console.WriteLine("13) size() -> " + list.size());
        }

        // Демонстрация всех 23 пунктов для задания 9 (на строках TagArrayList)
        static void DemoTask9_All()
        {
            Console.WriteLine("===== Задание 9: Чтение тегов и демонстрация 23 пунктов =====");

            // Часть 1: читаем теги из файла и убираем дубли
            TagArrayList tagsFromFile = new TagArrayList();

            string path = "input.txt";
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл input.txt не найден. Создаю пример файла...");
                File.WriteAllLines(path, new string[]
                {
                    "<html><body><H1>Title</H1></body></html>",
                    "</html><bOdY><h1>Another</H1></Body>",
                    "text<PrIvet>data</privet>zzz"
                });
            }

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                ExtractTagsFromLine(lines[i], tagsFromFile);
            }

            PrintList("Теги из файла", tagsFromFile);

            Console.WriteLine("Удаляем дубли без учёта '/' и регистра");
            tagsFromFile.removeDuplicatesIgnoreSlashAndCase();
            PrintList("После удаления дублей", tagsFromFile);

            // Часть 2: Демонстрация всех методов на строках
            Console.WriteLine();
            Console.WriteLine("=== Демонстрация 23 пунктов на TagArrayList<string> ===");

            Console.WriteLine("1) Конструктор MyArrayList() (наследуется)");
            TagArrayList list = new TagArrayList();
            PrintList("После конструктора по умолчанию", list);

            Console.WriteLine("2) Конструктор MyArrayList(T[] a)");
            TagArrayList listFromArray = new TagArrayList();
            listFromArray.addAll(new string[] { "<a>", "</A>", "<b>" }); // так как конструкторы не наследуются, просто addAll
            PrintList("Создан из массива {<a>,</A>,<b>}", listFromArray);

            Console.WriteLine("3) Конструктор MyArrayList(int capacity)");
            // Для TagArrayList нет своего конструктора с capacity, поэтому покажем на родительском:
            MyArrayList<string> capDemo = new MyArrayList<string>(5);
            PrintList("Родительский список с ёмкостью 5 (пока пустой)", capDemo);

            // 4) add
            Console.WriteLine("4) add(T e)");
            list.add("<a>");
            list.add("</A>");
            list.add("<b>");
            PrintList("После add <a>, </A>, <b>", list);

            // 5) addAll
            Console.WriteLine("5) addAll(T[] a)");
            list.addAll(new string[] { "<c>", "<d>" });
            PrintList("После addAll {<c>,<d>}", list);

            // 9) isEmpty
            Console.WriteLine("9) isEmpty() -> " + list.isEmpty());

            // 13) size
            Console.WriteLine("13) size() -> " + list.size());

            // 18) get
            Console.WriteLine("18) get(2) -> " + list.get(2));

            // 16) add(index, e)
            Console.WriteLine("16) add(2, <x>)");
            list.add(2, "<x>");
            PrintList("После add(2,<x>)", list);

            // 17) addAll(index, arr)
            Console.WriteLine("17) addAll(1, {<y>,<y>})");
            list.addAll(1, new string[] { "<y>", "<y>" });
            PrintList("После addAll(1,{<y>,<y>})", list);

            // 22) set
            Console.WriteLine("22) set(3, <z>)");
            string old = list.set(3, "<z>");
            Console.WriteLine("Старое значение было: " + old);
            PrintList("После set(3,<z>)", list);

            // 19) indexOf
            Console.WriteLine("19) indexOf(<y>) -> " + list.indexOf("<y>"));

            // 20) lastIndexOf
            Console.WriteLine("20) lastIndexOf(<y>) -> " + list.lastIndexOf("<y>"));

            // 7) contains
            Console.WriteLine("7) contains(<z>) -> " + list.contains("<z>"));
            Console.WriteLine("7) contains(<q>) -> " + list.contains("<q>)"));

            // 8) containsAll
            Console.WriteLine("8) containsAll({<a>,<c>}) -> " + list.containsAll(new string[] { "<a>", "<c>" }));

            // 10) remove(object o)
            Console.WriteLine("10) remove(<y>)");
            list.remove((object)"<y>");
            PrintList("После remove(<y>)", list);

            // 21) remove(index)
            Console.WriteLine("21) remove(0) -> удалён: " + list.remove(0));
            PrintList("После remove(0)", list);

            // 11) removeAll
            Console.WriteLine("11) removeAll({<c>,<d>})");
            list.removeAll(new string[] { "<c>", "<d>" });
            PrintList("После removeAll({<c>,<d>})", list);

            // 12) retainAll
            Console.WriteLine("12) retainAll({<x>,<z>})");
            list.retainAll(new string[] { "<x>", "<z>" });
            PrintList("После retainAll({<x>,<z>})", list);

            // 14) toArray()
            Console.WriteLine("14) toArray()");
            object[] arrObj = list.toArray();
            Console.Write("Содержимое object[]: ");
            for (int i = 0; i < arrObj.Length; i++) Console.Write(arrObj[i] + " ");
            Console.WriteLine();

            // 15) toArray(T[] a) — с большим массивом
            Console.WriteLine("15) toArray(T[] a) (передаём массив больше по размеру)");
            string[] arrStr = list.toArray(new string[list.size() + 3]);
            Console.Write("Содержимое string[]: ");
            for (int i = 0; i < list.size(); i++) Console.Write(arrStr[i] + " ");
            Console.WriteLine();

            // 15) toArray(T[] a) — с null
            Console.WriteLine("15) toArray(null)");
            string[] arrStr2 = list.toArray((string[])null);
            Console.Write("Содержимое, созданное toArray(null): ");
            for (int i = 0; i < arrStr2.Length; i++) Console.Write(arrStr2[i] + " ");
            Console.WriteLine();

            // 23) subList
            Console.WriteLine("23) subList(0, Math.Min(2, size))");
            int to = list.size() < 2 ? list.size() : 2;
            MyArrayList<string> sub = list.subList(0, to);
            PrintList("Подсписок", sub);

            // 6) clear
            Console.WriteLine("6) clear()");
            list.clear();
            PrintList("После clear()", list);
            Console.WriteLine("9) isEmpty() -> " + list.isEmpty());
            Console.WriteLine("13) size() -> " + list.size());
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Выберите задание: 8 — динамический массив, 9 — теги из файла");
                Console.Write("Введите 8 или 9: ");
                string choice = Console.ReadLine();

                if (choice == "9")
                {
                    DemoTask9_All();
                }
                else if (choice == "8")
                {
                    DemoTask8_All();
                }
                else
                {
                    Console.WriteLine("Неверный ввод. По умолчанию — задание 8.");
                    DemoTask8_All();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}