using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._15
{
    class IntCode
    {
        private List<int> Instructions { get; set; }
        private int? Input { get; set; }
        private int Pointer { get; set; }
        private int RelativeBase { get; set; }

        public IntCode(List<int> instructions, int? input = null)
        {
            Instructions = instructions;
            Input = input;
            Pointer = 0;
            RelativeBase = 0;
        }

        public int? RunProgram(int? input = null)
        {
            if (input.HasValue)
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
                    if (!Input.HasValue)
                    {
                        Console.WriteLine("Bug");
                        break;
                    }
                    InsertValue(Input.Value, Pointer + 1, mode1);
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
            var code = GetInputFromFile();
            var computer = new IntCode(code);
            var visited = new List<(int x, int y, int status, bool hasOxygen)>() { (0, 0, 1, false) };
            var possibleMovements = new Queue<(int x, int y, List<int> path)>();
            possibleMovements.Enqueue((0, 0, new List<int>()));
            (int x, int y) oxygenLocation = (0, 0);
            while (possibleMovements.Count > 0)
            {
                var currentPosition = possibleMovements.Dequeue();
                MoveDroidToPosition(computer, currentPosition.path);
                for (var i = 1; i <= 4; i++)
                {
                    var newPosition = GetNewPosition(i, (currentPosition.x, currentPosition.y));
                    if (!visited.Any(v => v.x == newPosition.x && v.y == newPosition.y))
                    {
                        var status = computer.RunProgram(i);
                        visited.Add((newPosition.x, newPosition.y, status.Value, false));
                        if (status != 0)
                        {
                            var newPath = currentPosition.path.Append(i).ToList();
                            possibleMovements.Enqueue((newPosition.x, newPosition.y, newPath));
                            computer.RunProgram(GetBackMovement(i));
                        }
                        if (status == 2)
                        {
                            oxygenLocation = (newPosition.x, newPosition.y);
                        }
                    }
                }
                MoveDroidToOrigin(computer, currentPosition.path);
            }

            var oxygenSpread = new Queue<(int x, int y, int minutes)>();
            oxygenSpread.Enqueue((oxygenLocation.x, oxygenLocation.y, 0));
            var minutes = 0;
            while (visited.Any(v => v.status == 1 && !v.hasOxygen))
            {
                var spread = oxygenSpread.Dequeue();
                minutes = spread.minutes;
                SetOxygen(spread.x, spread.y, visited);
                for (var i = 1; i <= 4; i++)
                {
                    var newSpread = GetNewPosition(i, (spread.x, spread.y));
                    if (visited.Any(v => v.status == 1 && v.x == newSpread.x && v.y == newSpread.y && !v.hasOxygen))
                    {
                        oxygenSpread.Enqueue((newSpread.x, newSpread.y, spread.minutes + 1));
                    }
                }
            }

            Console.WriteLine(minutes);
        }

        static void SetOxygen(int x, int y, List<(int x, int y, int status, bool hasOxygen)> map)
        {
            var location = map.Single(m => m.x == x && m.y == y);
            var index = map.IndexOf(location);
            map[index] = (location.x, location.y, location.status, true);
        }

        static void MoveDroidToPosition(IntCode computer, List<int> path)
        {
            foreach (var step in path)
            {
                computer.RunProgram(step);
            }
        }

        static void MoveDroidToOrigin(IntCode computer, List<int> path)
        {
            for (var i = path.Count - 1; i >= 0; i--)
            {
                computer.RunProgram(GetBackMovement(path[i]));
            }
        }

        static int GetBackMovement(int movement)
        {
            switch (movement)
            {
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 4;
                case 4:
                    return 3;
                default:
                    return -1;
            }
        }

        static (int x, int y) GetNewPosition(int movement, (int x, int y) currentPosition)
        {
            switch (movement)
            {
                case 1:
                    return (currentPosition.x, currentPosition.y - 1);
                case 2:
                    return (currentPosition.x, currentPosition.y + 1);
                case 3:
                    return (currentPosition.x - 1, currentPosition.y);
                case 4:
                    return (currentPosition.x + 1, currentPosition.y);
                default:
                    return (0, 0);
            }
        }

        static List<int> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => int.Parse(d)).ToList();
        }
    }
}
