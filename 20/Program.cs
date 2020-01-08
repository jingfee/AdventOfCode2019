using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._20
{
    class Portal
    {
        public string Name { get; set; }
        public (int x, int y, int level)[] Coordinates { get; set; }

        public Portal(string name)
        {
            Name = name;
            Coordinates = new (int x, int y, int level)[2];
        }
    }

    class Program
    {
        static (int x, int y) start;
        static (int x, int y) end;
        static int[,] map;
        static List<Portal> portals = new List<Portal>();
        static int[,] p;
        static int[,] l;
        static (int x, int y)[,] d;

        static HashSet<(int x, int y, int level)> visited = new HashSet<(int x, int y, int level)>();
        static Queue<(int x, int y, int steps, int level)> possiblePaths = new Queue<(int x, int y, int steps, int level)>();

        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            ParseMap(input);
            Console.WriteLine(FindShortestPath());
        }

        static int FindShortestPath()
        {
            possiblePaths.Enqueue((start.x, start.y, 0, 0));
            while (true)
            {
                var path = possiblePaths.Dequeue();
                if (path.x == end.x && path.y == end.y && path.level == 0)
                {
                    return path.steps;
                }
                if (p[path.x, path.y] == 1)
                {
                    var level = l[path.x, path.y];
                    if (path.level > 0 || level > 0)
                    {
                        var newCoordinates = d[path.x, path.y];
                        if (visited.Add((newCoordinates.x, newCoordinates.y, path.level + level)))
                        {
                            possiblePaths.Enqueue((newCoordinates.x, newCoordinates.y, path.steps + 1, path.level + level));
                        }
                    }

                }

                for (var i = 0; i < 4; i++)
                {
                    var newCoordinates = GetNewCoordinates(i, path.x, path.y);
                    if (map[newCoordinates.x, newCoordinates.y] == 1 && visited.Add((newCoordinates.x, newCoordinates.y, path.level)))
                    {
                        possiblePaths.Enqueue((newCoordinates.x, newCoordinates.y, path.steps + 1, path.level));
                    }
                }
            }
        }

        static (int x, int y) GetNewCoordinates(int direction, int x, int y)
        {
            switch (direction)
            {
                case 0:
                    return (x, y - 1);
                case 1:
                    return (x + 1, y);
                case 2:
                    return (x, y + 1);
                case 3:
                    return (x - 1, y);
                default:
                    return (0, 0);
            }
        }

        static void ParseMap(string[] input)
        {
            map = new int[input[0].Length, input.Length];
            p = new int[input[0].Length, input.Length];
            l = new int[input[0].Length, input.Length];
            d = new (int x, int y)[input[0].Length, input.Length];
            for (var y = 0; y < input.Length; y++)
            {
                var row = input[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var tile = row[x];
                    if (tile == ' ')
                    { // Empty
                        continue;
                    }
                    else if (tile == 'A')
                    { // Find start
                        var findStart = FindCoordinateByLegend(input, x, y);
                        if (findStart.LastLetter == 'A')
                        {
                            start = (findStart.x, findStart.y);
                        }
                        else
                        {
                            FindPortal(input, x, y, tile);
                        }
                    }
                    else if (tile == 'Z')
                    { // Find end
                        var findEnd = FindCoordinateByLegend(input, x, y);
                        if (findEnd.LastLetter == 'Z')
                        {
                            end = (findEnd.x, findEnd.y);
                        }
                        else
                        {
                            FindPortal(input, x, y, tile);
                        }
                    }
                    else if (tile == '.')
                    { // Find path
                        map[x, y] = 1;
                    }
                    else if (tile == '#')
                    { // Find wall
                        continue;
                    }
                    else
                    { // Find portal
                        FindPortal(input, x, y, tile);
                    }
                }
            }
        }

        static (int x, int y, char LastLetter, int level) FindCoordinateByLegend(string[] input, int x, int y)
        {
            if (y < input.Length - 2 && (int)input[y + 1][x] >= 65 && (int)input[y + 1][x] <= 90 && input[y + 2][x] == '.')
            {
                if (y == 0)
                {
                    return (x, y + 2, input[y + 1][x], -1);
                }
                else
                {
                    return (x, y + 2, input[y + 1][x], 1);
                }
            }
            else if (x < input[y].Length - 2 && (int)input[y][x + 1] >= 65 && (int)input[y][x + 1] <= 90 && input[y][x + 2] == '.')
            {
                if (x == 0)
                {
                    return (x + 2, y, input[y][x + 1], -1);
                }
                else
                {
                    return (x + 2, y, input[y][x + 1], 1);
                }
            }
            else if (x < input[y].Length - 1 && x > 0 && (int)input[y][x + 1] >= 65 && (int)input[y + 1][x + 1] <= 90 && input[y][x - 1] == '.')
            {
                if (x == input[y].Length - 2)
                {
                    return (x - 1, y, input[y][x + 1], -1);
                }
                else
                {
                    return (x - 1, y, input[y][x + 1], 1);
                }
            }
            else if (y < input.Length - 1 && y > 0 && (int)input[y + 1][x] >= 65 && (int)input[y + 1][x] <= 90 && input[y - 1][x] == '.')
            {
                if (y == input.Length - 2)
                {
                    return (x, y - 1, input[y + 1][x], -1);
                }
                return (x, y - 1, input[y + 1][x], 1);
            }

            return (0, 0, default, 0);
        }

        static void FindPortal(string[] input, int x, int y, char FoundLetter)
        {
            var searchPortal = FindCoordinateByLegend(input, x, y);

            if (searchPortal.LastLetter != default)
            {
                p[searchPortal.x, searchPortal.y] = 1;
                l[searchPortal.x, searchPortal.y] = searchPortal.level;
                var name = FoundLetter.ToString() + searchPortal.LastLetter.ToString();
                var existingPortal = portals.SingleOrDefault(p => p.Name == name);
                if (existingPortal == null)
                {
                    var portal = new Portal(name);
                    portal.Coordinates[0] = (searchPortal.x, searchPortal.y, searchPortal.level);
                    portals.Add(portal);
                }
                else
                {
                    existingPortal.Coordinates[1] = (searchPortal.x, searchPortal.y, searchPortal.level);
                    d[searchPortal.x, searchPortal.y] = (existingPortal.Coordinates[0].x, existingPortal.Coordinates[0].y);
                    d[existingPortal.Coordinates[0].x, existingPortal.Coordinates[0].y] = (searchPortal.x, searchPortal.y);
                }
            }
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }
}
