
namespace QarnotCLI
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class IEnumerableExtensions
    {
        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await funcBody(partition.Current);
                    }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p)));
        }

        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, CancellationToken, Task> funcBody, int maxDoP = 4, CancellationToken ct = default(CancellationToken))
        {
            async Task AwaitPartition(IEnumerator<T> partition, CancellationToken ct)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await funcBody(partition.Current, ct);
                    }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p, ct)));
        }

        public static async Task<IEnumerable<TResult>> ParallelForEachAsync<TSource, TResources, TResult>(this IEnumerable<TSource> source, Func<TSource, TResources, CancellationToken, Task<TResult>> funcBody, TResources resources, int maxDoP = 4, CancellationToken ct = default(CancellationToken))
        {
            async Task<IEnumerable<TResult>> AwaitPartition(IEnumerator<TSource> partition, TResources resources, CancellationToken ct)
            {
                List<TResult> tasks = new List<TResult>();
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        tasks.Add(await funcBody(partition.Current, resources, ct));
                    }
                }

                return tasks;
            }

            return (await Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(p => AwaitPartition(p, resources, ct))))
                    .SelectMany(i => i);
        }
    }
}
