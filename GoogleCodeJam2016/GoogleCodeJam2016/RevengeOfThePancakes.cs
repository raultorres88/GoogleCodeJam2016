using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GoogleCodeJam2016.RevengeOfThePancakes
{
    public class RevengeOfThePancakesCase
    {
        public string Input;
        public int CaseNumber;
        public string Output;
    }

    public class PancakeStack
    {
        public int Depth;
        public bool[] Stack;
        public int Hash;

        public PancakeStack(string input)
        {
            Hash = -1;

            Stack = new bool[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '-')
                    Stack[i] = false;
                else
                    Stack[i] = true;
            }

            Hash = GetHash();

        }

        public PancakeStack(bool[] input)
        {
            Hash = -1;
            Stack = new bool[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                Stack[i] = input[i];
            }

        }

        public int GetHash()
        {
            if (Hash > 0)
                return Hash;

            var statckAsString = string.Join("", Stack.Select(x => x ? "1" : "0").ToArray());

            Hash =  Convert.ToInt32(statckAsString, 2);

            return Hash;

        }
    }

    public static class PancakeFlipper
    {
        public static PancakeStack Flip(int fromTheTop, PancakeStack originalStack)
        {
            var newStack = new PancakeStack(originalStack.Stack);

            // Increade the depth
            newStack.Depth++;

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
        [TestCase("-", 0 )]
        [TestCase("+", 1)]
        [TestCase("-+", 1)]
        [TestCase("+-", 2)]
        [TestCase("+---", 8)]
        [TestCase("-+++", 7)]
        public void ConstructorTest(string input, int expectedHash)
        {
            var pancakeStack = new PancakeStack(input);

            Assert.AreEqual(expectedHash, pancakeStack.GetHash());
        }

        [TestCase("-", 1)]
        [TestCase("+", 0)]
        [TestCase("-+", 2)]
        [TestCase("+-", 1)]
        [TestCase("+---", 7)]
        [TestCase("-+++", 8)]
        public void FlipTest(string input, int expectedHash)
        {
            var pancakeStack = new PancakeStack(input);

            var newPancakeStack = PancakeFlipper.Flip(pancakeStack.Stack.Length, pancakeStack);

            Assert.AreEqual(expectedHash, newPancakeStack.GetHash());
        }
    }
}
