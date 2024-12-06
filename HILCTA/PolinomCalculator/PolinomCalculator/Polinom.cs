using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PolinomCalculator
{
    public class Polinom
    {
        public List<double> Numbers { get; set; }


        public Polinom(params double[] args)
        {
            Numbers = new List<double>(args);
            RemoveLeadingZeros();
        }

        public Polinom(int monomialDegree, bool isMonomial = false)
        {
            if (isMonomial)
            {
                Numbers = new List<double> { 1 };
                for (int i = 0; i < monomialDegree; i++)
                {
                    Numbers.Add(0);
                }
            }
            else
            {
                Numbers = new List<double> { monomialDegree };
            }

        }


        public Polinom Clone()
        {
            return new Polinom(Numbers.ToArray());
        }


        public Polinom Derivative()
        {
            var result = Clone();
            if (result.Numbers.Count > 0)
            {
                result.Numbers.RemoveAt(result.Numbers.Count - 1);
            }

            int multiplier = result.Numbers.Count;
            for (int i = 0; i < result.Numbers.Count; i++)
            {
                result.Numbers[i] *= multiplier;
                multiplier--;
            }

            return result;
        }

        private void RemoveLeadingZeros()
        {
            while (Numbers.Count > 0 && Numbers[0] == 0)
            {
                Numbers.RemoveAt(0);
            }
        }

        public double ValueAt(double x)
        {
            double result = 0;
            for (int i = 0; i < Numbers.Count; i++)
            {
                result += Numbers[i] * Math.Pow(x, Numbers.Count - 1 - i);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is Polinom other)
            {
                RemoveLeadingZeros();
                other.RemoveLeadingZeros();
                return Numbers.SequenceEqual(other.Numbers);
            }
            return false;
        }


        public static Polinom operator +(Polinom a, Polinom b)
        {
            var aNumbers = new List<double>(a.Numbers);
            var bNumbers = new List<double>(b.Numbers);
            int maxLength = Math.Max(aNumbers.Count, bNumbers.Count);
            while (aNumbers.Count < maxLength) aNumbers.Insert(0, 0);
            while (bNumbers.Count < maxLength) bNumbers.Insert(0, 0);

            var result = new Polinom();
            for (int i = 0; i < maxLength; i++)
            {
                result.Numbers.Add(aNumbers[i] + bNumbers[i]);
            }

            result.RemoveLeadingZeros();
            return result;
        }

        public static Polinom operator -(Polinom a, Polinom b)
        {
            return a + b * -1;
        }

        public static Polinom operator *(Polinom a, double scalar)
        {
            a.RemoveLeadingZeros();
            var result = new Polinom();
            foreach (var num in a.Numbers)
            {
                result.Numbers.Add(num * scalar);
            }
            result.RemoveLeadingZeros();
            return result;
        }

        public static Polinom operator *(Polinom a, Polinom b)
        {
            var result = new Polinom();

            for (int i = 0; i < a.Numbers.Count + b.Numbers.Count - 1; i++)
            {
                result.Numbers.Add(0);
            }

            for (int i = 0; i < a.Numbers.Count; i++)
            {
                for (int j = 0; j < b.Numbers.Count; j++)
                {
                    result.Numbers[i + j] += a.Numbers[i] * b.Numbers[j];
                }
            }

            result.RemoveLeadingZeros();

            return result;
        }

        public Polinom Pow(int power)
        {
            var res = new Polinom(1);
            for (int i = 0; i < power; i++)
            {
                res *= this;
            }
            return res;
        }

        public static (Polinom Quotient, Polinom Remainder) operator /(Polinom a, Polinom b)
        {
            var quotient = new Polinom();

            for (int i = 0; i < a.Numbers.Count - b.Numbers.Count + 1; ++i)
            {
                quotient.Numbers.Add(0);
            }

            var remainder = a.Clone();

            while (true)
            {
                if (remainder.Numbers.Count < b.Numbers.Count)
                {
                    return (quotient, remainder);
                }

                double quotientValue = remainder.Numbers[0] / b.Numbers[0];
                quotient.Numbers[quotient.Numbers.Count - (remainder.Numbers.Count - b.Numbers.Count + 1)] = quotientValue;

                var tmp = new Polinom(remainder.Numbers.Count - b.Numbers.Count, true) * quotientValue;
                remainder -= b * tmp;
            }
        }

        public override string ToString()
        {
            int degree = Numbers.Count - 1;

            if (Numbers.Count == 0)
            {
                return "(0)";
            }

            var result = "";
            for (int i = 0; i < Numbers.Count; i++)
            {
                if (Numbers[i] != 0)
                {
                    if (degree > 1)
                    {
                        result += i == 0 ? $"({Numbers[i]}x^{degree})" : $"+({Numbers[i]}x^{degree})";
                    }
                    else if (degree == 1)
                    {
                        result += i == 0 ? $"({Numbers[i]}x)" : $"+({Numbers[i]}x)";
                    }
                    else
                    {
                        result += i == 0 ? $"({Numbers[i]})" : $"+({Numbers[i]})";
                    }
                }
                degree--;
            }

            return result;
        }
    }
}