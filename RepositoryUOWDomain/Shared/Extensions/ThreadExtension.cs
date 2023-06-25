using System.Collections.Concurrent;
using RepositoryUOWDomain.Shared.Common;

namespace RepositoryUOWDomain.Shared.Extensions;

public static class ThreadExtension
{
    public static async Task RunExTask<T>(this Action<T> act, IEnumerable<T> list, CancellationToken token = default)
    {
        var throttler = new SemaphoreSlim(Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0)));

        var repLists = list.Chunk(10000).AsParallel().WithDegreeOfParallelism(
                      Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0))).ToList();

        var tasks = from partition in repLists
                .Select((jobs, i) => new { jobs, i })
                    select Task.Run(async () =>
                    {
                        try
                        {
                            await throttler.WaitAsync(token);

                            Partitioner.Create(partition.jobs, true).AsParallel().ForAll(act);
                            //if (BusyIndicatorModel.CancellationTokenSource.IsCancellationRequested) return;
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }, token);

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public static async Task RunExTask<T>(this Func<T, Task> act, T[] list, int procUsed = -1, CancellationToken token = default)
    {
        int processorUsed = procUsed > 0 ? procUsed : Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));

        int chunkCount = (processorUsed > list.Length) ? 1 : list.Length / processorUsed;

        var tasks = from part in Partitioner.Create(list.ToList(), true)
            .AsParallel().WithDegreeOfParallelism(processorUsed).Chunk(chunkCount)
            //.Select((jobs, i) => new {jobs, i})
                    select 
                        Task.Run(async () => { foreach (var f in part.AsParallel()) await act(f); }, token);
        try
        {
            await Task.WhenAll(tasks.ToList()).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public static async Task RunExTask<T>(this T[] list, Func<T, Task> act, int procUsed = -1, CancellationToken token = default)
    {
        await act.RunExTask(list, procUsed, token: token).ConfigureAwait(false);
    }


    public static async Task RunExTask<T>(this ConcurrentBagAsync<T> list, Func<T, Task> act, int procUsed = -1, CancellationToken token = default)
    {
        int processorUsed = procUsed > 0 ? procUsed : Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));

        //int chunkCount = (processorUsed > list.Count) ? 1 : Convert.ToInt32(Math.Ceiling((decimal)list.Count/processorUsed));

        int chunkCount = (processorUsed > list.Count) ? 1 : Convert.ToInt32(Math.Ceiling((decimal)list.Count / 1000));

        var tasks = from part in Partitioner.Create(list.ToList(), true)
                .AsParallel().Chunk(chunkCount)
            //.Select((jobs, i) => new {jobs, i})
            select
                Task.Run(  async () =>
                {
                    //try
                    //{
                    //    part.AsParallel().WithDegreeOfParallelism(processorUsed).ForAll( f => act(f));
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //    throw;
                    //}
                    // ReSharper disable once AsyncVoidLambda
                    
                    //Parallel.ForEach(part, async f => await act(f));
                    foreach (var f in part.AsParallel()) await act(f);

                    //await Task.WhenAll(part.Select(f => Task.Run(() => act(f), token)));
                }, token);
        try
        {
            await Task.WhenAll(tasks.ToList()).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }


    public static async Task RunExTaskSecond<T>(this ConcurrentBagAsync<T> list, Func<T, Task> act, CancellationToken token = default)
    {
        var throttler = new SemaphoreSlim(Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)));

        var repLists = list.Chunk(1000).AsParallel().WithDegreeOfParallelism(
            Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0))).ToList();

        var tasks = repLists.Select(async chunks =>
        {
            await throttler.WaitAsync(token);
            try
            {
                foreach (var f in chunks.AsParallel()) await act(f);
            }
            finally
            {
                throttler.Release();
            }
        }).ToList();

        try
        {
            await Task.WhenAll(tasks.ToList()).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }




    

    public static async Task SelectAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> selector, int maxDegreesOfParallelism = -1)
    {
        maxDegreesOfParallelism = maxDegreesOfParallelism > 0 ? maxDegreesOfParallelism : Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));

        var activeTasks = new HashSet<Task>();
        foreach (var item in source)
        {
            activeTasks.Add(selector(item));
            if (activeTasks.Count >= maxDegreesOfParallelism)
            {
                var completed = await Task.WhenAny(activeTasks);
                activeTasks.Remove(completed);
            }
        }

        await Task.WhenAll(activeTasks);
    }
}