using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tk_Kosheleva;

namespace Tk_Test
{
    [TestClass]
    public class EquationSolverTests
    {
        private EquationSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new EquationSolver();
        }

        #region Тесты парсинга коэффициентов

        [TestMethod]
        public void ParseCoefficient_ValidInteger_ReturnsCorrectValue()
        {
            // Arrange
            string input = "5";

            // Act
            double result = _solver.ParseCoefficient(input, "a");

            // Assert
            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void ParseCoefficient_ValidDouble_ReturnsCorrectValue()
        {
            // Arrange
            string input = "3.14";

            // Act
            double result = _solver.ParseCoefficient(input, "a");

            // Assert
            Assert.AreEqual(3.14, result, 0.0001);
        }

        [TestMethod]
        public void ParseCoefficient_NegativeNumber_ReturnsCorrectValue()
        {
            // Arrange
            string input = "-7";

            // Act
            double result = _solver.ParseCoefficient(input, "a");

            // Assert
            Assert.AreEqual(-7.0, result);
        }

        [TestMethod]
        public void ParseCoefficient_EmptyString_ThrowsArgumentException()
        {
            // Arrange
            string input = "";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                _solver.ParseCoefficient(input, "a"));
        }

        [TestMethod]
        public void ParseCoefficient_InvalidString_ThrowsArgumentException()
        {
            // Arrange
            string input = "abc";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                _solver.ParseCoefficient(input, "a"));
        }

        [TestMethod]
        public void ParseCoefficient_NullInput_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                _solver.ParseCoefficient(null, "a"));
        }

        [TestMethod]
        public void ParseCoefficient_Zero_ReturnsZero()
        {
            // Act
            double result = _solver.ParseCoefficient("0", "a");

            // Assert
            Assert.AreEqual(0.0, result);
        }

        [TestMethod]
        public void ParseCoefficient_WithSpaces_ReturnsCorrectValue()
        {
            // Arrange
            string input = "  42  ";

            // Act
            double result = _solver.ParseCoefficient(input, "a");

            // Assert
            Assert.AreEqual(42.0, result);
        }

        [TestMethod]
        public void ParseCoefficient_NegativeFraction_ReturnsCorrectValue()
        {
            // Arrange
            string input = "-2.5";

            // Act
            double result = _solver.ParseCoefficient(input, "a");

            // Assert
            Assert.AreEqual(-2.5, result, 0.0001);
        }

        #endregion

        #region Тесты линейных уравнений

        [TestMethod]
        public void SolveLinearEquation_StandardEquation_ReturnsCorrectRoot()
        {
            // Arrange: 2x + 4 = 0 => x = -2
            double a = 2;
            double b = 4;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(-2.0, result.X1.Value, 0.0001);
            Assert.IsFalse(result.HasInfiniteSolutions);
            Assert.IsFalse(result.HasNoSolutions);
        }

        [TestMethod]
        public void SolveLinearEquation_AisZero_BisZero_ReturnsInfiniteSolutions()
        {
            // Arrange
            double a = 0;
            double b = 0;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsTrue(result.HasInfiniteSolutions);
            Assert.IsFalse(result.HasNoSolutions);
            Assert.IsNull(result.X1);
        }

        [TestMethod]
        public void SolveLinearEquation_AisZero_BnotZero_ReturnsNoSolution()
        {
            // Arrange
            double a = 0;
            double b = 5;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsTrue(result.HasNoSolutions);
            Assert.IsFalse(result.HasInfiniteSolutions);
            Assert.IsNull(result.X1);
        }

        [TestMethod]
        public void SolveLinearEquation_NegativeCoefficients_ReturnsCorrectRoot()
        {
            // Arrange: -3x + 9 = 0 => x = 3
            double a = -3;
            double b = 9;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(3.0, result.X1.Value, 0.0001);
        }

        [TestMethod]
        public void SolveLinearEquation_FractionalCoefficients_ReturnsCorrectRoot()
        {
            // Arrange: 0.5x + 2 = 0 => x = -4
            double a = 0.5;
            double b = 2;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(-4.0, result.X1.Value, 0.0001);
        }

        [TestMethod]
        public void SolveLinearEquation_WithNegativeB_ReturnsCorrectRoot()
        {
            // Arrange: 3x - 6 = 0 => x = 2
            double a = 3;
            double b = -6;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(2.0, result.X1.Value, 0.0001);
        }

        #endregion

        #region Тесты квадратных уравнений

        [TestMethod]
        public void SolveQuadraticEquation_TwoRealRoots_ReturnsCorrectRoots()
        {
            // Arrange: x² - 5x + 6 = 0, корни: 2 и 3
            double a = 1;
            double b = -5;
            double c = 6;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Discriminant);
            Assert.AreEqual(1.0, result.Discriminant.Value, 0.0001);
            Assert.IsNotNull(result.X1);
            Assert.IsNotNull(result.X2);
            Assert.AreEqual(3.0, result.X1.Value, 0.0001);
            Assert.AreEqual(2.0, result.X2.Value, 0.0001);
            Assert.IsFalse(result.HasComplexRoots);
        }

        [TestMethod]
        public void SolveQuadraticEquation_DoubleRoot_ReturnsSingleRoot()
        {
            // Arrange: x² - 4x + 4 = 0, корень: 2
            double a = 1;
            double b = -4;
            double c = 4;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Discriminant);
            Assert.AreEqual(0.0, result.Discriminant.Value, 0.0001);
            Assert.IsNotNull(result.X1);
            Assert.IsNotNull(result.X2);
            Assert.AreEqual(2.0, result.X1.Value, 0.0001);
            Assert.AreEqual(2.0, result.X2.Value, 0.0001);
        }

        [TestMethod]
        public void SolveQuadraticEquation_ComplexRoots_ReturnsComplexResult()
        {
            // Arrange: x² + x + 1 = 0
            double a = 1;
            double b = 1;
            double c = 1;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Discriminant);
            Assert.IsTrue(result.Discriminant.Value < 0);
            Assert.IsTrue(result.HasComplexRoots);
            Assert.IsNull(result.X1);
            Assert.IsNull(result.X2);
        }

        [TestMethod]
        public void SolveQuadraticEquation_AisZero_FallsBackToLinear()
        {
            // Arrange: 0x² + 2x + 4 = 0 => 2x + 4 = 0 => x = -2
            double a = 0;
            double b = 2;
            double c = 4;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(-2.0, result.X1.Value, 0.0001);
        }

        [TestMethod]
        public void SolveQuadraticEquation_AllZeros_ReturnsInfiniteSolutions()
        {
            // Arrange
            double a = 0;
            double b = 0;
            double c = 0;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsTrue(result.HasInfiniteSolutions);
        }

        [TestMethod]
        public void SolveQuadraticEquation_NegativeDiscriminant_NoRealRoots()
        {
            // Arrange: 2x² + 4x + 5 = 0, D = 16 - 40 = -24
            double a = 2;
            double b = 4;
            double c = 5;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.Discriminant);
            Assert.IsTrue(result.Discriminant.Value < 0);
            Assert.IsTrue(result.HasComplexRoots);
        }

        [TestMethod]
        public void SolveQuadraticEquation_LargeCoefficients_ReturnsCorrectRoots()
        {
            // Arrange: 100x² + 500x + 600 = 0
            double a = 100;
            double b = 500;
            double c = 600;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.Discriminant);
            Assert.IsTrue(result.Discriminant.Value > 0);
            Assert.IsNotNull(result.X1);
            Assert.IsNotNull(result.X2);
        }

        [TestMethod]
        public void SolveQuadraticEquation_WithNegativeA_ReturnsCorrectRoots()
        {
            // Arrange: -x² + 5x - 6 = 0 => x² - 5x + 6 = 0 => корни: 2, 3
            double a = -1;
            double b = 5;
            double c = -6;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.IsNotNull(result.X2);
            Assert.AreEqual(2.0, result.X1.Value, 0.0001);
            Assert.AreEqual(3.0, result.X2.Value, 0.0001);
        }

        #endregion

        #region Пограничные тесты

        [TestMethod]
        public void SolveLinearEquation_VerySmallA_HandlesPrecision()
        {
            // Arrange
            double a = 1e-15;
            double b = 2;

            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsNotNull(result.X1);
            double expected = -2e15;
            Assert.AreEqual(expected, result.X1.Value, 1e10);
        }

        [TestMethod]
        public void SolveQuadraticEquation_VerySmallA_FallsBackToLinear()
        {
            // Arrange
            double a = 1e-15;
            double b = 3;
            double c = 6;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(-2.0, result.X1.Value, 0.0001);
        }

        [TestMethod]
        public void ParseCoefficient_ScientificNotation_ThrowsArgumentException()
        {
            // Arrange
            string input = "1e-5";

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                _solver.ParseCoefficient(input, "a"));
        }

        [TestMethod]
        public void SolveQuadraticEquation_DiscriminantZero_WithPrecision()
        {
            // Arrange: D очень близок к нулю
            double a = 1;
            double b = -2;
            double c = 1.0000000001;

            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.Discriminant);
            Assert.IsTrue(Math.Abs(result.Discriminant.Value) < 0.0001);
        }

        #endregion

        #region Комбинаторные тесты с DataRow

        [TestMethod]
        [DataRow(1, -3, 2, 2, 1)]    // x² - 3x + 2 = 0, корни 2 и 1
        [DataRow(2, -7, 3, 3, 0.5)]  // 2x² - 7x + 3 = 0, корни 3 и 0.5
        [DataRow(1, -4, 4, 2, 2)]    // x² - 4x + 4 = 0, корень 2
        [DataRow(1, -5, 6, 3, 2)]    // x² - 5x + 6 = 0, корни 3 и 2
        [DataRow(1, 5, 6, -2, -3)]   // x² + 5x + 6 = 0, корни -2 и -3
        public void SolveQuadraticEquation_MultipleTestCases_ReturnsCorrectRoots(
            double a, double b, double c, double expectedX1, double expectedX2)
        {
            // Act
            var result = _solver.SolveQuadraticEquation(a, b, c);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.IsNotNull(result.X2);
            Assert.AreEqual(expectedX1, result.X1.Value, 0.0001);
            Assert.AreEqual(expectedX2, result.X2.Value, 0.0001);
        }

        [TestMethod]
        [DataRow(2, 4, -2)]     // 2x + 4 = 0 => x = -2
        [DataRow(3, -6, 2)]     // 3x - 6 = 0 => x = 2
        [DataRow(-2, 8, 4)]     // -2x + 8 = 0 => x = 4
        [DataRow(5, 0, 0)]      // 5x + 0 = 0 => x = 0
        public void SolveLinearEquation_MultipleTestCases_ReturnsCorrectRoots(
            double a, double b, double expectedX)
        {
            // Act
            var result = _solver.SolveLinearEquation(a, b);

            // Assert
            Assert.IsNotNull(result.X1);
            Assert.AreEqual(expectedX, result.X1.Value, 0.0001);
        }

        #endregion

        #region Тесты валидации строк

        [TestMethod]
        [DataRow("123")]
        [DataRow("0")]
        [DataRow("-456")]
        [DataRow("3.14")]
        [DataRow("-2.5")]
        [DataRow("0.0")]
        public void ParseCoefficient_ValidFormats_ReturnsSuccess(string input)
        {
            // Act
            double result = _solver.ParseCoefficient(input, "test");

            // Assert
            Assert.IsTrue(double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double expected));
            Assert.AreEqual(expected, result, 0.0001);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("abc")]
        [DataRow("1,5")]
        [DataRow("1.2.3")]
        [DataRow("a1b2")]
        public void ParseCoefficient_InvalidFormats_ThrowsException(string input)
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                _solver.ParseCoefficient(input, "test"));
        }

        #endregion
    }
}
