using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AoC._2019._13.Tile;

namespace AoC._2019._13
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

    class Tile
    {
        public enum TileType
        {
            Empty,
            Wall,
            Block,
            Paddle,
            Ball
        }

        public (int x, int y) Coordinates { get; set; }
        public TileType Type { get; set; }

        public Tile(TileType type, int x, int y)
        {
            Type = type;
            Coordinates = (x, y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            input[0] = 2;
            var computer = new IntCode(input);
            var tiles = new List<Tile>();
            var score = 0;
            var gameOver = false;
            var joyStick = 0;
            while (!gameOver)
            {
                var x = computer.RunProgram(joyStick);
                var y = computer.RunProgram();
                var type = computer.RunProgram();

                if (x.HasValue && y.HasValue && type.HasValue)
                {
                    if (x == -1 && y == 0)
                    {
                        score = (int)type;
                        continue;
                    }

                    var oldTile = tiles.Where(t => t.Coordinates.x == x && t.Coordinates.y == y).SingleOrDefault();
                    if (oldTile == null)
                    {
                        tiles.Add(new Tile((TileType)type.Value, (int)x.Value, (int)y.Value));
                    }
                    else
                    {
                        if ((TileType)type.Value == TileType.Ball)
                        {
                            var paddleX = tiles.Single(t => t.Type == TileType.Paddle).Coordinates.x;
                            joyStick = paddleX == x ? 0 : x > paddleX ? 1 : -1;
                        }
                        oldTile.Type = (TileType)type;
                    }
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine(score);
        }

        static string GetCharacter(TileType type)
        {
            switch (type)
            {
                case TileType.Empty:
                    return " ";
                case TileType.Wall:
                    return "|";
                case TileType.Block:
                    return "#";
                case TileType.Paddle:
                    return "-";
                case TileType.Ball:
                    return "o";
                default:
                    return string.Empty;
            }
        }

        static List<double> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => double.Parse(d)).ToList();
        }
    }
}
