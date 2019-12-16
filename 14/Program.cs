using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._14
{
    class Reaction
    {
        public List<(int quantity, string chemical)> InputChemicals { get; set; }
        public (int quantity, string chemical) OutputChemical { get; set; }

        public Reaction(string reaction)
        {
            var reactionSplit = reaction.Split(" => ");
            var inputString = reactionSplit[0];
            var outputString = reactionSplit[1];

            InputChemicals = new List<(int quantity, string chemical)>();
            foreach (var input in inputString.Split(", "))
            {
                var inputSplit = input.Split(" ");
                InputChemicals.Add((int.Parse(inputSplit[0]), inputSplit[1]));
            }

            var outputSplit = outputString.Split(" ");
            OutputChemical = (int.Parse(outputSplit[0]), outputSplit[1]);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = GetInputFromFile();
            var reactions = new List<Reaction>();
            foreach (var row in input)
            {
                reactions.Add(new Reaction(row));
            }

            var needs = new Dictionary<string, int>() { { "FUEL", 1 } };
            var restChemicals = new Dictionary<string, double>();
            double oresUsed = 0;
            double fuelProduced = 0;
            while (true)
            {
                var newNeed = new Dictionary<string, int>();
                foreach (var need in needs)
                {
                    var reaction = reactions.Single(r => r.OutputChemical.chemical == need.Key);
                    var numberOfRests = restChemicals.ContainsKey(reaction.OutputChemical.chemical) ? restChemicals[reaction.OutputChemical.chemical] : 0;
                    var reactionsNeeded = (int)Math.Ceiling((decimal)(need.Value - numberOfRests) / (decimal)reaction.OutputChemical.quantity);

                    if (reactionsNeeded <= 0)
                    {
                        restChemicals[reaction.OutputChemical.chemical] = numberOfRests - need.Value;
                    }
                    else
                    {
                        foreach (var inputChemical in reaction.InputChemicals)
                        {
                            var inputChemicalsNeeded = inputChemical.quantity * reactionsNeeded;
                            if (inputChemical.chemical == "ORE")
                            {
                                oresUsed += inputChemicalsNeeded;
                            }
                            else
                            {
                                if (newNeed.ContainsKey(inputChemical.chemical))
                                {
                                    newNeed[inputChemical.chemical] += inputChemicalsNeeded;
                                }
                                else
                                {
                                    newNeed.Add(inputChemical.chemical, inputChemicalsNeeded);
                                }
                            }

                        }
                        restChemicals[reaction.OutputChemical.chemical] = (reactionsNeeded * reaction.OutputChemical.quantity) - (need.Value - numberOfRests);
                    }
                }
                if (newNeed.Count == 0)
                {
                    if (oresUsed < 1000000000000)
                    {
                        needs = new Dictionary<string, int>() { { "FUEL", 1 } };
                        fuelProduced++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    needs = newNeed;
                }
            }
            Console.WriteLine(fuelProduced);
        }

        static string[] GetInputFromFile()
        {
            var data = File.ReadAllLines("input.txt");
            return data;
        }
    }
}
