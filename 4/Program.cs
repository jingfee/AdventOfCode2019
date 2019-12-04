using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AoC._2019._4
{
    class Program
    {
        static void Main(string[] args)
        {
            var lowerBound = 372037;
            var upperBound = 905157;
            var foundPasswords = 0;
            for (var password = lowerBound; password <= upperBound; password++)
            {
                if (IsValidPassword(password))
                {
                    foundPasswords++;
                }
            }

            Console.WriteLine(foundPasswords);
        }

        static bool IsValidPassword(int password)
        {
            var passwordString = password.ToString();

            var foundPair = false;
            for (var i = 0; i < (passwordString.Length - 1); i++)
            {
                if (passwordString[i] != passwordString[i + 1])
                {
                    continue;
                }

                if (i == 0)
                {
                    if (passwordString[i] == passwordString[i + 2])
                    {
                        continue;
                    }
                }
                else if (i + 2 == passwordString.Length)
                {
                    if (passwordString[i] == passwordString[i - 1])
                    {
                        continue;
                    }
                }
                else
                {
                    if (passwordString[i] == passwordString[i - 1] || passwordString[i] == passwordString[i + 2])
                    {
                        continue;
                    }
                }

                foundPair = true;
                break;
            }


            var allIncreasing = true;
            for (var i = 1; i < passwordString.Length; i++)
            {
                if (passwordString[i - 1] > passwordString[i])
                {
                    allIncreasing = false;
                    break;
                }
            }

            return foundPair && allIncreasing;
        }
    }
}
