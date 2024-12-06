using HILCTA;
using NUnit.Framework;
using PolinomCalculator;
using System;
namespace PolinomCalculatorTests
{
    [TestFixture]
    public class PolinomTests
    {
        [Test]
        public void Constructor_WithParams_CreatesPolynomial()
        {
            var poly = new Polinom(1, 2, 3);
            Assert.That(poly.Numbers, Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
        }

        [Test]
        public void Constructor_Monomial_CreatesCorrectPolynomial()
        {
            var poly = new Polinom(3, true);
            Assert.That(poly.Numbers, Is.EqualTo(new[] { 1.0, 0.0, 0.0, 0.0 }));
        }

        [Test]
        public void Clone_CreatesExactCopy()
        {
            var original = new Polinom(1, 2, 3);
            var cloned = original.Clone();

            Assert.That(cloned.Numbers, Is.EqualTo(original.Numbers));
            Assert.AreNotSame(original, cloned);
        }

        [Test]
        public void Derivative_CalculatesCorrectly()
        {
            var poly = new Polinom(1, 2, 3, 4);
            var derivative = poly.Derivative();

            Assert.That(derivative.Numbers, Is.EqualTo(new[] { 3.0, 4.0, 3.0 }));
        }

        [Test]
        public void ValueAt_CalculatesCorrectValue()
        {
            var poly = new Polinom(1, 2, 1);

            Assert.AreEqual(9, poly.ValueAt(2));
            Assert.AreEqual(1, poly.ValueAt(0));
        }

        [Test]
        public void Addition_AddsPolynomials()
        {
            var poly1 = new Polinom(1, 2, 3);
            var poly2 = new Polinom(4, 5, 6);

            var result = poly1 + poly2;
            Assert.That(result.Numbers, Is.EqualTo(new[] { 5.0, 7.0, 9.0 }));
        }

        [Test]
        public void Subtraction_SubtractsPolynomials()
        {
            var poly1 = new Polinom(4, 5, 6);
            var poly2 = new Polinom(1, 2, 3);

            var result = poly1 - poly2;
            Assert.That(result.Numbers, Is.EqualTo(new[] { 3.0, 3.0, 3.0 }));
        }

        [Test]
        public void ScalarMultiplication_MultipliesPolynomial()
        {
            var poly = new Polinom(1, 2, 3);

            var result = poly * 2;
            Assert.That(result.Numbers, Is.EqualTo(new[] { 2.0, 4.0, 6.0 }));
        }

        [Test]
        public void Multiplication_MultipliesPolynomials()
        {
            var poly1 = new Polinom(1, 1);
            var poly2 = new Polinom(1, 2);

            var result = poly1 * poly2;
            Assert.That(result.Numbers, Is.EqualTo(new[] { 1.0, 3.0, 2.0 }));
        }

        [Test]
        public void Power_RaisesToPower()
        {
            var poly = new Polinom(1, 1);

            var result = poly.Pow(3);
            Assert.That(result.Numbers, Is.EqualTo(new[] { 1.0, 3.0, 3.0, 1.0 }));
        }

        [Test]
        public void Division_WithQuotientAndRemainder()
        {
            var dividend = new Polinom(1, 2, 5);
            var divisor = new Polinom(1, 1);

            var (quotient, remainder) = dividend / divisor;

            Assert.That(quotient.Numbers, Is.EqualTo(new[] { 1.0, 1.0 }));
            Assert.That(remainder.Numbers, Is.EqualTo(new[] {4.0}));
        }

        [Test]
        public void Gcd_FindsGreatestCommonDivisor()
        {
            var poly1 = new Polinom(1, 2, 1); 
            var poly2 = new Polinom(1, 1);    

            var gcd = PolinomUtils.Gcd(poly1, poly2);

            Assert.That(gcd.Numbers, Is.EqualTo(new[] { 1.0, 1.0 })); 
        }

        [Test]
        public void ToString_FormatsPolynomialCorrectly()
        {
            var poly = new Polinom(1, 2, 3);
            Assert.AreEqual("(1x^2)+(2x)+(3)", poly.ToString());
        }

        [Test]
        public void RemoveLeadingZeros_RemovesInitialZeros()
        {
            var poly = new Polinom(0, 0, 1, 2, 3);
            Assert.That(poly.Numbers, Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
        }

        [Test]
        public void Equals_ComparesPolynomials()
        {
            var poly1 = new Polinom(1, 2, 3);
            var poly2 = new Polinom(1, 2, 3);
            var poly3 = new Polinom(1, 2, 4);

            Assert.AreEqual(poly1, poly2);
            Assert.AreNotEqual(poly1, poly3);
        }
    }
}