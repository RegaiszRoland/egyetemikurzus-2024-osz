using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace PolinomCalculator
{
    [Serializable]
    public class PolynomialData
    {
        public string Name { get; set; }
        public double[] Coefficients { get; set; }
    }

    class Program
    {
        static void PrintCommandList()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("- help: Show this list");
            Console.WriteLine("- exit: Close application");
            Console.WriteLine("- list: Show current polynomials");
            Console.WriteLine("- p1=<polynomial expression>: Create/assign polynomial");
            Console.WriteLine("- value p1 x: Calculate polynomial value at x");
            Console.WriteLine("- save as <filename.xml>: Save polynomials to XML file");
            Console.WriteLine("- load <filename.xml>: Load polynomials from XML file");
            Console.WriteLine("Polynomial Expressions support:");
            Console.WriteLine("  - Direct coefficients: p1 = 1 2 0 1");
            Console.WriteLine("  - Copying: p1=p2");
            Console.WriteLine("  - Operations:");
            Console.WriteLine("     - Addition: p1+p2");
            Console.WriteLine("     - Subtraction: p1-p2");
            Console.WriteLine("     - Multiplication (with polynomial): p1*p2");
            Console.WriteLine("     - Multiplication (with scalar): p1*k");
            Console.WriteLine("     - Division (quotient): p1/p2");
            Console.WriteLine("     - Division (remainder): p1%p2");
            Console.WriteLine("  - Derivative: der(p1)");
            Console.WriteLine("  - Greatest Common Divisor: gcd(p1,p2)");
        }

        static void SavePolynomialsToXml(Dictionary<string, Polinom> polynomials, string filename)
        {
            var polynomialData = polynomials.Select(p => new PolynomialData
            {
                Name = p.Key,
                Coefficients = p.Value.Numbers.ToArray()
            }).ToList();

            try
            {
                var serializer = new XmlSerializer(typeof(List<PolynomialData>));
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, polynomialData);
                }
                Console.WriteLine($"Polynomials saved to {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        static void LoadPolynomialsFromXml(Dictionary<string, Polinom> polynomials, string filename)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<PolynomialData>));
                using (var reader = new StreamReader(filename))
                {
                    var polynomialData = (List<PolynomialData>)serializer.Deserialize(reader);

                    polynomials.Clear();

                    foreach (var data in polynomialData)
                    {
                        polynomials[data.Name] = new Polinom(data.Coefficients);
                    }
                    Console.WriteLine($"Polynomials loaded from {filename}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading file: {ex.Message}");
            }
        }
        static Polinom EvaluateExpression(string expression, Dictionary<string, Polinom> polynomials)
        {
            expression = Regex.Replace(expression, @"\s*([+\-*/%,()])\s*", "$1");

            if (polynomials.ContainsKey(expression))
                return polynomials[expression];

            if (expression.Split(' ').All(x => double.TryParse(x, out _)))
                return new Polinom(expression.Split(' ').Select(double.Parse).ToArray());

            if (expression.Contains("+"))
            {
                var parts = expression.Split('+');
                return EvaluateExpression(parts[0], polynomials) + EvaluateExpression(parts[1], polynomials);
            }
            if (expression.Contains("-"))
            {
                var parts = expression.Split('-');
                return EvaluateExpression(parts[0], polynomials) - EvaluateExpression(parts[1], polynomials);
            }
            if (expression.Contains("*"))
            {
                var parts = expression.Split('*');
                if (double.TryParse(parts[1], out double scalar))
                    return EvaluateExpression(parts[0], polynomials) * scalar;
                return EvaluateExpression(parts[0], polynomials) * EvaluateExpression(parts[1], polynomials);
            }
            if (expression.Contains("/"))
            {
                var parts = expression.Split('/');
                var (quotient, _) = EvaluateExpression(parts[0], polynomials) / EvaluateExpression(parts[1], polynomials);
                return quotient;
            }
            if (expression.Contains("%"))
            {
                var parts = expression.Split('%');
                var (_, remainder) = EvaluateExpression(parts[0], polynomials) / EvaluateExpression(parts[1], polynomials);
                return remainder;
            }
            if (expression.StartsWith("der(") && expression.EndsWith(")"))
            {
                var polyName = expression.Substring(4, expression.Length - 5);
                return polynomials[polyName].Derivative();
            }
            if (expression.StartsWith("gcd(") && expression.EndsWith(")"))
            {
                var parts = expression.Substring(4, expression.Length - 5).Split(',');
                return PolinomUtils.Gcd(
                    EvaluateExpression(parts[0], polynomials),
                    EvaluateExpression(parts[1], polynomials)
                );
            }

            throw new ArgumentException($"Invalid polynomial expression: {expression}");
        }

        static void Main(string[] args)
        {
            PrintCommandList();

            var polynomials = new Dictionary<string, Polinom>();

            while (true)
            {
                Console.Write(">> ");
                var input = Console.ReadLine().Trim();

                try
                {
                    if (input.StartsWith("value "))
                    {
                        var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 3)
                        {
                            Console.WriteLine("Usage: value <polynomial> <x>");
                            continue;
                        }

                        var polyName = parts[1];
                        if (!polynomials.ContainsKey(polyName))
                        {
                            Console.WriteLine($"Polynomial {polyName} not found.");
                            continue;
                        }

                        if (!double.TryParse(parts[2], out double x))
                        {
                            Console.WriteLine("Invalid x value.");
                            continue;
                        }

                        var result = polynomials[polyName].ValueAt(x);
                        Console.WriteLine(result);
                        continue;
                    }

                    else if (input.Contains("="))
                    {
                        var parts = input.Split('=');
                        var varName = parts[0].Trim();
                        var content = parts[1].Trim();

                        polynomials[varName] = EvaluateExpression(content, polynomials);
                        Console.WriteLine($"{varName} = {polynomials[varName]}");
                    }

                    else if (input.StartsWith("save as "))
                    {
                        var filename = input.Substring(8).Trim();
                        if (!filename.EndsWith(".xml"))
                        {
                            filename += ".xml";
                        }
                        SavePolynomialsToXml(polynomials, filename);
                        continue;
                    }

                    else if (input.StartsWith("load "))
                    {
                        var filename = input.Substring(5).Trim();
                        if (!filename.EndsWith(".xml"))
                        {
                            filename += ".xml";
                        }
                        LoadPolynomialsFromXml(polynomials, filename);
                        continue;
                    }

                    else if (input == "list")
                    {
                        foreach (var poly in polynomials)
                        {
                            Console.WriteLine($"{poly.Key} = {poly.Value}");
                        }
                    }
                    else if (input == "help")
                    {
                        PrintCommandList();
                    }
                    else if (input == "exit")
                    {
                        Environment.Exit(0);
                    }
                    else if (input == "")
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            var result = EvaluateExpression(input, polynomials);
                            Console.WriteLine(result);
                        }
                        catch
                        {
                            Console.WriteLine("Unknown command.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}