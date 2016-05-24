using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleCodeJam2016.CountingSheep;
using GoogleCodeJam2016.RevengeOfThePancakes;

namespace GoogleCodeJam2016
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args[0].ToLower() == "sheep")
            {
                var cases = CountingSheep.CountingSheepCaseHandler.GetCases(args[1]);
                var countingSheepCases = cases as CountingSheepCase[] ?? cases.ToArray();
                CountingSheep.CountingSheepCaseHandler.ProcessCases(countingSheepCases);
                CountingSheep.CountingSheepCaseHandler.PrintCaesResults(countingSheepCases);
            }

            else if (args[0].ToLower() == "pancakes")
            {
                var cases = RevengeOfThePancakes.RevengeOfThePancakesCaseHandler.GetCases(args[1]);
                var revengeOfThePancakesCases = cases as RevengeOfThePancakesCase[] ?? cases.ToArray();
                RevengeOfThePancakes.RevengeOfThePancakesCaseHandler.ProcessCases(revengeOfThePancakesCases);
                RevengeOfThePancakes.RevengeOfThePancakesCaseHandler.PrintCaesResults(revengeOfThePancakesCases);
            }



        }
    }
}
