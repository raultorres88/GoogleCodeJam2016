using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;


namespace GoogleCodeJam2016.CountingSheep
{
    public class CountingSheepCase
    {
        public int N;
        public string Output;
        public int CaseNumber;
        private static Logger logger = LogManager.GetCurrentClassLogger();

    }

    public static class CountingSheepCaseHandler
    {
        public static IEnumerable<CountingSheepCase> GetCases(string filePath)
        {
            var cases = new List<CountingSheepCase>();

            if (!File.Exists(filePath))
                throw new Exception("Can't find file");

            string[] text = File.ReadAllLines(filePath);

            var lines = int.Parse(text[0]);

            for (var i = 1; i <= lines; i++)
            {
                cases.Add(new CountingSheepCase
                {
                    N = int.Parse(text[i]),
                    CaseNumber = i
                });
            }

            return cases;
        }

        public static void ProcessCases(IEnumerable<CountingSheepCase> cases)
        {
            foreach (var countingSheepCase in cases)
            {
                var tiredSheep = new TiredSheep();

                countingSheepCase.Output = tiredSheep.GetSleepNumber(countingSheepCase.N);
            }
        }

        public static void PrintCaesResults(IEnumerable<CountingSheepCase> cases)
        {
            var countingSheepCases = cases as CountingSheepCase[] ?? cases.ToArray();

            var output = new string[countingSheepCases.Count()];

            foreach (var tiredSheepCase in countingSheepCases.OrderBy(x => x.CaseNumber))
            {
                output[tiredSheepCase.CaseNumber -1] = string.Format("Case #{0}: {1}", tiredSheepCase.CaseNumber,
                    tiredSheepCase.Output);
            }

            File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "CountingSheepResults.txt"), output);
        }
    }

    public class NumberLogger
    {
        public bool[] Numbers = new bool[10];

        public bool IsAsleep()
        {
            return !Numbers.Contains(false);
        }

        public void LogNumber(int i)
        {
            Numbers[i] = true;
        }
    }

    public class TiredSheep
    {
        public static int MAX_EVALUATOR = 100;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string GetSleepNumber(int n)
        {
            logger.Debug("Starting {0}", n);

            NumberLogger numberLogger = new NumberLogger();

            for (var i = 1; i < MAX_EVALUATOR; i++)
            {
                int currentNumber = i*n;

                foreach (var number in currentNumber.ToString())
                {
                    numberLogger.LogNumber((int)Char.GetNumericValue(number));
                }

                if (numberLogger.IsAsleep())
                {
                    logger.Debug("Finished {0}, itterated {1} times", n, i);
                    return currentNumber.ToString();
                }

            }

            logger.Debug("Finished {0}, itterated {1} times", n, MAX_EVALUATOR);

            return "INSOMNIA";
        }
        
    }


    [TestFixture]
    public class CountingSheepUnitTests
    {

        [Test]
        public void NumberLoggerIsAsleep()
        {
            var numberLogger = new NumberLogger();

            // Set up the scenario where the asleep would be true

            for (var i = 0; i < 10; i ++)
            {
                numberLogger.LogNumber(i);
            }

            var results = numberLogger.IsAsleep();
            Assert.IsTrue(results);
        }

        [Test]
        public void NumberLoggerIsNotAsleep()
        {
            var numberLogger = new NumberLogger();

            // Set up the scenario where the asleep would be true

            for (var i = 0; i < 9; i++)
            {
                numberLogger.LogNumber(i);
            }

            var results = numberLogger.IsAsleep();
            Assert.IsFalse(results);
        }

        [TestCase(1, "10")]
        [TestCase(2, "90")]
        [TestCase(11, "110")]
        [TestCase(1692, "5076")]
        [TestCase(1234567890, "1234567890")]
        [TestCase(0, "INSOMNIA")]
        public void TesttiredSheep(int n, string expectedResult)
        {
            var tiredSheep = new TiredSheep();

            var result = tiredSheep.GetSleepNumber(n);

            Assert.AreEqual(expectedResult, result);

        }

        [TestCase(1000000)]
        [TestCase(9999999)]
        [TestCase(999999)]
        public void TesttiredSheepMaxTest(int n)
        {
            var tiredSheep = new TiredSheep();

            var result = tiredSheep.GetSleepNumber(n);

            Console.WriteLine(result);
        }

        [Test]
        public void SmallDatasetTest()
        {
            for (var i = 101; i > -1; i --)
            {
                var tiredSheep = new TiredSheep();

                var result = tiredSheep.GetSleepNumber(200 - i);
            }
        }

        [Test]
        public void LargeDatasetTest()
        {
            for (var i = 101; i > -1; i--)
            {
                var tiredSheep = new TiredSheep();

                var result = tiredSheep.GetSleepNumber(1000000 - i);
            }
        }

    }
}
