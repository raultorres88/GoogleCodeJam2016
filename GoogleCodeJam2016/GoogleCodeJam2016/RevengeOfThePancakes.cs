using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;

namespace GoogleCodeJam2016.RevengeOfThePancakes
{
    public class RevengeOfThePancakesCase
    {
        public string Input;
        public int CaseNumber;
        public string Output;
    }

    public static class RevengeOfThePancakesCaseHandler
    {
        public static IEnumerable<RevengeOfThePancakesCase> GetCases(string filePath)
        {
            var cases = new List<RevengeOfThePancakesCase>();

            if (!File.Exists(filePath))
                throw new Exception("Can't find file");

            string[] text = File.ReadAllLines(filePath);

            var lines = int.Parse(text[0]);

            for (var i = 1; i <= lines; i++)
            {
                cases.Add(new RevengeOfThePancakesCase
                {
                    Input = text[i],
                    CaseNumber = i
                });
            }

            return cases;
        }

        public static void ProcessCases(IEnumerable<RevengeOfThePancakesCase> cases)
        {
            foreach (var revengeOfThePancakesCase in cases)
            {
                var pancakeStack = new PancakeStack(revengeOfThePancakesCase.Input);

                var flipFinder = new FlipFinder();

                var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> { pancakeStack });
                revengeOfThePancakesCase.Output = bestStack.Depth.ToString();
            }
        }

