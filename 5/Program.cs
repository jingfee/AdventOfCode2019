using System;
using System.IO;
using System.Linq;

namespace AoC._2019._5
{
    class Program
    {
        static int InputNumber = 5;

        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            RunProgram(input);
        }

        static void RunProgram(int[] input)
        {
            var pointer = 0;
            int op;
            while (true)
            {
                op = input[pointer];
                if (op == 99)
                {
                    break;
                }

                pointer = DoOperation(op, input, pointer);
            }
        }

        static int DoOperation(int operation, int[] input, int pointer)
        {
            var opWithZeros = operation.ToString("00000");
            var mode1 = opWithZeros.Substring(2, 1);
            var mode2 = opWithZeros.Substring(1, 1);

            switch (opWithZeros.Substring(3))
            {
                case "01":
                    input[input[pointer + 3]] = GetValue(input, input[pointer + 1], mode1) + GetValue(input, input[pointer + 2], mode2);
                    return pointer + 4;
                case "02":
                    input[input[pointer + 3]] = GetValue(input, input[pointer + 1], mode1) * GetValue(input, input[pointer + 2], mode2);
                    return pointer + 4;
                case "03":
                    input[input[pointer + 1]] = InputNumber;
                    return pointer + 2;
                case "04":
                    Console.WriteLine(GetValue(input, input[pointer + 1], mode1));
                    return pointer + 2;
                case "05":
                    return GetValue(input, input[pointer + 1], mode1) != 0 ? GetValue(input, input[pointer + 2], mode2) : pointer + 3;
                case "06":
                    return GetValue(input, input[pointer + 1], mode1) == 0 ? GetValue(input, input[pointer + 2], mode2) : pointer + 3;
                case "07":
                    input[input[pointer + 3]] = GetValue(input, input[pointer + 1], mode1) < GetValue(input, input[pointer + 2], mode2) ? 1 : 0;
                    return pointer + 4;
                case "08":
                    input[input[pointer + 3]] = GetValue(input, input[pointer + 1], mode1) == GetValue(input, input[pointer + 2], mode2) ? 1 : 0;
                    return pointer + 4;
                default:
                    Console.WriteLine("Bug");
                    return 0;
            }
        }

        static int GetValue(int[] input, int value, string mode)
        {
            return mode == "0" ? input[value] : value;
        }

        static int[] GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => Int32.Parse(d)).ToArray();
        }
    }
}
