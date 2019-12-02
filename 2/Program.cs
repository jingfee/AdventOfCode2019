using System;
using System.IO;
using System.Linq;

namespace AoC._2019._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var breakOutput = 19690720;
            for (var noun = 0; noun < 100; noun++)
            {
                for (var verb = 0; verb < 100; verb++)
                {
                    var input = GetInputFromFile();
                    input[1] = noun;
                    input[2] = verb;
                    if (breakOutput == RunProgram(input))
                    {
                        Console.WriteLine(100 * noun + verb);
                    }
                }
            }
        }

        static int RunProgram(int[] input)
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

                input[input[pointer + 3]] = DoOperation(op, input[input[pointer + 1]], input[input[pointer + 2]]);
                pointer += 4;
            }

            return input[0];
        }

        static int DoOperation(int operation, int input1, int input2)
        {
            switch (operation)
            {
                case 1:
                    return input1 + input2;
                case 2:
                    return input1 * input2;
                default:
                    return -1;
            }
        }

        static int[] GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => Int32.Parse(d)).ToArray();
        }
    }
}
