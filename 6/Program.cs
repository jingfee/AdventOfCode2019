using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._6
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var orbitMap = new List<OrbitObject>();
            foreach (var orbit in input)
            {
                var split = orbit.Split(")");

                var orbitee = orbitMap.SingleOrDefault(o => o.Name == split[1]);
                if (orbitee == null)
                {
                    orbitee = new OrbitObject(split[1]);
                    orbitMap.Add(orbitee);
                }

                var orbitObject = orbitMap.SingleOrDefault(o => o.Name == split[0]);
                if (orbitObject == null)
                {
                    orbitObject = new OrbitObject(split[0]);
                    orbitMap.Add(orbitObject);
                }
                orbitee.Orbits = orbitObject;
                orbitObject.Orbiters.Add(orbitee);
            }

            var visited = new List<OrbitObject>();
            var path = new List<OrbitObject>();
            var you = orbitMap.Single(o => o.Name == "YOU");
            var pathToSanta = FindSanta(you.Orbits, path, visited);

            Console.WriteLine(pathToSanta.Count - 1);
        }

        static List<OrbitObject> FindSanta(OrbitObject orbitObject, List<OrbitObject> path, List<OrbitObject> visited)
        {
            visited.Add(orbitObject);
            if (orbitObject.Orbiters.Any(o => o.Name == "SAN"))
            {
                path.Add(orbitObject);
                return path;
            }

            var connections = orbitObject.Orbiters.Concat(new List<OrbitObject> { orbitObject.Orbits }).Where(o => !visited.Contains(o));
            foreach (var possibleTransfer in connections)
            {
                var newPath = path.Concat(new List<OrbitObject> { orbitObject }).ToList();
                var foundPath = FindSanta(possibleTransfer, newPath, visited);
                if (foundPath != null && foundPath.Count > 0)
                {
                    return foundPath;
                }
            }
            return null;
        }

        static int CalcDepth(OrbitObject orbitObject, int depth)
        {
            if (orbitObject.Orbits == null)
            {
                return depth;
            }
            else
            {
                return CalcDepth(orbitObject.Orbits, depth + 1);
            }
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }

    class OrbitObject
    {
        public string Name { get; set; }
        public OrbitObject Orbits { get; set; }
        public List<OrbitObject> Orbiters { get; set; }
        public int TotalOrbits { get; set; }

        public OrbitObject(string name)
        {
            Orbiters = new List<OrbitObject>();
            Name = name;
        }
    }
}
