using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolinomCalculator
{
    public class PolinomUtils
    {
        public static Polinom Gcd(Polinom a, Polinom b)
        {
            if (a.Numbers.Count < b.Numbers.Count)
            {
                return Gcd(b, a);
            }

            var f = a;
            var g = b;

            while (true)
            {
                var (_, remainder) = f / g;
                f = g;
                g = remainder;

                if (remainder.Numbers.Count == 0)
                {
                    return f;
                }
            }
        }
    }
}
