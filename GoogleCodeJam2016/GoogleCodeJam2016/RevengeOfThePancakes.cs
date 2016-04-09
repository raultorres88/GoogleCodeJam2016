using System;
using System.Collections.Generic;
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
            foreach (var pancakeStack in pancakeStacks)
            {
                if (pancakeStack.ReadyToServe())
                {
                    logger.Debug("Found best stack, {0} optimal flips", pancakeStack.Depth);
                    return pancakeStack;
                }
            }

            // Not Good stack, so let's start flipping each stack we have (breadth first)
            foreach (var pancakeStack in pancakeStacks)
            {
                // Keeping track of stacks that have already generated children stacks 
                Hashes.Add(pancakeStack.GetHash());

                // Flip all possible ways
                for (var i = 1; i <= pancakeStack.Stack.Length; i++)
                {
                    var flippedStack = PancakeFlipper.Flip(i, pancakeStack);

                    // If this is the first time we see the stack, we'll check out all it's options
                    if(!Hashes.Contains(flippedStack.GetHash()))
                        NextStacksToCheck.Add(flippedStack);
                }
            }


            return FindOptimalStack(NextStacksToCheck);
        }

    }


    public class PancakeStack
    {
        public int Depth;
        public bool[] Stack;

        public PancakeStack(string input)
        {
            Depth = 0;      // new stacks start at 0

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

        //[TestCase(10)]
        //[TestCase(100)]
        public void OptimalFlipsBigDataSet(int length)
        {
            var pancakeStack = new PancakeStack(new String('-', length));

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
