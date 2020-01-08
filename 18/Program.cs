using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._18
{
    class Key
    {
        string name { get; set; }
        List<(Key key, int distance)> possibleKeys { get; set; }

        public Key()
        {
            possibleKeys = new List<(Key key, int distance)>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var map = new char[input[0].Length, input.Length];
            var possiblePath = new Queue<((int x, int y)[], int steps, string keyString)>();
            var visited = new HashSet<(int x, int y, string keyString)>();
            var numberOfKeys = 0;
            (int x, int y) start = (0, 0);
            for (var y = 0; y < input.Length; y++)
            {
                var row = input[y];
                for (var x = 0; x < row.Length; x++)
                {
                    if (row[x] != '#')
                    {
                        map[x, y] = row[x];

                        if (row[x] >= 97 && row[x] <= 122)
                        {
                            numberOfKeys++;
                        }

                        if (row[x] == '@')
                        {
                            possiblePath.Enqueue((new (int x, int y)[4] { (x - 1, y - 1), (x + 1, y - 1), (x + 1, y + 1), (x - 1, y + 1) }, 0, ""));
                            start = (x, y);
                        }
                    }
                }
            }

            map[start.x, start.y] = '\0';
            map[start.x, start.y - 1] = '\0';
            map[start.x, start.y + 1] = '\0';
            map[start.x - 1, start.y] = '\0';
            map[start.x + 1, start.y] = '\0';

            while (possiblePath.Count > 0)
            {
                var currentPos = possiblePath.Dequeue();

                if (currentPos.keyString.Length == numberOfKeys)
                {
                    Console.WriteLine(currentPos.steps);
                    break;
                }

                for (var r = 0; r < 4; r++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        var robot = currentPos.Item1[r];
                        var newPosition = GetNewPosition(robot.x, robot.y, i);

                        if (newPosition.x.HasValue && newPosition.y.HasValue &&
                         newPosition.x > 0 && newPosition.y > 0 && newPosition.x < input[0].Length && newPosition.y < input.Length) //new coordinates in matrix range
                        {
                            var newTile = map[newPosition.x.Value, newPosition.y.Value];
                            if (newTile != '\0')
                            { //new coordinates is not a wall
                                if (newTile >= 97 && newTile <= 122 && !currentPos.keyString.Contains(newTile.ToString()))
                                { //new coordinate is a key which isn't picked up already
                                    var newKeys = String.Concat((currentPos.keyString + newTile).OrderBy(c => c));
                                    if (visited.Add((newPosition.x.Value, newPosition.y.Value, newKeys)))
                                    {
                                        (int x, int y)[] newPos = new (int x, int y)[4];
                                        currentPos.Item1.CopyTo(newPos, 0);
                                        newPos[r] = (newPosition.x.Value, newPosition.y.Value);
                                        possiblePath.Enqueue((newPos, currentPos.steps + 1, newKeys));
                                    }
                                }
                                else if (newTile >= 65 && newTile <= 90)
                                { //new coordinate is a door
                                    if (currentPos.keyString.Contains(((char)(newTile + 32)).ToString()))
                                    {//we have key
                                        if (visited.Add((newPosition.x.Value, newPosition.y.Value, currentPos.keyString)))
                                        {
                                            (int x, int y)[] newPos = new (int x, int y)[4];
                                            currentPos.Item1.CopyTo(newPos, 0);
                                            newPos[r] = (newPosition.x.Value, newPosition.y.Value);
                                            possiblePath.Enqueue((newPos, currentPos.steps + 1, currentPos.keyString));
                                        }
                                    }
                                }
                                else
                                { //new coordinate is normal path
                                    if (visited.Add((newPosition.x.Value, newPosition.y.Value, currentPos.keyString)))
                                    {
                                        (int x, int y)[] newPos = new (int x, int y)[4];
                                        currentPos.Item1.CopyTo(newPos, 0);
                                        newPos[r] = (newPosition.x.Value, newPosition.y.Value);
                                        possiblePath.Enqueue((newPos, currentPos.steps + 1, currentPos.keyString));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static (int? x, int? y) GetNewPosition(int currentX, int currentY, int direction)
        {
            switch (direction)
            {
                case 0:
                    return (currentX, currentY - 1);
                case 1:
                    return (currentX + 1, currentY);
                case 2:
                    return (currentX, currentY + 1);
                case 3:
                    return (currentX - 1, currentY);
                default:
                    return (null, null);
            }
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }
}
