using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tk_Kosheleva
{
    /// <summary>
    /// Класс для решения уравнений, вынесенный отдельно для тестирования.
    /// Содержит логику парсинга коэффициентов и решения линейных/квадратных уравнений.
    /// </summary>
    public class EquationSolver
    {
        /// <summary>
        /// Класс, представляющий результат решения уравнения.
        /// Содержит не только решение, но и дополнительные метаданные для тестирования.
        /// </summary>
        public class SolutionResult
        {
            public bool IsSuccess { get; set; }           // Успешно ли выполнено вычисление
            public string Equation { get; set; }          // Строковое представление уравнения
            public string Solution { get; set; }          // Текстовое решение (для вывода пользователю)
            public string ErrorMessage { get; set; }      // Сообщение об ошибке (если есть)
            public double? Discriminant { get; set; }     // Дискриминант (для квадратных уравнений)
            public double? X1 { get; set; }               // Первый корень (если вещественный)
            public double? X2 { get; set; }               // Второй корень (если вещественный)
            public bool HasInfiniteSolutions { get; set; } // Бесконечно много решений (0·x = 0)
            public bool HasNoSolutions { get; set; }       // Нет решений (0·x = b, b ≠ 0)
            public bool HasComplexRoots { get; set; }      // Комплексные корни (D < 0)
        }

        /// <summary>
        /// Преобразование строки в число с валидацией.
        /// Допустимые форматы: целые числа, десятичные дроби с точкой, отрицательные числа.
        /// </summary>
        /// <param name="input">Входная строка от пользователя</param>
        /// <param name="coeffName">Имя коэффициента (для сообщения об ошибке)</param>
        /// <returns>Числовое значение коэффициента</returns>
        /// <exception cref="ArgumentException">Выбрасывается при пустом вводе, нечисловых символах или ошибке парсинга</exception>
        public double ParseCoefficient(string input, string coeffName)
        {
            // Удаляем лишние пробелы, обрабатываем null как пустую строку
            input = input?.Trim() ?? "";

            // Проверка на пустое поле
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"Коэффициент {coeffName} не может быть пустым");
            }

            // Регулярное выражение: опциональный минус, цифры, опциональная точка и цифры
            // Примеры: "5", "-3", "2.5", "-0.75"
            if (!Regex.IsMatch(input, @"^-?\d*\.?\d+$"))
            {
                throw new ArgumentException($"Коэффициент {coeffName} должен быть числом");
            }

            // Парсинг с инвариантной культурой (используем точку как разделитель)
            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            // Если парсинг не удался (хотя regex пропустил) – дополнительная проверка
            throw new ArgumentException($"Не удалось преобразовать коэффициент {coeffName} в число");
        }

        /// <summary>
        /// Решение линейного уравнения вида a·x + b = 0.
        /// </summary>
        /// <param name="a">Коэффициент при x</param>
        /// <param name="b">Свободный член</param>
        /// <returns>Объект SolutionResult с решением или особым случаем</returns>
        public SolutionResult SolveLinearEquation(double a, double b)
        {
            var result = new SolutionResult
            {
                Equation = $"{a}·x + {b} = 0",
                IsSuccess = true
            };

            // Используем эпсилон для сравнения чисел с плавающей точкой
            const double epsilon = 1e-10;

            if (Math.Abs(a) < epsilon)  // a == 0
            {
                if (Math.Abs(b) < epsilon)  // b == 0: 0·x = 0
                {
                    result.Solution = "x - любое число (бесконечно много решений)";
                    result.HasInfiniteSolutions = true;
                }
                else  // b ≠ 0: 0·x = b → противоречие
                {
                    result.Solution = "нет решений (противоречивое уравнение)";
                    result.HasNoSolutions = true;
                }
            }
            else  // a ≠ 0 → единственное решение x = -b/a
            {
                double x = -b / a;
                result.Solution = $"x = {x:F4}";  // Форматируем до 4 знаков после запятой
                result.X1 = x;
            }

            return result;
        }

        /// <summary>
        /// Решение квадратного уравнения вида a·x² + b·x + c = 0.
        /// Если a = 0, уравнение вырождается в линейное (вызывается SolveLinearEquation).
        /// </summary>
        /// <param name="a">Коэффициент при x²</param>
        /// <param name="b">Коэффициент при x</param>
        /// <param name="c">Свободный член</param>
        /// <returns>Объект SolutionResult с дискриминантом, корнями или комплексными корнями</returns>
        public SolutionResult SolveQuadraticEquation(double a, double b, double c)
        {
            var result = new SolutionResult
            {
                Equation = $"{a}·x² + {b}·x + {c} = 0",
                IsSuccess = true
            };

            const double epsilon = 1e-10;

            // Вырожденный случай: a = 0 → уравнение становится линейным
            if (Math.Abs(a) < epsilon)
            {
                var linearResult = SolveLinearEquation(b, c);
                result.Solution = $"Внимание: a = 0, уравнение становится линейным!\n{linearResult.Solution}";
                result.X1 = linearResult.X1;
                result.HasInfiniteSolutions = linearResult.HasInfiniteSolutions;
                result.HasNoSolutions = linearResult.HasNoSolutions;
                return result;
            }

            // Вычисление дискриминанта
            double discriminant = b * b - 4 * a * c;
            result.Discriminant = discriminant;

            // Случай 1: D > 0 → два различных вещественных корня
            if (discriminant > epsilon)
            {
                double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                result.Solution = $"Дискриминант D = {discriminant:F4}\nКорни:\nx₁ = {x1:F4}\nx₂ = {x2:F4}";
                result.X1 = x1;
                result.X2 = x2;
            }
            // Случай 2: D ≈ 0 → один кратный корень (с учетом погрешности)
            else if (Math.Abs(discriminant) < epsilon)
            {
                double x = -b / (2 * a);
                result.Solution = $"Дискриминант D = 0\nКорень (кратный): x = {x:F4}";
                result.X1 = x;
                result.X2 = x;
            }
            // Случай 3: D < 0 → комплексные корни
            else
            {
                double realPart = -b / (2 * a);
                double imaginaryPart = Math.Sqrt(-discriminant) / (2 * a);
                result.Solution = $"Дискриминант D = {discriminant:F4} < 0\nКомплексные корни:\n" +
                                 $"x₁ = {realPart:F4} + {imaginaryPart:F4}i\n" +
                                 $"x₂ = {realPart:F4} - {imaginaryPart:F4}i";
                result.HasComplexRoots = true;
            }

            return result;
        }
    }
}
