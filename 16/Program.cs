using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC._2019._16
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var inputSB = new StringBuilder();
            for (var i = 0; i < 10000; i++)
            {
                inputSB.Append(input);
            }
            input = inputSB.ToString();
            var offset = int.Parse(input.Substring(0, 7));
            var list = input.Select(i => int.Parse(i.ToString())).ToList();
            /*for (var phase = 0; phase < 100; phase++)
            {
                list = DoFFT(list);
            }
            Console.WriteLine(input.Substring(0, offset));*/
            Console.WriteLine(GetNumberAt(list, 1, offset));
        }

        static int GetNumberAt(List<int> input, int phases, int index)
        {
            if (phases == 0 || index == input.Count - 1)
            {
                return input[index];
            }
            else
            {
                return (GetNumberAt(input, phases, index + 1) + GetNumberAt(input, phases - 1, index)) % 10;
            }
        }

        static List<double> DoFFT(List<double> input)
        {
            var output = new List<double>();
            for (var i = 0; i < input.Count; i++)
            {
                var j = i;
                double rowSum = 0;
                while (j < input.Count)
                {
                    for (var r = 0; r < i + 1 && j + r < input.Count; r++)
                    {
                        rowSum += input[j + r];
                    }
                    j += i + 1;
                    for (var r = 0; r < i + 1 && j + r < input.Count; r++)
                    {
                        rowSum -= input[j + r];
                    }
                    j += i + 1;
                }
                output.Add(Math.Abs(rowSum % 10));
            }
            return output;
        }

        static string GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data;
        }
    }
}
