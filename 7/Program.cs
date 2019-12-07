using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._7
{
    class IntCode
    {
        public string name { get; set; }
        public int[] instructions { get; set; }
        public int phaseSetting { get; set; }
        private int inputFromAmp { get; set; }
        private int pointer { get; set; }

        public IntCode(int[] instructions, int firstInput, string name)
        {
            this.instructions = instructions;
            this.phaseSetting = firstInput;
            this.pointer = 0;
            this.name = name;
        }

        public int? RunProgram(int inputFromAmp)
        {
            this.inputFromAmp = inputFromAmp;
            int op;
            while (true)
            {
                op = instructions[pointer];
                if (op == 99)
                {
                    return null;
                }

                var output = DoOperation(op);

                if (output.HasValue)
                {
                    return output.Value;
                }
            }
        }

        private int? DoOperation(int operation)
        {
            var opWithZeros = operation.ToString("00000");
            var mode1 = opWithZeros.Substring(2, 1);
            var mode2 = opWithZeros.Substring(1, 1);

            switch (opWithZeros.Substring(3))
            {
                case "01":
                    instructions[instructions[pointer + 3]] = GetValue(instructions[pointer + 1], mode1) + GetValue(instructions[pointer + 2], mode2);
                    pointer += 4;
                    break;
                case "02":
                    instructions[instructions[pointer + 3]] = GetValue(instructions[pointer + 1], mode1) * GetValue(instructions[pointer + 2], mode2);
                    pointer += 4;
                    break;
                case "03":
                    instructions[instructions[pointer + 1]] = pointer == 0 ? phaseSetting : inputFromAmp;
                    pointer += 2;
                    break;
                case "04":
                    var returnValue = GetValue(instructions[pointer + 1], mode1);
                    pointer += 2;
                    return returnValue;
                case "05":
                    pointer = GetValue(instructions[pointer + 1], mode1) != 0 ? GetValue(instructions[pointer + 2], mode2) : pointer + 3;
                    break;
                case "06":
                    pointer = GetValue(instructions[pointer + 1], mode1) == 0 ? GetValue(instructions[pointer + 2], mode2) : pointer + 3;
                    break;
                case "07":
                    instructions[instructions[pointer + 3]] = GetValue(instructions[pointer + 1], mode1) < GetValue(instructions[pointer + 2], mode2) ? 1 : 0;
                    pointer += 4;
                    break;
                case "08":
                    instructions[instructions[pointer + 3]] = GetValue(instructions[pointer + 1], mode1) == GetValue(instructions[pointer + 2], mode2) ? 1 : 0;
                    pointer += 4;
                    break;
                default:
                    Console.WriteLine("Bug");
                    return null;
            }
            return null;
        }

        private int GetValue(int value, string mode)
        {
            return mode == "0" ? instructions[value] : value;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bestOutput = 0;
            for (var a = 5; a < 10; a++)
            {
                for (var b = 5; b < 10; b++)
                {
                    if (b == a)
                    {
                        continue;
                    }
                    for (var c = 5; c < 10; c++)
                    {
                        if (c == a || c == b)
                        {
                            continue;
                        }
                        for (var d = 5; d < 10; d++)
                        {
                            if (d == c || d == b || d == a)
                            {
                                continue;
                            }
                            for (var e = 5; e < 10; e++)
                            {
                                if (e == d || e == c || e == b || e == a)
                                {
                                    continue;
                                }
                                var input = 0;
                                var runs = 0;
                                var stopCode = false;
                                var lastOutputFromE = 0;
                                var amps = new IntCode[] {
                                    new IntCode(GetInputFromFile(), a, "A"),
                                    new IntCode(GetInputFromFile(), b, "B"),
                                    new IntCode(GetInputFromFile(), c, "C"),
                                    new IntCode(GetInputFromFile(), d, "D"),
                                    new IntCode(GetInputFromFile(), e, "E")
                                };
                                while (true)
                                {
                                    var amp = amps[runs % 5];
                                    var output = amp.RunProgram(input);
                                    if (!output.HasValue)
                                    {
                                        stopCode = true;
                                    }
                                    else
                                    {
                                        input = output.Value;
                                    }
                                    if (amp.name == "E")
                                    {
                                        if (output.HasValue)
                                        {
                                            lastOutputFromE = output.Value;
                                        }
                                        if (stopCode)
                                        {
                                            bestOutput = lastOutputFromE > bestOutput ? lastOutputFromE : bestOutput;
                                            break;
                                        }
                                    }
                                    runs++;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(bestOutput);
        }

        static int[] GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => Int32.Parse(d)).ToArray();
        }
    }
}
