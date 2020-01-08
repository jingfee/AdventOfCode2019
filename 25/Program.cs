using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._25
{
    class IntCode
    {
        private List<Int64> Instructions { get; set; }
        private int Input { get; set; }
        private int Pointer { get; set; }
        private Int64 RelativeBase { get; set; }

        public IntCode(List<Int64> instructions)
        {
            Instructions = instructions;
            Pointer = 0;
            RelativeBase = 0;
        }

        public Int64? RunProgram(int? input = null)
        {
            if (input.HasValue)
            {
                Input = input.Value;
            }

            Int64 op;

            op = Instructions[Pointer];
            if (op == 99)
            {
                return null;
            }

            return DoOperation(op);
        }

        public bool RunUntilInput(int input)
        {
            Input = input;
            Int64 op;
            while (true)
            {
                op = Instructions[Pointer];
                if (op.ToString("00000").Substring(3) == "03")
                {
                    DoOperation(op);
                    return true;
                }

                DoOperation(op);
            }
        }

        public Int64? RunUntilOutput()
        {
            Int64 op;
            while (true)
            {
                op = Instructions[Pointer];
                if (op.ToString("00000").Substring(3) == "04")
                {
                    return DoOperation(op);
                }

                DoOperation(op);
            }
        }

        private Int64? DoOperation(Int64 operation)
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
                    InsertValue(Input, Pointer + 1, mode1);
                    Pointer += 2;
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

        private Int64 GetValue(Int64 value, string mode)
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

        private void InsertValue(Int64 value, int position, string mode)
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
        static List<string> items = new List<string>() {
            "antenna",
            "weather machine",
            "klein bottle",
            "spool of cat6",
            "mug",
            "cake",
            "shell",
            "tambourine"
        };

        static void Main(string[] args)
        {
            for (var i = 1; i <= 8; i++)
            {
                var permuatations = GetPermutations(items, i);
                foreach (var permutation in permuatations)
                {
                    var comp = new IntCode(GetInputFromFile());
                    PickUpAllAndGoToPortal(comp);
                    foreach (var item in items)
                    {
                        if (!permutation.Contains(item))
                        {
                            DropItem(comp, item);
                        }
                    }
                    RunInstruction(comp, new Queue<int>("east".Select(c => (int)c).Append(10)));
                    while (true)
                    {
                        var output = comp.RunUntilOutput();
                        if (output.HasValue)
                        {
                            Console.Write(Convert.ToChar(output).ToString());
                            if (output == 63)
                            {
                                break;
                            }
                            else if (output == 34)
                            {

                            }
                        }
                    }
                }
            }

        }

        static void RunInstruction(IntCode comp, Queue<int> instructions)
        {
            while (instructions.Count > 0)
            {
                var output = comp.RunUntilOutput();
                if (Convert.ToChar(output).ToString() == "?")
                {
                    int instruction = -1;
                    while (instruction != 10)
                    {
                        instruction = instructions.Dequeue();
                        comp.RunUntilInput(instruction);
                    }
                }
            }
        }

        static void PickUpAllAndGoToPortal(IntCode comp)
        {
            var instructions = new Queue<int>(GetInstructionsFromFile());
            RunInstruction(comp, instructions);
        }

        static void DropItem(IntCode comp, string item)
        {
            var command = "drop " + item;
            var instructions = new Queue<int>(command.Select(c => (int)c).Append(10));
            RunInstruction(comp, instructions);
        }

        static List<int> GetInstructionsFromFile()
        {
            var instructions = new List<int>();
            var data = File.ReadAllLines("instructions.txt");
            foreach (var line in data)
            {
                instructions.AddRange(line.ToCharArray().Select(c => (int)c));
                instructions.Add(10);
            }
            return instructions;
        }

        static List<Int64> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => Int64.Parse(d)).ToList();
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }
    }
}
