using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tk_Kosheleva
{
    /// <summary>
    /// Класс для решения уравнений, вынесенный отдельно для тестирования
    /// </summary>
    public class EquationSolver
    {
        /// <summary>
        /// Результат решения уравнения
        /// </summary>
        public class SolutionResult
        {
            public bool IsSuccess { get; set; }
            public string Equation { get; set; }
            public string Solution { get; set; }
            public string ErrorMessage { get; set; }
            public double? Discriminant { get; set; }
            public double? X1 { get; set; }
            public double? X2 { get; set; }
            public bool HasInfiniteSolutions { get; set; }
            public bool HasNoSolutions { get; set; }
            public bool HasComplexRoots { get; set; }
        }

        /// <summary>
        /// Преобразование строки в число с валидацией
        /// </summary>
        public double ParseCoefficient(string input, string coeffName)
        {
            input = input?.Trim() ?? "";

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"Коэффициент {coeffName} не может быть пустым");
            }

            if (!Regex.IsMatch(input, @"^-?\d*\.?\d+$"))
            {
                throw new ArgumentException($"Коэффициент {coeffName} должен быть числом");
            }

            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            throw new ArgumentException($"Не удалось преобразовать коэффициент {coeffName} в число");
        }

        /// <summary>
        /// Решение линейного уравнения
        /// </summary>
        public SolutionResult SolveLinearEquation(double a, double b)
        {
            var result = new SolutionResult
            {
                Equation = $"{a}·x + {b} = 0",
                IsSuccess = true
            };

            if (Math.Abs(a) < 1e-10)
            {
                if (Math.Abs(b) < 1e-10)
                {
                    result.Solution = "x - любое число (бесконечно много решений)";
                    result.HasInfiniteSolutions = true;
                }
                else
                {
                    result.Solution = "нет решений (противоречивое уравнение)";
                    result.HasNoSolutions = true;
                }
            }
            else
            {
                double x = -b / a;
                result.Solution = $"x = {x:F4}";
                result.X1 = x;
            }

            return result;
        }

        /// <summary>
        /// Решение квадратного уравнения
        /// </summary>
        public SolutionResult SolveQuadraticEquation(double a, double b, double c)
        {
            var result = new SolutionResult
            {
                Equation = $"{a}·x² + {b}·x + {c} = 0",
                IsSuccess = true
            };

            // Вырожденный случай - линейное уравнение
            if (Math.Abs(a) < 1e-10)
            {
                var linearResult = SolveLinearEquation(b, c);
                result.Solution = $"Внимание: a = 0, уравнение становится линейным!\n{linearResult.Solution}";
                result.X1 = linearResult.X1;
                result.HasInfiniteSolutions = linearResult.HasInfiniteSolutions;
                result.HasNoSolutions = linearResult.HasNoSolutions;
                return result;
            }

            double discriminant = b * b - 4 * a * c;
            result.Discriminant = discriminant;

            if (discriminant > 1e-10)
            {
                double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                result.Solution = $"Дискриминант D = {discriminant:F4}\nКорни:\nx₁ = {x1:F4}\nx₂ = {x2:F4}";
                result.X1 = x1;
                result.X2 = x2;
            }
            else if (Math.Abs(discriminant) < 1e-10)
            {
                double x = -b / (2 * a);
                result.Solution = $"Дискриминант D = 0\nКорень (кратный): x = {x:F4}";
                result.X1 = x;
                result.X2 = x;
            }
            else
            {
                double realPart = -b / (2 * a);
                double imaginaryPart = Math.Sqrt(-discriminant) / (2 * a);
                result.Solution = $"Дискриминант D = {discriminant:F4} < 0\nКомплексные корни:\nx₁ = {realPart:F4} + {imaginaryPart:F4}i\nx₂ = {realPart:F4} - {imaginaryPart:F4}i";
                result.HasComplexRoots = true;
            }

            return result;
        }
    }
}
