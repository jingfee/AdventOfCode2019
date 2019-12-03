using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._3
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var map = new List<Wire>();
            var wireId = 0;
            foreach (var wire in input)
            {
                var startPosition = (0, 0, 0);
                foreach (var instruction in wire)
                {
                    startPosition = PlotWire(startPosition, instruction, map, wireId);
                }
                wireId++;
            }

            var closestIntersection = map.Where(m => m.Intersection != (0, 0, 0)).OrderBy(m => m.Intersection.totalSteps).First();
            Console.WriteLine(closestIntersection.Intersection.totalSteps);
        }

        static (int x, int y, int steps) PlotWire((int x, int y, int steps) startPosition, string instruction, List<Wire> map, int wireId)
        {
            var direction = instruction.Substring(0, 1);
            var length = Int32.Parse(instruction.Substring(1));

            (int x, int y, int steps) endPosition = (0, 0, 0);
            switch (direction)
            {
                case "U":
                    endPosition = (startPosition.x, startPosition.y + length, startPosition.steps + length);
                    break;
                case "D":
                    endPosition = (startPosition.x, startPosition.y - length, startPosition.steps + length);
                    break;
                case "L":
                    endPosition = (startPosition.x - length, startPosition.y, startPosition.steps + length);
                    break;
                case "R":
                    endPosition = (startPosition.x + length, startPosition.y, startPosition.steps + length);
                    break;
                default:
                    break;
            }
            AddCoordinate(startPosition, endPosition, map, wireId);
            return endPosition;
        }

        static void AddCoordinate((int x, int y, int steps) startPosition, (int x, int y, int steps) endPosition, List<Wire> map, int wireId)
        {
            var wire = new Wire()
            {
                From = startPosition,
                To = endPosition,
                WireId = wireId
            };

            if (startPosition.x == endPosition.x)
            {
                var intersectingWire = map.Where(m => m.WireId != wireId && m.From.y == m.To.y && m.IsXInside(startPosition.x) && wire.IsYInside(m.From.y)).OrderBy(m => m.From.steps + Math.Abs(m.From.x - startPosition.x)).FirstOrDefault();
                if (intersectingWire != null)
                {
                    wire.Intersection = (startPosition.x, intersectingWire.From.y, (startPosition.steps + Math.Abs(startPosition.y - intersectingWire.From.y) + intersectingWire.From.steps + Math.Abs(intersectingWire.From.x - startPosition.x)));
                }
            }
            else
            {
                var intersectingWire = map.Where(m => m.WireId != wireId && m.From.x == m.To.x && wire.IsXInside(m.From.x) && m.IsYInside(startPosition.y)).OrderBy(m => m.From.steps + Math.Abs(m.From.y - startPosition.y)).FirstOrDefault();
                if (intersectingWire != null)
                {
                    wire.Intersection = (intersectingWire.From.x, startPosition.y, (startPosition.steps + Math.Abs(startPosition.x - intersectingWire.From.x) + intersectingWire.From.steps + Math.Abs(intersectingWire.From.y - startPosition.y)));
                }
            }

            map.Add(wire);
        }

        static string[][] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data.Select(d => d.Split(',')).ToArray();
        }
    }

    class Wire
    {
        public (int x, int y, int steps) From { get; set; }
        public (int x, int y, int steps) To { get; set; }
        public int WireId { get; set; }
        public (int x, int y, int totalSteps) Intersection { get; set; }

        public int Distance()
        {
            return Math.Abs(Intersection.x) + Math.Abs(Intersection.y);
        }

        public bool IsXInside(int x)
        {
            return (From.x > x && To.x < x) || (To.x > x && From.x < x);
        }

        public bool IsYInside(int y)
        {
            return (From.y > y && To.y < y) || (To.y > y && From.y < y);
        }
    }
}
