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
    /// Главное окно приложения для решения линейных и квадратных уравнений
    /// </summary>
    public partial class MainWindow : Window
    {
        // Экземпляр класса-решателя, вынесенного для тестирования
        private EquationSolver _solver;

        /// <summary>
        /// Конструктор окна. Инициализирует компоненты XAML и настраивает начальное состояние.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _solver = new EquationSolver();

            // При старте приложения выбирается линейное уравнение,
            // поэтому поле для коэффициента c должно быть заблокировано.
            txtC.IsEnabled = false;
        }

        /// <summary>
        /// Обработчик изменения типа уравнения (линейное/квадратное).
        /// Вызывается при клике на радиокнопки rbLinear или rbQuadratic.
        /// </summary>
        /// <param name="sender">Радиокнопка-источник события</param>
        /// <param name="e">Параметры события</param>
        private void EquationType_Changed(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрано ли линейное уравнение
            if (rbLinear.IsChecked == true)
            {
                // Для линейного уравнения коэффициент c не используется:
                // блокируем поле ввода и принудительно устанавливаем значение 0
                txtC.IsEnabled = false;
                txtC.Text = "0";
            }
            else
            {
                // Для квадратного уравнения коэффициент c нужен:
                // разблокируем поле для ввода
                txtC.IsEnabled = true;
            }

            // При смене типа уравнения сбрасываем предыдущий результат,
            // чтобы избежать путаницы у пользователя
            txtResult.Text = "(не вычислено)";
            txtResult.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Вычислить".
        /// Считывает коэффициенты, определяет тип уравнения и вызывает соответствующий метод решателя.
        /// В случае ошибки выводит сообщение красным цветом.
        /// </summary>
        /// <param name="sender">Кнопка btnCalculate</param>
        /// <param name="e">Параметры события</param>
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Парсинг коэффициентов из текстовых полей
                // Метод ParseCoefficient выбрасывает ArgumentException при ошибке
                double a = _solver.ParseCoefficient(txtA.Text, "a");
                double b = _solver.ParseCoefficient(txtB.Text, "b");
                double c = _solver.ParseCoefficient(txtC.Text, "c");

                EquationSolver.SolutionResult result;

                // Выбор метода решения в зависимости от типа уравнения
                if (rbLinear.IsChecked == true)
                {
                    // Линейное уравнение: a·x + b = 0
                    result = _solver.SolveLinearEquation(a, b);
                }
                else
                {
                    // Квадратное уравнение: a·x² + b·x + c = 0
                    result = _solver.SolveQuadraticEquation(a, b, c);
                }

                // Вывод результата в текстовый блок
                txtResult.Text = $"Уравнение: {result.Equation}\nРешение: {result.Solution}";
                txtResult.Foreground = Brushes.Black;
            }
            catch (ArgumentException ex)
            {
                // Ошибка валидации ввода (не число, пустое поле и т.п.)
                txtResult.Text = $"Ошибка: {ex.Message}";
                txtResult.Foreground = Brushes.Red;
            }
            catch (Exception ex)
            {
                // Любая другая непредвиденная ошибка
                txtResult.Text = $"Непредвиденная ошибка: {ex.Message}";
                txtResult.Foreground = Brushes.Red;
            }
        }
    }
}
