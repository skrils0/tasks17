using System;

// Структура для хранения комплексных чисел
public struct ComplexNumber
{
    public double Real { get; set; }
    public double Imaginary { get; set; }

    // Конструктор
    public ComplexNumber(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }

    // Сложение
    public static ComplexNumber Add(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.Real + b.Real, a.Imaginary + b.Imaginary);
    }

    // Вычитание
    public static ComplexNumber Subtract(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.Real - b.Real, a.Imaginary - b.Imaginary);
    }

    // Умножение
    public static ComplexNumber Multiply(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(
            a.Real * b.Real - a.Imaginary * b.Imaginary,
            a.Real * b.Imaginary + a.Imaginary * b.Real
        );
    }

    // Деление
    public static ComplexNumber Divide(ComplexNumber a, ComplexNumber b)
    {
        double denominator = b.Real * b.Real + b.Imaginary * b.Imaginary;
        if (denominator == 0)
            throw new DivideByZeroException("Деление на ноль невозможно");

        return new ComplexNumber(
            (a.Real * b.Real + a.Imaginary * b.Imaginary) / denominator,
            (a.Imaginary * b.Real - a.Real * b.Imaginary) / denominator
        );
    }

    // Модуль
    public double Magnitude()
    {
        return Math.Sqrt(Real * Real + Imaginary * Imaginary);
    }

    // Аргумент (в радианах)
    public double Argument()
    {
        if (Real == 0 && Imaginary == 0)
            return 0;

        return Math.Atan2(Imaginary, Real);
    }

    // Вывод комплексного числа
    public void Print()
    {
        if (Imaginary >= 0)
            Console.WriteLine($"{Real} + {Imaginary}i");
        else
            Console.WriteLine($"{Real} - {Math.Abs(Imaginary)}i");
    }
}

class Program
{
    static void Main()
    {
        ComplexNumber currentNumber = new ComplexNumber(0, 0); // Начальное значение
        bool running = true;

        Console.WriteLine("Калькулятор комплексных чисел");
        Console.WriteLine("Текущее число: " + currentNumber);

        while (running)
        {
            PrintMenu();
            Console.Write("Выберите команду: ");
            char command = Console.ReadKey().KeyChar;
            Console.WriteLine();

            try
            {
                switch (char.ToLower(command))
                {
                    case '1': // Ввод нового числа
                        currentNumber = InputComplexNumber();
                        Console.WriteLine("Текущее число: " + currentNumber);
                        break;

                    case '2': // Сложение
                        currentNumber = ComplexNumber.Add(currentNumber, InputSecondNumber());
                        Console.WriteLine("Результат: " + currentNumber);
                        break;

                    case '3': // Вычитание
                        currentNumber = ComplexNumber.Subtract(currentNumber, InputSecondNumber());
                        Console.WriteLine("Результат: " + currentNumber);
                        break;

                    case '4': // Умножение
                        currentNumber = ComplexNumber.Multiply(currentNumber, InputSecondNumber());
                        Console.WriteLine("Результат: " + currentNumber);
                        break;

                    case '5': // Деление
                        currentNumber = ComplexNumber.Divide(currentNumber, InputSecondNumber());
                        Console.WriteLine("Результат: " + currentNumber);
                        break;

                    case '6': // Модуль
                        Console.WriteLine($"Модуль: {currentNumber.Magnitude():F4}");
                        break;

                    case '7': // Аргумент
                        Console.WriteLine($"Аргумент: {currentNumber.Argument():F4} радиан");
                        break;

                    case '8': // Вещественная часть
                        Console.WriteLine($"Вещественная часть: {currentNumber.Real}");
                        break;

                    case '9': // Мнимая часть
                        Console.WriteLine($"Мнимая часть: {currentNumber.Imaginary}");
                        break;

                    case 'p': // Вывод текущего числа
                        Console.WriteLine("Текущее число: " + currentNumber);
                        break;

                    case 'q': // Выход
                        running = false;
                        Console.WriteLine("Выход из программы...");
                        break;

                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    static void PrintMenu()
    {
        Console.WriteLine("Меню:");
        Console.WriteLine("1 - Ввод нового комплексного числа");
        Console.WriteLine("2 - Сложение");
        Console.WriteLine("3 - Вычитание");
        Console.WriteLine("4 - Умножение");
        Console.WriteLine("5 - Деление");
        Console.WriteLine("6 - Модуль");
        Console.WriteLine("7 - Аргумент");
        Console.WriteLine("8 - Вещественная часть");
        Console.WriteLine("9 - Мнимая часть");
        Console.WriteLine("P - Вывод текущего числа");
        Console.WriteLine("Q - Выход");
    }

    static ComplexNumber InputComplexNumber()
    {
        Console.Write("Введите вещественную часть: ");
        double real = double.Parse(Console.ReadLine());

        Console.Write("Введите мнимую часть: ");
        double imaginary = double.Parse(Console.ReadLine());

        return new ComplexNumber(real, imaginary);
    }

    static ComplexNumber InputSecondNumber()
    {
        Console.WriteLine("Введите второе комплексное число:");
        return InputComplexNumber();
    }

}
