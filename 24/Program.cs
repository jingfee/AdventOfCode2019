using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AoC._2019._24
{
    class Program
    {
        static void Main(string[] args)
        {
            var biodiversities = new HashSet<double>();
            var input = GetInputFromFile();
            var maps = new Dictionary<int, int[,]>();
            var map = new int[5, 5];
            for (var y = 0; y < 5; y++)
            {
                var row = input[y];
                for (var x = 0; x < 5; x++)
                {
                    if (row[x] == '#')
                    {
                        map[y, x] = 1;
                    }
                }
            }
            maps.Add(0, map);

            for (var minutes = 0; minutes < 200; minutes++)
            {
                if (!maps.ContainsKey(-1 * minutes - 1))
                {
                    maps.Add(-1 * minutes - 1, new int[5, 5]);
                }
                if (!maps.ContainsKey(minutes + 1))
                {
                    maps.Add(minutes + 1, new int[5, 5]);
                }

                var mapsToAdd = new List<KeyValuePair<int, int[,]>>();
                foreach (var mapToUpdate in maps)
                {
                    var newMap = new int[5, 5];
                    var innerMap = maps.ContainsKey(mapToUpdate.Key + 1) ? maps[mapToUpdate.Key + 1] : new int[5, 5];
                    var outerMap = maps.ContainsKey(mapToUpdate.Key - 1) ? maps[mapToUpdate.Key - 1] : new int[5, 5];

                    for (var y = 0; y < 5; y++)
                    {
                        for (var x = 0; x < 5; x++)
                        {
                            if (y == 2 && x == 2)
                            {
                                continue;
                            }

                            if (mapToUpdate.Value[y, x] == 1)
                            {
                                newMap[y, x] = GetAdjacentBugs(mapToUpdate.Value, innerMap, outerMap, x, y) == 1 ? 1 : 0;
                            }
                            else
                            {
                                var adjacentBugs = GetAdjacentBugs(mapToUpdate.Value, innerMap, outerMap, x, y);
                                newMap[y, x] = adjacentBugs == 1 || adjacentBugs == 2 ? 1 : 0;
                            }
                        }
                    }

                    mapsToAdd.Add(new KeyValuePair<int, int[,]>(mapToUpdate.Key, newMap));
                }

                foreach (var mapToAdd in mapsToAdd)
                {
                    maps[mapToAdd.Key] = mapToAdd.Value;
                }
                mapsToAdd.Clear();
            }

            Console.WriteLine(maps.Sum(m => GetBugs(m.Value)));
        }

        static int GetAdjacentBugs(int[,] map, int[,] innerMap, int[,] outerMap, int x, int y)
        {
            int up;
            if (y == 0)
            {
                up = outerMap[1, 2];
            }
            else if (y == 3 && x == 2)
            {
                up = innerMap[4, 0] + innerMap[4, 1] + innerMap[4, 2] + innerMap[4, 3] + innerMap[4, 4];
            }
            else
            {
                up = map[y - 1, x];
            }

            int right;
            if (x == 4)
            {
                right = outerMap[2, 3];
            }
            else if (y == 2 && x == 1)
            {
                right = innerMap[0, 0] + innerMap[1, 0] + innerMap[2, 0] + innerMap[3, 0] + innerMap[4, 0];
            }
            else
            {
                right = map[y, x + 1];
            }

            int down;
            if (y == 4)
            {
                down = outerMap[3, 2];
            }
            else if (y == 1 && x == 2)
            {
                down = innerMap[0, 0] + innerMap[0, 1] + innerMap[0, 2] + innerMap[0, 3] + innerMap[0, 4];
            }
            else
            {
                down = map[y + 1, x];
            }

            int left;
            if (x == 0)
            {
                left = outerMap[2, 1];
            }
            else if (y == 2 && x == 3)
            {
                left = innerMap[0, 4] + innerMap[1, 4] + innerMap[2, 4] + innerMap[3, 4] + innerMap[4, 4];
            }
            else
            {
                left = map[y, x - 1];
            }

            return up + right + down + left;
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }

        static double GetBiodiversity(int[,] map)
        {
            double biodiversity = 0;
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    if (map[y, x] == 1)
                    {
                        biodiversity += Math.Pow(2, y * 5 + x);
                    }
                }
            }
            return biodiversity;
        }

        static int GetBugs(int[,] map)
        {
            var bugs = 0;
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    if (map[y, x] == 1)
                    {
                        bugs++;
                    }
                }
            }
            return bugs;
        }
    }
}
