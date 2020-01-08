using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AoC._2019._22
{
    enum Technique
    {
        DealNew,
        Cut,
        DealWithIncrement
    };

    class Program
    {
        static void Main(string[] args)
        {
            Part2();
        }

        static void Part1()
        {
            var deck = new List<int>();
            for (var i = 0; i < 10007; i++)
            {
                deck.Add(i);
            }
            var input = GetInputFromFile();
            foreach (var instruction in input)
            {
                if (instruction == "deal into new stack")
                {
                    DealNewStack(deck);
                }
                else if (instruction.StartsWith("cut"))
                {
                    var param = int.Parse(instruction.Split(" ")[1]);
                    Cut(deck, param);
                }
                else if (instruction.StartsWith("deal with increment"))
                {
                    var param = int.Parse(instruction.Split(" ")[3]);
                    deck = DealWithIncrement(deck, param);
                }
            }

            Console.WriteLine(deck.IndexOf(2019));
        }

        static void Part2()
        {
            BigInteger a = 1;
            BigInteger b = 0;
            long deckSize = 119315717514047;
            long shuffles = 101741582076661;

            var input = GetInputFromFile();
            foreach (var instruction in input)
            {
                if (instruction == "deal into new stack")
                {
                    a *= -1;
                    b = -1 - b;
                }
                else if (instruction.StartsWith("cut"))
                {
                    var param = BigInteger.Parse(instruction.Split(" ")[1]);
                    b -= param;
                }
                else if (instruction.StartsWith("deal with increment"))
                {
                    var param = BigInteger.Parse(instruction.Split(" ")[3]);
                    a *= param;
                    b *= param;
                }

                a %= deckSize;
                b %= deckSize;
            }

            var bigA = BigInteger.ModPow(a, shuffles, deckSize);
            var bigB = b * (1 - bigA) * BigInteger.ModPow((1 - a), deckSize - 2, deckSize);
            var inv = (2020 - bigB) * BigInteger.ModPow(bigA, deckSize - 2, deckSize) % deckSize;
            Console.WriteLine(inv);
        }

        static void DealNewStack(List<int> deck)
        {
            deck.Reverse();
        }

        static void Cut(List<int> deck, int cardsToCut)
        {
            if (cardsToCut > 0)
            {
                var subList = deck.Take(cardsToCut).ToList();
                deck.RemoveRange(0, cardsToCut);
                deck.AddRange(subList);
            }
            else
            {
                cardsToCut = -1 * cardsToCut;
                var subList = deck.TakeLast(cardsToCut).ToList();
                deck.RemoveRange(deck.Count - cardsToCut, cardsToCut);
                deck.InsertRange(0, subList.ToList());
            }
        }

        static List<int> DealWithIncrement(List<int> deck, int increment)
        {
            var newDeck = new int[deck.Count];
            var pointer = 0;
            foreach (var card in deck)
            {
                newDeck[pointer] = card;
                pointer = (pointer + increment) % deck.Count;
            }

            return newDeck.ToList();
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }
}
