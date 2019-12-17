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
            var list = input.Select(i => int.Parse(i.ToString())).ToArray();
            for (var phase = 0; phase < 100; phase++)
            {
                list = GetOutputRest(list, offset);
            }
            Console.WriteLine($"{list[offset]}{list[offset + 1]}{list[offset + 2]}{list[offset + 3]}{list[offset + 4]}{list[offset + 5]}{list[offset + 6]}{list[offset + 7]}");
        }

        static int[] GetOutputRest(int[] input, int index)
        {
            var length = input.Length - index;
            var output = new int[input.Length];
            for (var i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    output[input.Length - 1 - i] = input[input.Length - 1 - i];
                }
                else
                {
                    output[input.Length - 1 - i] = (output[input.Length - i] + input[input.Length - 1 - i]) % 10;
                }
            }
            return output;
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
