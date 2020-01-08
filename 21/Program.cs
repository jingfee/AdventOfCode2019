using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._21
{
    class IntCode
    {
        private List<double> Instructions { get; set; }
        private int[] Input { get; set; }
        private int InputPointer { get; set; }
        private int Pointer { get; set; }
        private double RelativeBase { get; set; }

        public IntCode(List<double> instructions, int[] input = null)
        {
            Instructions = instructions;
            Input = input;
            Pointer = 0;
            RelativeBase = 0;
        }

        public double? RunProgram(int[] input = null)
        {
            InputPointer = 0;
            if (input != null)
            {
                Input = input;
            }
            double op;
            while (true)
            {
                op = Instructions[Pointer];
                if (op == 99)
                {
                    return null;
                }

                var outputValue = DoOperation(op);
                if (outputValue.HasValue)
                {
                    return outputValue;
                }
            }
        }

        private double? DoOperation(double operation)
        {
            var opWithZeros = operation.ToString("00000");
            var mode1 = opWithZeros.Substring(2, 1);
            var mode2 = opWithZeros.Substring(1, 1);
            var mode3 = opWithZeros.Substring(0, 1);

            switch (opWithZeros.Substring(3))
            {
                case "01":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) + GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2), Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "02":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) * GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2), Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "03":
                    if (Input == null)
                    {
                        Console.WriteLine("Bug");
                        break;
                    }
                    InsertValue(Input[InputPointer], Pointer + 1, mode1);
                    Pointer += 2;
                    InputPointer = (InputPointer + 1) % Input.Length;
                    break;
                case "04":
                    var returnValue = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1);
                    Pointer += 2;
                    return returnValue;
                case "05":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) != 0 ? (int)GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
                    break;
                case "06":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) == 0 ? (int)GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
                    break;
                case "07":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) < GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) ? 1 : 0, Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "08":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) == GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) ? 1 : 0, Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "09":
                    RelativeBase += GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1);
                    Pointer += 2;
                    break;
                default:
                    Console.WriteLine("Bug");
                    break;
            }

            return null;
        }

        private double GetValue(double value, string mode)
        {
            switch (mode)
            {
                case "0":
                    return Instructions.ElementAtOrDefault((int)value);
                case "1":
                    return value;
                case "2":
                    return Instructions.ElementAtOrDefault((int)(RelativeBase + value));
                default:
                    Console.WriteLine("Bug");
                    return -1;
            }
        }

        private void InsertValue(double value, int position, string mode)
        {
            int positionValue = 0;

            switch (mode)
            {
                case "0":
                    positionValue = (int)Instructions.ElementAtOrDefault(position);
                    break;
                case "2":
                    positionValue = (int)(RelativeBase + Instructions.ElementAtOrDefault(position));
                    break;
                default:
                    Console.WriteLine("Bug");
                    return;
            }



            if (positionValue >= Instructions.Count)
            {
                var itemsToInsert = positionValue - Instructions.Count + 1;

                for (var i = 0; i < itemsToInsert; i++)
                {
                    Instructions.Add(0);
                }
            }

            Instructions[positionValue] = value;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var instructions = GetInstructionsFromFile();
            var instructionsAscii = instructions.SelectMany(i => i.Select(c => Convert.ToInt32(c)).Append(10)).ToArray();
            var droid = new IntCode(GetInputFromFile(), instructionsAscii);
            while (true)
            {
                var output = droid.RunProgram();
                if (output == null)
                {
                    break;
                }
                else
                {
                    if (output.Value > 127)
                    {
                        Console.Write(output.Value);
                    }
                    else
                    {
                        Console.Write((char)output.Value);
                    }

                }
            }
        }

        static string[] GetInstructionsFromFile()
        {
            var data = File.ReadAllLines("instructions.txt");
            return data;
        }

        static List<Double> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => double.Parse(d)).ToList();
        }
    }
}
