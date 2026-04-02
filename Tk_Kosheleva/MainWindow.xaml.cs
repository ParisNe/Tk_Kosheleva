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
        private EquationSolver _solver;

        public MainWindow()
        {
            InitializeComponent();
            _solver = new EquationSolver();

            // Начальная настройка
            txtC.IsEnabled = false;
        }

        private void EquationType_Changed(object sender, RoutedEventArgs e)
        {
            if (rbLinear.IsChecked == true)
            {
                txtC.IsEnabled = false;
                txtC.Text = "0";
            }
            else
            {
                txtC.IsEnabled = true;
            }

            txtResult.Text = "(не вычислено)";
            txtResult.Foreground = Brushes.Black;
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double a = _solver.ParseCoefficient(txtA.Text, "a");
                double b = _solver.ParseCoefficient(txtB.Text, "b");
                double c = _solver.ParseCoefficient(txtC.Text, "c");

                EquationSolver.SolutionResult result;

                if (rbLinear.IsChecked == true)
                {
                    result = _solver.SolveLinearEquation(a, b);
                }
                else
                {
                    result = _solver.SolveQuadraticEquation(a, b, c);
                }

                txtResult.Text = $"Уравнение: {result.Equation}\nРешение: {result.Solution}";
                txtResult.Foreground = Brushes.Black;
            }
            catch (ArgumentException ex)
            {
                txtResult.Text = $"Ошибка: {ex.Message}";
                txtResult.Foreground = Brushes.Red;
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Непредвиденная ошибка: {ex.Message}";
                txtResult.Foreground = Brushes.Red;
            }
        }
    }
}
