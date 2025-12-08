using System;
using System.IO;
using System.Collections.Generic; // Используется только для Dictionary (хранение переменных), сами структуры данных - свои.
using System.Globalization;

namespace VectorTasks
{
    // ==========================================
    // ЗАДАЧА 10: Обобщённый класс MyVector
    // ==========================================
    public class MyVector<T>
    {
        protected T[] elementData;
        protected int elementCount;
        protected int capacityIncrement;

        public MyVector(int initialCapacity, int capacityIncrement)
        {
            if (initialCapacity < 0) throw new ArgumentException("Емкость не может быть отрицательной");
            this.elementData = new T[initialCapacity];
            this.capacityIncrement = capacityIncrement;
            this.elementCount = 0;
        }

        public MyVector(int initialCapacity) : this(initialCapacity, 0) { }
        public MyVector() : this(10, 0) { }

        public MyVector(T[] a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            elementCount = a.Length;
            elementData = new T[elementCount];
            capacityIncrement = 0;
            Array.Copy(a, elementData, elementCount);
        }

        protected void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > elementData.Length)
            {
                int oldCapacity = elementData.Length;
                int newCapacity;
                if (capacityIncrement > 0) newCapacity = oldCapacity + capacityIncrement;
                else newCapacity = oldCapacity * 2;

                if (newCapacity < minCapacity) newCapacity = minCapacity;
                if (newCapacity == 0) newCapacity = 10;

                T[] newArray = new T[newCapacity];
                if (elementCount > 0) Array.Copy(elementData, newArray, elementCount);
                elementData = newArray;
            }
        }

        public void Add(T e)
        {
            EnsureCapacity(elementCount + 1);
            elementData[elementCount++] = e;
        }

        public void AddAll(T[] a)
        {
            if (a == null || a.Length == 0) return;
            EnsureCapacity(elementCount + a.Length);
            Array.Copy(a, 0, elementData, elementCount, a.Length);
            elementCount += a.Length;
        }

        public void Clear()
        {
            Array.Clear(elementData, 0, elementCount);
            elementCount = 0;
        }

        public bool Contains(object o) => IndexOf(o) >= 0;

        public bool ContainsAll(T[] a)
        {
            if (a == null) return false;
            foreach (var item in a) if (!Contains(item)) return false;
            return true;
        }

        public bool IsEmpty() => elementCount == 0;

        public bool Remove(object o)
        {
            int i = IndexOf(o);
            if (i >= 0) { RemoveElementAt(i); return true; }
            return false;
        }

        public bool RemoveAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;
            foreach (var item in a)
                while (Contains(item)) { Remove(item); modified = true; }
            return modified;
        }

        public bool RetainAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;
            for (int i = elementCount - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (var item in a) if (object.Equals(elementData[i], item)) { found = true; break; }
                if (!found) { RemoveElementAt(i); modified = true; }
            }
            return modified;
        }

        public int Size() => elementCount;

        public object[] ToArray()
        {
            object[] result = new object[elementCount];
            Array.Copy(elementData, result, elementCount);
            return result;
        }

        public T[] ToArray(T[] a)
        {
            if (a == null || a.Length < elementCount) a = new T[elementCount];
            Array.Copy(elementData, a, elementCount);
            return a;
        }

        public void Add(int index, T e)
        {
            if (index > elementCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureCapacity(elementCount + 1);
            Array.Copy(elementData, index, elementData, index + 1, elementCount - index);
            elementData[index] = e;
            elementCount++;
        }

        public void AddAll(int index, T[] a)
        {
            if (index > elementCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (a == null || a.Length == 0) return;
            EnsureCapacity(elementCount + a.Length);
            int numMoved = elementCount - index;
            if (numMoved > 0) Array.Copy(elementData, index, elementData, index + a.Length, numMoved);
            Array.Copy(a, 0, elementData, index, a.Length);
            elementCount += a.Length;
        }

        public T Get(int index)
        {
            if (index >= elementCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            return elementData[index];
        }

        public int IndexOf(object o)
        {
            if (o == null)
            {
                for (int i = 0; i < elementCount; i++) if (elementData[i] == null) return i;
            }
            else
            {
                for (int i = 0; i < elementCount; i++) if (o.Equals(elementData[i])) return i;
            }
            return -1;
        }

        public int LastIndexOf(object o)
        {
            if (o == null)
            {
                for (int i = elementCount - 1; i >= 0; i--) if (elementData[i] == null) return i;
            }
            else
            {
                for (int i = elementCount - 1; i >= 0; i--) if (o.Equals(elementData[i])) return i;
            }
            return -1;
        }

        public T Remove(int index)
        {
            if (index >= elementCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            T oldValue = elementData[index];
            RemoveElementAt(index);
            return oldValue;
        }

        public T Set(int index, T e)
        {
            if (index >= elementCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            T oldValue = elementData[index];
            elementData[index] = e;
            return oldValue;
        }

        public MyVector<T> SubList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || toIndex > elementCount || fromIndex > toIndex) throw new ArgumentOutOfRangeException();
            MyVector<T> sub = new MyVector<T>(toIndex - fromIndex);
            for (int i = fromIndex; i < toIndex; i++) sub.Add(elementData[i]);
            return sub;
        }

        public T FirstElement()
        {
            if (elementCount == 0) throw new InvalidOperationException("Вектор пуст");
            return elementData[0];
        }

        public T LastElement()
        {
            if (elementCount == 0) throw new InvalidOperationException("Вектор пуст");
            return elementData[elementCount - 1];
        }

        public void RemoveElementAt(int pos)
        {
            if (pos >= elementCount || pos < 0) throw new ArgumentOutOfRangeException(nameof(pos));
            int numMoved = elementCount - pos - 1;
            if (numMoved > 0) Array.Copy(elementData, pos + 1, elementData, pos, numMoved);
            elementData[--elementCount] = default(T);
        }

        public void RemoveRange(int begin, int end)
        {
            if (begin < 0 || end > elementCount || begin > end) throw new ArgumentOutOfRangeException();
            int numMoved = elementCount - end;
            if (numMoved > 0) Array.Copy(elementData, end, elementData, begin, numMoved);
            int newCount = elementCount - (end - begin);
            while (elementCount != newCount) elementData[--elementCount] = default(T);
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
    }

    // ==========================================
    // ЗАДАЧА 11: Работа с файлами (Наследование)
    // ==========================================
    public class IpVector : MyVector<string>
    {
        public void ProcessFiles(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath)) { Console.WriteLine("Файл не найден."); return; }
            string[] lines = File.ReadAllLines(inputPath);
            Clear();
            AddAll(lines);

            MyVector<string> validIps = new MyVector<string>();
            for (int i = 0; i < Size(); i++)
            {
                string line = Get(i) + " ";
                string currentToken = "";
                foreach (char c in line)
                {
                    if (char.IsDigit(c) || c == '.') currentToken += c;
                    else if (currentToken.Length > 0)
                    {
                        if (IsValidIp(currentToken)) validIps.Add(currentToken);
                        currentToken = "";
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                for (int i = 0; i < validIps.Size(); i++) writer.WriteLine(validIps.Get(i));
            }
            Console.WriteLine($"Результат записан в: {outputPath} (найдено {validIps.Size()} адресов)");
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
                if (!int.TryParse(part, out int num) || num < 0 || num > 255) return false;
            }
            return true;
        }
    }

    // ==========================================
    // ЗАДАЧА 12: Стек (MyStack)
    // ==========================================
    public class MyStack<T> : MyVector<T>
    {
        public MyStack() : base() { }

        // 1) push(T item) - на вершину (в конец вектора)
        public void Push(T item)
        {
            Add(item);
        }

        // 2) pop() - извлечь с вершины
        public T Pop()
        {
            if (IsEmpty()) throw new InvalidOperationException("Stack is empty");
            T obj = LastElement();
            RemoveElementAt(Size() - 1);
            return obj;
        }

        // 3) peek() - посмотреть вершину
        public T Peek()
        {
            if (IsEmpty()) throw new InvalidOperationException("Stack is empty");
            return LastElement();
        }

        // 4) empty() - проверка
        public bool Empty()
        {
            return IsEmpty();
        }

        // 5) search(T item) - "глубина" от вершины (1 - вершина)
        public int Search(T item)
        {
            int i = LastIndexOf(item);
            if (i >= 0)
            {
                return Size() - i;
            }
            return -1;
        }
    }

    // ==========================================
    // ЗАДАЧА 13: Обратная польская запись
    // ==========================================
    public class RpnCalculator
    {
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        // Определение приоритетов
        private int GetPriority(string op)
        {
            switch (op)
            {
                case "(": return 0;
                case "+": case "-": return 1;
                case "*": case "/": case "%": case "div": return 2;
                case "^": return 3;
                default: return 4; // Функции (sin, cos, min, max...)
            }
        }

        private bool IsOperator(string s)
        {
            return "+-*/^%".Contains(s) || s == "div";
        }

        private bool IsFunction(string s)
        {
            string[] funcs = { "sin", "cos", "tan", "sqrt", "abs", "sign", "ln", "lg", "exp", "min", "max", "trunc" };
            foreach (var f in funcs) if (f == s) return true;
            return false;
        }

        // Парсинг строки и перевод в RPN (Shunting-yard algorithm)
        public MyVector<string> ConvertToRpn(string expression)
        {
            MyVector<string> rpn = new MyVector<string>();
            MyStack<string> opStack = new MyStack<string>();

            // Токенайзер
            int i = 0;
            while (i < expression.Length)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // 1. Числа
                if (char.IsDigit(c) || c == '.')
                {
                    string number = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        number += expression[i];
                        i++;
                    }
                    rpn.Add(number);
                }
                // 2. Буквы (переменные или функции)
                else if (char.IsLetter(c))
                {
                    string word = "";
                    while (i < expression.Length && char.IsLetterOrDigit(expression[i]))
                    {
                        word += expression[i];
                        i++;
                    }

                    if (IsFunction(word) || word == "div") // div treated as operator in precedence, but technically word-like
                    {
                        opStack.Push(word);
                    }
                    else
                    {
                        // Переменная - сразу заменяем на значение
                        if (variables.ContainsKey(word))
                            rpn.Add(variables[word].ToString(CultureInfo.InvariantCulture));
                        else
                            throw new ArgumentException($"Неизвестная переменная: {word}");
                    }
                }
                // 3. Операторы и скобки
                else
                {
                    string op = c.ToString();
                    i++;

                    if (op == "(")
                    {
                        opStack.Push(op);
                    }
                    else if (op == ")")
                    {
                        while (!opStack.Empty() && opStack.Peek() != "(")
                        {
                            rpn.Add(opStack.Pop());
                        }
                        if (!opStack.Empty()) opStack.Pop(); // Удалить '('
                        else throw new ArgumentException("Несогласованные скобки");

                        // Если после скобки была функция (например sin(...)), переносим её
                        if (!opStack.Empty() && IsFunction(opStack.Peek()))
                        {
                            rpn.Add(opStack.Pop());
                        }
                    }
                    else // Операторы +, -, *, /...
                    {
                        // Унарный минус: если '-' в начале или после '('
                        if (op == "-" && (rpn.IsEmpty() || (!opStack.Empty() && opStack.Peek() == "(")))
                        {
                            rpn.Add("0"); // трюк: -5 -> 0 5 -
                        }

                        while (!opStack.Empty() && GetPriority(opStack.Peek()) >= GetPriority(op))
                        {
                            rpn.Add(opStack.Pop());
                        }
                        opStack.Push(op);
                    }
                }
            }

            while (!opStack.Empty())
            {
                string op = opStack.Pop();
                if (op == "(") throw new ArgumentException("Несогласованные скобки");
                rpn.Add(op);
            }

            return rpn;
        }

        public double Calculate(string expression, Dictionary<string, double> vars)
        {
            this.variables = vars;
            MyVector<string> rpn = ConvertToRpn(expression);
            MyStack<double> stack = new MyStack<double>();

            for (int i = 0; i < rpn.Size(); i++)
            {
                string token = rpn.Get(i);

                // Пробуем распарсить число
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                {
                    stack.Push(num);
                }
                else
                {
                    ExecuteOp(stack, token);
                }
            }

            if (stack.Size() != 1) throw new ArgumentException("Некорректное выражение");
            return stack.Pop();
        }

        private void ExecuteOp(MyStack<double> stack, string op)
        {
            // Сначала проверим операции, требующие 2 операнда
            // (кроме min/max, которые тоже требуют 2, но могут быть вызваны как функции)
            bool isBinary = "+-*/^%divminmax".Contains(op);
            // min/max здесь костыль для простоты проверки, они реально бинарные

            if (isBinary)
            {
                if (stack.Size() < 2) throw new ArgumentException($"Недостаточно аргументов для операции {op}");
                double b = stack.Pop();
                double a = stack.Pop();

                switch (op)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/":
                        if (Math.Abs(b) < 1e-9) throw new DivideByZeroException();
                        stack.Push(a / b);
                        break;
                    case "^": stack.Push(Math.Pow(a, b)); break;
                    case "%": stack.Push(a % b); break;
                    case "div": stack.Push(Math.Floor(a / b)); break; // Частное
                    case "min": stack.Push(Math.Min(a, b)); break;
                    case "max": stack.Push(Math.Max(a, b)); break;
                }
            }
            else
            {
                // Унарные операции
                if (stack.Empty()) throw new ArgumentException($"Недостаточно аргументов для операции {op}");
                double a = stack.Pop();

                switch (op)
                {
                    case "sqrt":
                        if (a < 0) throw new ArgumentException("Sqrt от отрицательного числа");
                        stack.Push(Math.Sqrt(a));
                        break;
                    case "abs": stack.Push(Math.Abs(a)); break;
                    case "sign": stack.Push(Math.Sign(a)); break;
                    case "sin": stack.Push(Math.Sin(a)); break;
                    case "cos": stack.Push(Math.Cos(a)); break;
                    case "tan": stack.Push(Math.Tan(a)); break;
                    case "ln":
                        if (a <= 0) throw new ArgumentException("Ln от неположительного числа");
                        stack.Push(Math.Log(a));
                        break;
                    case "lg":
                        if (a <= 0) throw new ArgumentException("Lg от неположительного числа");
                        stack.Push(Math.Log10(a));
                        break;
                    case "exp": stack.Push(Math.Exp(a)); break;
                    case "trunc": stack.Push(Math.Truncate(a)); break;
                    default: throw new ArgumentException($"Неизвестная операция: {op}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Установка культуры для правильного парсинга точек в числах (10.5 вместо 10,5)
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                Console.WriteLine("\n================ МЕНЮ ================");
                Console.WriteLine("1. MyVector (Задача 10)");
                Console.WriteLine("2. IP Filter (Задача 11)");
                Console.WriteLine("3. MyStack (Задача 12)");
                Console.WriteLine("4. Калькулятор RPN (Задача 13)");
                Console.WriteLine("0. Выход");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunTask10(); break;
                    case "2": RunTask11(); break;
                    case "3": RunTask12(); break;
                    case "4": RunTask13(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
            }
        }

        static void RunTask10()
        {
            Console.WriteLine("\n--- MyVector Test ---");
            MyVector<int> v = new MyVector<int>();
            v.Add(1); v.Add(2); v.Add(3);
            Console.WriteLine($"Vector: {v}, Size: {v.Size()}");
            v.Remove(2); // Удаляет по индексу 2 (число 3)
            Console.WriteLine($"После удаления index 2: {v}");
        }

        static void RunTask11()
        {
            Console.WriteLine("\n--- IP Processing ---");
            string f = "input.txt";
            File.WriteAllText(f, "Valid: 192.168.0.1 and 10.0.0.5.\nInvalid: 1.1.1.1.1 and 300.1.1.1\nOverlap: 123.192.168.1.1");
            Console.WriteLine($"Создан файл {f} с тестовыми данными.");
            IpVector ipv = new IpVector();
            ipv.ProcessFiles(f, "output.txt");
        }

        static void RunTask12()
        {
            Console.WriteLine("\n--- MyStack Test ---");
            MyStack<string> stack = new MyStack<string>();

            Console.WriteLine($"Empty? {stack.Empty()}");

            stack.Push("First");
            stack.Push("Second");
            stack.Push("Third");

            Console.WriteLine($"Peek: {stack.Peek()}"); // Third
            Console.WriteLine($"Search 'First': {stack.Search("First")}"); // 3
            Console.WriteLine($"Search 'Third': {stack.Search("Third")}"); // 1 (вершина)

            Console.WriteLine($"Pop: {stack.Pop()}"); // Third
            Console.WriteLine($"Pop: {stack.Pop()}"); // Second
            Console.WriteLine($"Stack size: {stack.Size()}");
        }

        static void RunTask13()
        {
            Console.WriteLine("\n--- RPN Calculator ---");
            Console.WriteLine("Поддерживаются: +, -, *, /, ^, %, div, min, max, sin, cos, tan, sqrt, abs, ln, lg, exp, trunc.");

            Console.Write("Введите выражение (например: 3 + 4 * 2 / (1 - 5) ^ 2): ");
            string expr = Console.ReadLine();

            Console.Write("Введите переменные (например: a=5 b=10 c=3) или Enter, если нет: ");
            string varsInput = Console.ReadLine();

            Dictionary<string, double> vars = new Dictionary<string, double>();
            if (!string.IsNullOrWhiteSpace(varsInput))
            {
                string[] pairs = varsInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in pairs)
                {
                    string[] kv = p.Split('=');
                    if (kv.Length == 2 && double.TryParse(kv[1], out double val))
                        vars[kv[0]] = val;
                }
            }

            try
            {
                RpnCalculator calc = new RpnCalculator();
                double result = calc.Calculate(expr, vars);
                Console.WriteLine($"Результат: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка вычисления: {ex.Message}");
            }
        }
    }
}