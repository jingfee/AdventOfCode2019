using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC._2019._12
{
    class Moon
    {
        public List<(int x, int y, int z)> Positions { get; set; }
        public (int x, int y, int z) Velocity { get; set; }


        public (List<int> x, List<int> y, List<int> z) positionPattern;
        public (bool x, bool y, bool z) patternFound;

        private string pattern = @"^<x=(?<x>-?[0-9]+), y=(?<y>-?[0-9]+), z=(?<z>-?[0-9]+)>$";

        public Moon(string positionRaw)
        {
            var match = Regex.Match(positionRaw, pattern);
            Positions = new List<(int x, int y, int z)>() {
                (int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value), int.Parse(match.Groups["z"].Value))
            };
            positionPattern.x = new List<int>();
            positionPattern.y = new List<int>();
            positionPattern.z = new List<int>();
        }

        public int PotentialEnergy()
        {
            return Math.Abs(Positions.Last().x) + Math.Abs(Positions.Last().y) + Math.Abs(Positions.Last().z);
        }

        public int KineticEnergy()
        {
            return Math.Abs(Velocity.x) + Math.Abs(Velocity.y) + Math.Abs(Velocity.z);
        }

        public int TotalEnergy()
        {
            return PotentialEnergy() * KineticEnergy();
        }
    }

    class Program
    {
        private static List<Moon> moons = new List<Moon>();

        static void Main(string[] args)
        {
            /*var input = GetInputFromFile();
            foreach (var row in input)
            {
                moons.Add(new Moon(row));
            }

            while (true)
            {
                if (moons.All(m => m.patternFound.x && m.patternFound.y && m.patternFound.z))
                {
                    break;
                }
                TimeStep();
            }

            var frequencies = moons.SelectMany(m => new List<int>() { m.positionPattern.x.Count, m.positionPattern.y.Count, m.positionPattern.z.Count }).Distinct();*/
            var frequencies = new List<int>() {
                286332,
                193052,
                102356
            };
            var increment = frequencies.Min();
            double step = increment;
            while (true)
            {
                if (frequencies.All(f => step % f == 0))
                {
                    Console.WriteLine(step);
                    break;
                }
                step += increment;
            }
        }

        static void TimeStep()
        {
            UpdateVelocity();
            UpdatePosition();
        }

        static void UpdateVelocity()
        {
            foreach (var moon in moons)
            {
                foreach (var mg in moons)
                {
                    if (moon == mg)
                    {
                        continue;
                    }

                    moon.Velocity = (
                        moon.Velocity.x + GetGravity(moon.Positions.Last().x, mg.Positions.Last().x),
                        moon.Velocity.y + GetGravity(moon.Positions.Last().y, mg.Positions.Last().y),
                        moon.Velocity.z + GetGravity(moon.Positions.Last().z, mg.Positions.Last().z)
                    );
                }
            }
        }

        static void UpdatePosition()
        {
            foreach (var moon in moons)
            {
                (int x, int y, int z) position = (
                    moon.Positions.Last().x + moon.Velocity.x,
                    moon.Positions.Last().y + moon.Velocity.y,
                    moon.Positions.Last().z + moon.Velocity.z
                );

                moon.Positions.Add(position);

                if (!moon.patternFound.x)
                {
                    if (position.x != moon.Positions[moon.positionPattern.x.Count].x)
                    {
                        moon.positionPattern.x.Clear();
                    }

                    if (position.x == moon.Positions[moon.positionPattern.x.Count].x)
                    {
                        moon.positionPattern.x.Add(position.x);
                        if (moon.positionPattern.x.Count == ((decimal)moon.Positions.Count / 2))
                        {
                            moon.patternFound.x = true;
                        }
                    }
                }
                if (!moon.patternFound.y)
                {
                    if (position.y != moon.Positions[moon.positionPattern.y.Count].y)
                    {
                        moon.positionPattern.y.Clear();
                    }

                    if (position.y == moon.Positions[moon.positionPattern.y.Count].y)
                    {
                        moon.positionPattern.y.Add(position.y);
                        if (moon.positionPattern.y.Count == ((decimal)moon.Positions.Count / 2))
                        {
                            moon.patternFound.y = true;
                        }
                    }
                }
                if (!moon.patternFound.z)
                {
                    if (position.z != moon.Positions[moon.positionPattern.z.Count].z)
                    {
                        moon.positionPattern.z.Clear();
                    }

                    if (position.z == moon.Positions[moon.positionPattern.z.Count].z)
                    {
                        moon.positionPattern.z.Add(position.z);
                        if (moon.positionPattern.z.Count == ((decimal)moon.Positions.Count / 2))
                        {
                            moon.patternFound.z = true;
                        }
                    }
                }
            }
        }

        static int GetGravity(int position1, int position2)
        {
            return position1 > position2 ? -1 : position1 < position2 ? 1 : 0;
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }
}
