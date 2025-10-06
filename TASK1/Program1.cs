using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.Write("Введите имя файла с данными: ");
            string fileName = Console.ReadLine();

            (double[,] G, double[] x) = ReadDataFromFile(fileName);

            if (!IsSymmetricMatrix(G))
            {
                Console.WriteLine("Ошибка: Матрица метрического тензора не является симметричной!");
                return;
            }

            double length = CalculateVectorLength(G, x);
            Console.WriteLine($"Длина вектора: {length:F6}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Чтение данных из файла
    static (double[,], double[]) ReadDataFromFile(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);

        // Чтение размерности пространства
        int dimension = int.Parse(lines[0].Trim());

        // Чтение матрицы метрического тензора G
        double[,] G = new double[dimension, dimension];
        for (int i = 0; i < dimension; i++)
        {
            double[] row = lines[i + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(double.Parse)
                                      .ToArray();
            if (row.Length != dimension)
                throw new ArgumentException("Неверное количество элементов в строке матрицы");

            for (int j = 0; j < dimension; j++)
            {
                G[i, j] = row[j];
            }
        }

        // Чтение вектора x
        double[] x = lines[dimension + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(double.Parse)
                                        .ToArray();
        if (x.Length != dimension)
            throw new ArgumentException("Размерность вектора не соответствует размерности пространства");

        return (G, x);
    }

    // Проверка симметричности матрицы
    static bool IsSymmetricMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (Math.Abs(matrix[i, j] - matrix[j, i]) > 1e-10)
                    return false;
            }
        }
        return true;
    }

    // Вычисление длины вектора по формуле: √(x × G × x^T)
    static double CalculateVectorLength(double[,] G, double[] x)
    {
        int n = x.Length;

        // Вычисление x × G
        double[] xG = new double[n];
        for (int j = 0; j < n; j++)
        {
            for (int i = 0; i < n; i++)
            {
                xG[j] += x[i] * G[i, j];
            }
        }

        // Вычисление (x × G) × x^T (скалярное произведение)
        double result = 0;
        for (int i = 0; i < n; i++)
        {
            result += xG[i] * x[i];
        }

        return Math.Sqrt(result);
    }
}