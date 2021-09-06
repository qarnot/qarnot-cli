namespace QarnotCLI.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using QarnotCLI;

    [TestFixture]
    [Author("NEBIE Guillaume Lale", "guillaume.nebie@qarnot-computing.com")]
    public class TestIEnumerableExtention
    {
        public class TestParallelObject
        {
        }

        public async Task<string> TestParallelFunctionIntToString(int source, TestParallelObject resources, CancellationToken ct)
        {
            await Task.Delay(100);
            return source.ToString();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public async Task ParallelForEachAsyncReturnTheGoodValue(int count)
        {
            List<int> lst = new List<int>();
            for (int value = 0; value < count; value++)
            {
                lst.Add(value);
            }

            IEnumerable<string> lstCheck = lst.Select(x => x.ToString());

            var lstResult = await lst.ParallelForEachAsync(TestParallelFunctionIntToString, new TestParallelObject());

            CollectionAssert.AreEqual(lstResult.OrderBy(p => p), lstCheck.OrderBy(p => p));
        }

        public async Task<string> TestParallelFunctionWait(int source, int waitTime, CancellationToken ct)
        {
            await Task.Delay(waitTime);
            return "foo";
        }

        [Test]
        [TestCase(10, 100, 10)]
        [TestCase(100, 100, 10)]
        [TestCase(10, 100, 100)]
        [TestCase(50, 100, 100)]
        [TestCase(50, 100, 10)]
        public async Task ParallelForEachAsyncWaitEveryXElement(int count, int waitTime, int maxRepeat)
        {
            List<int> lst = new List<int>();
            for (int value = 0; value < count; value++)
            {
                lst.Add(value);
            }

            IEnumerable<string> lstTest = lst.Select(x => x.ToString());
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var ret = await lst.ParallelForEachAsync(TestParallelFunctionWait, waitTime, maxDoP: maxRepeat);
            stopwatch.Stop();
            TimeSpan diffDate = stopwatch.Elapsed;
            int minTime = waitTime * (count / maxRepeat);
            int maxTime = waitTime * count;
            Assert.Greater(diffDate.TotalMilliseconds, minTime * 0.95);
            Assert.Less(diffDate.TotalMilliseconds, maxTime * 1.05);
        }
    }
}