        public static void PrintCaesResults(IEnumerable<RevengeOfThePancakesCase> cases)
        {
            var revengeOfThePancakesCases = cases as RevengeOfThePancakesCase[] ?? cases.ToArray();

            var output = new string[revengeOfThePancakesCases.Count()];

            foreach (var revengeOfThePancakesCase in revengeOfThePancakesCases.OrderBy(x => x.CaseNumber))
            {
                output[revengeOfThePancakesCase.CaseNumber - 1] = string.Format("Case #{0}: {1}", revengeOfThePancakesCase.CaseNumber,
                    revengeOfThePancakesCase.Output);
            }

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "PancakeResults.txt"), output);
        }
    }


    public class FlipFinder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<string> Hashes = new List<string>();
    
        public PancakeStack FindOptimalStack(IEnumerable<PancakeStack> stacks)
        {
            var pancakeStacks = stacks as PancakeStack[] ?? stacks.ToArray();
            logger.Debug("Evaluating {0} stacks", pancakeStacks.Length);

            // Reach end of recursion if there are no more options to check
            if (!pancakeStacks.Any())
                return null;

            var NextStacksToCheck = new List<PancakeStack>();

            // Return any if we are ready

            var minPartiontion = pancakeStacks[0].Stack.Length;
            foreach (var pancakeStack in pancakeStacks.OrderBy(x => x.GetPartitions()))
            {
                if (pancakeStack.ReadyToServe())
                {
                    logger.Debug("Found best stack, {0} optimal flips, Orderd {1}", pancakeStack.Depth, pancakeStack.FlipOrder);
                    return pancakeStack;
                }

                if (pancakeStack.GetPartitions() < minPartiontion)
                    minPartiontion = pancakeStack.GetPartitions();
            }

            logger.Debug("Minimum partions is {0}", minPartiontion);

            // Not Good stack, so let's start flipping each stack we have that is a step in the correct direction
            foreach (var pancakeStack in pancakeStacks.Where(x => x.GetPartitions() == minPartiontion))
            {

                // Keeping track of stacks that have already generated children stacks 
                Hashes.Add(pancakeStack.GetHash());

                // Flip all possible ways
                for (var i = 1; i <= pancakeStack.Stack.Length; i++)
                {
                    var flippedStack = PancakeFlipper.Flip(i, pancakeStack);

                    // More Entropy probably means we aren't going in the right direction
                    if (flippedStack.GetPartitions() > pancakeStack.GetPartitions())
                        continue; 

                    // If this is the first time we see the stack, we'll check out all it's options
                    if (!Hashes.Contains(flippedStack.GetHash()))
                    { 
                        NextStacksToCheck.Add(flippedStack);
                        break;
                    }
                }
            }


            return FindOptimalStack(NextStacksToCheck);
        }

    }

    public class PancakeStack
    {
        public int Depth;
        public bool[] Stack;
        public string FlipOrder;

        public PancakeStack(string input)
        {
            Depth = 0;      // new stacks start at 0
            FlipOrder = string.Empty;

            Stack = new bool[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '-')
                    Stack[i] = false;
                else
                    Stack[i] = true;
            }

        }

        public PancakeStack(bool[] input)
        {
            Stack = new bool[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                Stack[i] = input[i];
            }

        }

        public string GetHash()
        {
            return string.Join("", Stack.Select(x => x ? "1" : "0").ToArray()); ;
        }

        public int GetPartitions()
        {
            var partions = 1;
            
            var currentPartition = Stack[0];

            for (int index = 0; index < Stack.Length; index++)
            {
                if (Stack[index] != currentPartition)
                {
                    partions++;
                    currentPartition = Stack[index];
                }
            }

            return partions;
        }

        public bool ReadyToServe()
        {
            return !Stack.Any(x => x == false);
        }
    }

    public static class PancakeFlipper
    {
        public static PancakeStack Flip(int fromTheTop, PancakeStack originalStack)
        {
            var newStack = new PancakeStack(originalStack.Stack);

            // Increade the depth, or flip count.... thinking about a tree for now, so keeping "depth" verbage
            newStack.Depth = originalStack.Depth + 1;
            newStack.FlipOrder = originalStack.FlipOrder + " -" + fromTheTop + "- ";

            // Change the order
            var flipped = newStack.Stack.Take(fromTheTop).ToArray();

            // flip each over
            for (var i = 0; i < flipped.Length; i++)
            {
                newStack.Stack[i] = !flipped[i];
            }

            return newStack;
        }
    }

    [TestFixture]
    public class RevengeOfThePancakesTests
    {
        [TestCase("-", 1)]
        [TestCase("-+", 1)]
        [TestCase("+-", 2)]
        [TestCase("+++", 0)]
        [TestCase("--+-", 3)]
        public void OptimalFlips(string input, int expectedDepth)
        {
            var pancakeStack = new PancakeStack(input);

            var flipFinder = new FlipFinder();

            var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> {pancakeStack});

            Assert.IsNotNull(bestStack);
            Assert.AreEqual(expectedDepth, bestStack.Depth);
        }

        [TestCase(10)]
        [TestCase(100)]
        public void OptimalFlipsBigDataSet(int down)
        {
            var pancakeStack = new PancakeStack(new String('-', down));

            var flipFinder = new FlipFinder();

            var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> { pancakeStack });
        }

        [TestCase(9, 1)]
        [TestCase(99, 1)]
        [TestCase(99, 1)]
        public void OptimalFlipsBigDataSet(int down, int up)
        {
            string setup = new String('-', down) + new String('+', up);
            Random rnd = new Random();

            var mixed = setup.OrderBy(x => rnd.Next()).ToArray();
            var mixedString = new string(mixed);
            Console.WriteLine(mixedString);
            var pancakeStack = new PancakeStack(mixedString);

            var flipFinder = new FlipFinder();

            var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> { pancakeStack });
        }

        [Test]
        public void benchMarkTeset()
        {
            var longString = "-------------------------------------------------+--------------------------------------------------";
            var pancakeStack = new PancakeStack(longString);

            var flipFinder = new FlipFinder();

            var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> { pancakeStack });
        }

        [Test]
        public void BenchmarkTest02()
        {
            var longString = "---------------+--------------------------------+---------------+---------++++--------+-----------+---";
            var pancakeStack = new PancakeStack(longString);

            var flipFinder = new FlipFinder();

            var bestStack = flipFinder.FindOptimalStack(new List<PancakeStack> { pancakeStack });
        }


        [TestCase("-", "0" )]
        [TestCase("+", "1")]
        [TestCase("-+", "01")]
        [TestCase("+-", "10")]
        [TestCase("+---", "1000")]
        [TestCase("-+++", "0111")]
        public void ConstructorTest(string input, string expectedHash)
        {
            var pancakeStack = new PancakeStack(input);

            Assert.AreEqual(expectedHash, pancakeStack.GetHash());
        }

        [TestCase("-", "1")]
        [TestCase("+", "0")]
        [TestCase("-+", "10")]
        [TestCase("+-", "01")]
        [TestCase("+---", "0111")]
        [TestCase("-+++", "1000")]
        public void FlipTest(string input, string expectedHash)
        {
            var pancakeStack = new PancakeStack(input);

            var newPancakeStack = PancakeFlipper.Flip(pancakeStack.Stack.Length, pancakeStack);

            Assert.AreEqual(expectedHash, newPancakeStack.GetHash());
            Assert.AreEqual(1, newPancakeStack.Depth);
        }

        [TestCase("-", "1")]
        [TestCase("+", "0")]
        [TestCase("-+", "11")]
        [TestCase("+-", "00")]
        [TestCase("+---", "0000")]
        [TestCase("-+++", "1111")]
        public void FlipTestTopOnly(string input, string expectedHash)
        {
            var pancakeStack = new PancakeStack(input);

            var newPancakeStack = PancakeFlipper.Flip(1, pancakeStack);

            Assert.AreEqual(expectedHash, newPancakeStack.GetHash());
            Assert.AreEqual(1, newPancakeStack.Depth);
        }

        [TestCase("-")]
        [TestCase("-+")]
        [TestCase("+-")]
        [TestCase("+---")]
        [TestCase("-+++")]
        public void NotReadyToServe(string input)
        {
            var pancakeStack = new PancakeStack(input);

            Assert.IsFalse(pancakeStack.ReadyToServe());
        }

        [TestCase("+")]
        [TestCase("++")]
        [TestCase("+++")]
        [TestCase("++++")]
        [TestCase("+++++++")]
        public void ReadyToServe(string input)
        {
            var pancakeStack = new PancakeStack(input);

            Assert.IsTrue(pancakeStack.ReadyToServe());
        }
    }
}
