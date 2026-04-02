using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            UpdateCFieldVisibility(); // При старте скрываем поле c (по умолчанию линейное)
        }

        // Обработчик переключения типа уравнения
        private void EquationType_Changed(object sender, RoutedEventArgs e)
        {
            UpdateCFieldVisibility();
        }

        // Показываем или скрываем поле c в зависимости от выбора
        private void UpdateCFieldVisibility()
        {
            if (rbQuadratic.IsChecked == true)
            {
                txtC.IsEnabled = true;
                txtC.Background = System.Windows.Media.Brushes.White;
            }
            else
            {
                txtC.IsEnabled = false;
                txtC.Background = System.Windows.Media.Brushes.LightGray;
            }
        }

        // Кнопка "Вычислить"
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbLinear.IsChecked == true)
                {
                    SolveLinear();
                }
                else if (rbQuadratic.IsChecked == true)
                {
                    SolveQuadratic();
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void SolveLinear()
        {
            // a·x + b = 0
            double a = GetDouble(txtA.Text);
            double b = GetDouble(txtB.Text);

            if (a == 0)
            {
                if (b == 0)
                    txtResult.Text = "Уравнение имеет бесконечно много решений (0·x = 0)";
                else
                    txtResult.Text = "Уравнение не имеет решений (0·x ≠ 0)";
            }
            else
            {
                double x = -b / a;
                txtResult.Text = $"x = {x:F4}\n(Уравнение: {a}·x + {b} = 0)";
            }
        }

        private void SolveQuadratic()
        {
            // a·x² + b·x + c = 0
            double a = GetDouble(txtA.Text);
            double b = GetDouble(txtB.Text);
            double c = GetDouble(txtC.Text);

            if (a == 0)
            {
                // Вырождается в линейное
                txtResult.Text = "a = 0, уравнение становится линейным.\n";
                double x = -b / a; // бессмысленно, но пересчитаем через линейное
                if (b == 0)
                    txtResult.Text += "Нет решений (0·x + c = 0, c ≠ 0)";
                else
                    txtResult.Text += $"x = {-c / b:F4}";
                return;
            }

            double discriminant = b * b - 4 * a * c;
            txtResult.Text = $"Дискриминант D = {discriminant:F4}\n";

            if (discriminant > 0)
            {
                double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
                txtResult.Text += $"Два действительных корня:\nx₁ = {x1:F4}\nx₂ = {x2:F4}";
            }
            else if (Math.Abs(discriminant) < 1e-10) // D ≈ 0
            {
                double x = -b / (2 * a);
                txtResult.Text += $"Один корень (два совпадающих):\nx = {x:F4}";
            }
            else
            {
                double realPart = -b / (2 * a);
                double imagPart = Math.Sqrt(-discriminant) / (2 * a);
                txtResult.Text += $"Комплексные корни:\nx₁ = {realPart:F4} + {imagPart:F4}·i\nx₂ = {realPart:F4} - {imagPart:F4}·i";
            }
        }

        private double GetDouble(string text)
        {
            text = text.Trim().Replace('.', ','); // поддержка и . и ,
            if (double.TryParse(text, out double result))
                return result;
            throw new Exception($"Некорректное число: '{text}'");
        }
    }
}
