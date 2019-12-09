using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._8
{
    class Program
    {
        static int HEIGHT = 6;
        static int WIDTH = 25;

        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var layers = new List<int[]>();
            int[] currentLayer = new int[WIDTH * HEIGHT];
            layers.Add(currentLayer);
            for (var i = 0; i < input.Length; i++)
            {
                if (i > 0 && (i % (WIDTH * HEIGHT)) == 0)
                {
                    currentLayer = new int[WIDTH * HEIGHT];
                    layers.Add(currentLayer);
                }

                currentLayer[i % (WIDTH * HEIGHT)] = int.Parse(input[i].ToString());
            }

            for (var y = 0; y < HEIGHT; y++)
            {
                for (var x = 0; x < WIDTH; x++)
                {
                    Console.Write(GetOutput(layers, x, y));
                }
                Console.WriteLine();
            }
        }

        static string GetOutput(List<int[]> layers, int x, int y)
        {
            foreach (var layer in layers)
            {
                var d = layer[y * WIDTH + x];
                if (d == 2)
                {
                    continue;
                }
                else if (d == 1)
                {
                    return "|";
                }
                else
                {
                    return " ";
                }
            }
            return " ";
        }

        static string GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data;
        }
    }
}
