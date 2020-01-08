using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._19
{
    class IntCode
    {
        private List<int> Instructions { get; set; }
        private int[] Input { get; set; }
        private int InputCounter { get; set; }
        private int Pointer { get; set; }
        private int RelativeBase { get; set; }

        public IntCode(List<int> instructions, int[] input = null)
        {
            Instructions = instructions.ToList();
            Input = input;
            Pointer = 0;
            RelativeBase = 0;
        }

        public int? RunProgram(int[] input = null)
        {
            InputCounter = 0;
            if (input != null)
            {
                Input = input;
            }
            int op;
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

        private int? DoOperation(int operation)
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
                    if (Input.Length == 0)
                    {
                        Console.WriteLine("Bug");
                        break;
                    }
                    var input = Input[InputCounter % Input.Length];
                    InputCounter++;

                    InsertValue(input, Pointer + 1, mode1);
                    Pointer += 2;
                    break;
                case "04":
                    var returnValue = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1);
                    Pointer += 2;
                    return returnValue;
                case "05":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) != 0 ? GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
                    break;
                case "06":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) == 0 ? GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
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

        private int GetValue(int value, string mode)
        {
            switch (mode)
            {
                case "0":
                    return Instructions.ElementAtOrDefault(value);
                case "1":
                    return value;
                case "2":
                    return Instructions.ElementAtOrDefault(RelativeBase + value);
                default:
                    Console.WriteLine("Bug");
                    return -1;
            }
        }

        private void InsertValue(int value, int position, string mode)
        {
            int positionValue = 0;

            switch (mode)
            {
                case "0":
                    positionValue = (int)Instructions.ElementAtOrDefault(position);
                    break;
                case "2":
                    positionValue = RelativeBase + (int)Instructions.ElementAtOrDefault(position);
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
            //var beamCounter = 0;
            for (var y = 1660; y < 1780; y++)
            {
                for (var x = 1020; x < 1170; x++)
                {
                    var computer = new IntCode(GetInputFromFile());

                    var output = computer.RunProgram(new int[] { x, y });
                    if (output == 1)
                    {
                        if (x >= 1031 && x < 1131 && y >= 1666 && y < 1766)
                        {
                            Console.Write("O");
                        }
                        else
                        {
                            Console.Write("#");
                        }

                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            //Console.WriteLine(beamCounter);
            var drones = GetInputFromFile();
            var shipSize = 100;
            var i = 0;
            var xOffset = 0;
            while (true)
            {
                var offsetDiff = 0;
                while (true)
                {
                    var xToTry = i - xOffset - offsetDiff;
                    if (xToTry >= 0)
                    {
                        if (IsInBeam(drones, xToTry, i))
                        {
                            xOffset += offsetDiff;
                            break;
                        }
                        offsetDiff++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (i > shipSize - 1 && IsInBeam(drones, i - xOffset - (shipSize - 1), i) && IsInBeam(drones, i - xOffset - (shipSize - 1), i + (shipSize - 1)))
                {
                    PrintAnswer(i - xOffset - (shipSize - 1), i);
                    break;
                }

                i++;
            }
        }

        static void PrintAnswer(int x, int y)
        {
            Console.WriteLine(x * 10000 + y);
        }

        static bool IsInBeam(List<int> drones, int x, int y)
        {
            return new IntCode(drones).RunProgram(new int[] { x, y }).Value == 1;
        }

        static List<int> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => int.Parse(d)).ToList();
        }
    }
}
