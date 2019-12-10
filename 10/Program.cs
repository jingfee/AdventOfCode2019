using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._10
{
    class Asteroid
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<LineOfSight> LineOfSights { get; set; }

        public Asteroid(int x, int y)
        {
            X = x;
            Y = y;
            LineOfSights = new List<LineOfSight>();
        }
    }

    class LineOfSight
    {
        public decimal Slope { get; set; }
        public int Direction { get; set; }
        public List<Asteroid> Asteroids { get; set; }

        public LineOfSight(decimal slope, int direction, Asteroid asteroid)
        {
            Slope = slope;
            Direction = direction;
            Asteroids = new List<Asteroid>() { asteroid };
        }
    }

    class Program
    {
        private static List<Asteroid> asteroids = new List<Asteroid>();

        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            ParseAsteroids(input);
            foreach (var candidate in asteroids)
            {
                foreach (var target in asteroids)
                {
                    if (candidate == target)
                    {
                        continue;
                    }

                    var lineOfSight = CalculateLineOfSight(candidate, target);

                    var existingLoS = candidate.LineOfSights.SingleOrDefault(l => l.Slope == lineOfSight.Slope && l.Direction == lineOfSight.Direction);
                    if (existingLoS == null)
                    {
                        candidate.LineOfSights.Add(lineOfSight);
                    }
                    else
                    {
                        existingLoS.Asteroids.Add(target);
                    }
                }
            }

            var baseAsteroid = asteroids.OrderByDescending(a => a.LineOfSights.Count).First();

            var laserDirection = baseAsteroid.LineOfSights.OrderByDescending(x => x.Direction).ThenByDescending(x => x.Slope).ToList();
            for (var i = 0; i < 200; i++)
            {
                var currentLoS = laserDirection[i % laserDirection.Count];
                var asteroidToBlast = currentLoS.Asteroids.OrderBy(x => Math.Abs(baseAsteroid.X - x.X) + Math.Abs(baseAsteroid.Y - x.Y)).FirstOrDefault();
                if (asteroidToBlast == null)
                {
                    continue;
                }
                currentLoS.Asteroids.Remove(asteroidToBlast);
                /*if(i == 199) {
                    Console.WriteLine(asteroidToBlast.X*100 + asteroidToBlast.Y);
                }*/
                Console.WriteLine($"{i + 1}: {asteroidToBlast.X},{asteroidToBlast.Y}");
            }
        }

        static LineOfSight CalculateLineOfSight(Asteroid origin, Asteroid target)
        {
            var diffX = target.X - origin.X;
            var diffY = origin.Y - target.Y;
            decimal slope;
            int direction;
            if (diffX != 0)
            {
                slope = (decimal)diffY / (decimal)diffX;
                direction = diffX > 0 ? 1 : -1;
            }
            else
            {
                slope = decimal.MaxValue;
                direction = diffY > 0 ? 1 : -1;
            }

            return new LineOfSight(slope, direction, target);
        }

        static void ParseAsteroids(string[] map)
        {
            for (var y = 0; y < map.Length; y++)
            {
                var row = map[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var coordinate = row[x];
                    if (coordinate == '#')
                    {
                        asteroids.Add(new Asteroid(x, y));
                    }
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
