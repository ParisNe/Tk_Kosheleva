using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tk_Kosheleva
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик изменения типа уравнения (линейное/квадратное)
        /// При переключении на квадратное уравнение поле c становится доступным
        /// </summary>
        private void EquationType_Changed(object sender, RoutedEventArgs e)
        {
            // Если выбрано линейное уравнение, поле c неактивно
            if (rbLinear.IsChecked == true)
            {
                txtC.IsEnabled = false;
                txtC.Text = "0";
            }
            // Если выбрано квадратное уравнение, поле c активно
            else
            {
                txtC.IsEnabled = true;
            }

            // Очищаем предыдущий результат при смене типа уравнения
            txtResult.Text = "(не вычислено)";
        }

        /// <summary>
        /// Основной обработчик вычисления уравнения
        /// Проверяет ввод, определяет тип уравнения и вызывает соответствующий метод решения
        /// </summary>
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение и проверка коэффициентов
                double a = ParseCoefficient(txtA.Text, "a");
                double b = ParseCoefficient(txtB.Text, "b");
                double c = ParseCoefficient(txtC.Text, "c");

                // Определение типа уравнения и вызов соответствующего метода решения
                if (rbLinear.IsChecked == true)
                {
                    SolveLinearEquation(a, b);
                }
                else
                {
                    SolveQuadraticEquation(a, b, c);
                }
            }
            catch (ArgumentException ex)
            {
                // Отображение ошибок валидации в поле результата
                txtResult.Text = $"Ошибка: {ex.Message}";
                txtResult.Foreground = System.Windows.Media.Brushes.Red;
            }
            catch (Exception ex)
            {
                // Обработка непредвиденных ошибок
                txtResult.Text = $"Непредвиденная ошибка: {ex.Message}";
                txtResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        /// <summary>
        /// Преобразование строки в число с валидацией
        /// </summary>
        private double ParseCoefficient(string input, string coeffName)
        {
            // Удаляем пробелы и проверяем на пустую строку
            input = input.Trim();
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"Коэффициент {coeffName} не может быть пустым");
            }

            // Проверка на корректный числовой формат (разрешаем целые и дробные числа с точкой)
            if (!Regex.IsMatch(input, @"^-?\d*\.?\d+$"))
            {
                throw new ArgumentException($"Коэффициент {coeffName} должен быть числом");
            }

            // Парсинг числа с учетом десятичного разделителя
            if (double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            throw new ArgumentException($"Не удалось преобразовать коэффициент {coeffName} в число");
        }

        /// <summary>
        /// Решение линейного уравнения вида a·x + b = 0
        /// </summary>
        private void SolveLinearEquation(double a, double b)
        {
            string equation = $"Уравнение: {a}·x + {b} = 0";

            if (a == 0)
            {
                if (b == 0)
                {
                    txtResult.Text = $"{equation}\nРешение: x - любое число (бесконечно много решений)";
                }
                else
                {
                    txtResult.Text = $"{equation}\nРешение: нет решений (противоречивое уравнение)";
                }
            }
            else
            {
                double x = -b / a;
                txtResult.Text = $"{equation}\nРешение: x = {x:F4}";
            }
            txtResult.Foreground = System.Windows.Media.Brushes.Black;
        }

        /// <summary>
        /// Решение квадратного уравнения вида a·x² + b·x + c = 0
        /// </summary>
        private void SolveQuadraticEquation(double a, double b, double c)
        {
            string equation = $"Уравнение: {a}·x² + {b}·x + {c} = 0";

            // Если a = 0, уравнение вырождается в линейное
            if (a == 0)
            {
                txtResult.Text = $"{equation}\nВнимание: a = 0, уравнение становится линейным!";
                SolveLinearEquation(b, c);
                return;
            }

            // Вычисление дискриминанта
            double discriminant = b * b - 4 * a * c;

            if (discriminant > 0)
            {
                // Два корня
                double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                txtResult.Text = $"{equation}\nДискриминант D = {discriminant:F4}\n" +
                                $"Корни:\nx₁ = {x1:F4}\nx₂ = {x2:F4}";
            }
            else if (Math.Abs(discriminant) < 1e-10) // Погрешность для сравнения с нулем
            {
                // Один корень (кратный)
                double x = -b / (2 * a);
                txtResult.Text = $"{equation}\nДискриминант D = 0\n" +
                                $"Корень (кратный): x = {x:F4}";
            }
            else
            {
                // Комплексные корни
                double realPart = -b / (2 * a);
                double imaginaryPart = Math.Sqrt(-discriminant) / (2 * a);
                txtResult.Text = $"{equation}\nДискриминант D = {discriminant:F4} < 0\n" +
                                $"Комплексные корни:\nx₁ = {realPart:F4} + {imaginaryPart:F4}i\n" +
                                $"x₂ = {realPart:F4} - {imaginaryPart:F4}i";
            }
            txtResult.Foreground = System.Windows.Media.Brushes.Black;
        }
    }
}
