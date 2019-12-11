using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._11
{
    class IntCode
    {
        private List<double> Instructions { get; set; }
        private double? Input { get; set; }
        private int Pointer { get; set; }
        private int RelativeBase { get; set; }

        public IntCode(List<double> instructions, double? input = null)
        {
            Instructions = instructions;
            Input = input;
            Pointer = 0;
            RelativeBase = 0;
        }

        public double? RunProgram(double? input = null)
        {
            if (input.HasValue)
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
                    RelativeBase += (int)GetValue((int)Instructions.ElementAtOrDefault(Pointer + 1), mode1);
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
                    return Instructions.ElementAtOrDefault(RelativeBase + (int)value);
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
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        };

        static void Main(string[] args)
        {
            var painted = new List<(int? x, int? y, int? p)>();
            var input = GetInputFromFile();
            var computer = new IntCode(input);
            var currentPaint = 1;
            var currentDirection = Direction.Up;
            var currentCoordinates = (x: 0, y: 0);
            while (true)
            {
                var paint = computer.RunProgram(currentPaint);
                if (!paint.HasValue)
                {
                    break;
                }

                var previousPaintIndex = painted.FindIndex(p => p.x == currentCoordinates.x && p.y == currentCoordinates.y);
                if (previousPaintIndex > -1)
                {
                    painted[previousPaintIndex] = (currentCoordinates.x, currentCoordinates.y, (int)paint.Value);
                }
                else
                {
                    painted.Add((currentCoordinates.x, currentCoordinates.y, (int)paint.Value));
                }

                var turn = computer.RunProgram();
                if (!turn.HasValue)
                {
                    break;
                }

                currentDirection = GetNextDirection(currentDirection, (int)turn.Value);
                currentCoordinates = GetNextCoordinates(currentDirection, currentCoordinates);

                var nextTile = painted.SingleOrDefault(p => p.x == currentCoordinates.x && p.y == currentCoordinates.y);
                currentPaint = nextTile.p.HasValue ? nextTile.p.Value : 1;
            }
            var minY = painted.Min(p => p.y) - 1;
            var maxY = painted.Max(p => p.y) + 1;
            var minX = painted.Min(p => p.x) - 1;
            var maxX = painted.Max(p => p.x) + 1;
            for (var y = minY; y < maxY; y++)
            {
                for (var x = minX; x < maxX; x++)
                {
                    var paint = painted.SingleOrDefault(p => p.x == x && p.y == y);
                    if (paint.x.HasValue)
                    {
                        Console.Write(paint.p.Value == 1 ? "#" : ".");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        static Direction GetNextDirection(Direction currentDirection, int turn)
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    return turn == 0 ? Direction.Left : Direction.Right;
                case Direction.Down:
                    return turn == 0 ? Direction.Right : Direction.Left;
                case Direction.Left:
                    return turn == 0 ? Direction.Down : Direction.Up;
                case Direction.Right:
                    return turn == 0 ? Direction.Up : Direction.Down;
                default:
                    Console.WriteLine("bug");
                    return Direction.Up;
            }
        }

        static (int x, int y) GetNextCoordinates(Direction direction, (int x, int y) c)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (c.x, c.y - 1);
                case Direction.Down:
                    return (c.x, c.y + 1);
                case Direction.Left:
                    return (c.x - 1, c.y);
                case Direction.Right:
                    return (c.x + 1, c.y);
                default:
                    Console.WriteLine("bug");
                    return c;
            }
        }

        static List<double> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => double.Parse(d)).ToList();
        }
    }
}
