using System;
using System.IO;
using System.Linq;

namespace AoC._2019._1
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();

            double sum = 0;
            foreach (var module in input)
            {
                sum += GetTotalFuelForModule(module);
            }

            Console.WriteLine(sum);
        }

        static double GetTotalFuelForModule(double mass)
        {
            double totalFuel = 0;
            var massInput = mass;
            while (true)
            {
                massInput = CalculateFuel(massInput);
                if (massInput > 0)
                {
                    totalFuel += massInput;
                }
                else
                {
                    break;
                }
            }

            return totalFuel;
        }

        static double CalculateFuel(double mass)
        {
            return (Math.Floor(mass / 3) - 2);
        }

        static double[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data.Select(d => Double.Parse(d)).ToArray();
        }
    }
}
