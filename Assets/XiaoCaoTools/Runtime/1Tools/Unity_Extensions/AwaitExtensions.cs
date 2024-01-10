#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace GG.Extensions
{
    public static class AwaitExtensions
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if(waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            #if UNITY_WEBGL
            while (!condition.Invoke())
            {
                await new WaitForSeconds(frequency / 1000);
            }
            #else
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, 
                    Task.Delay(timeout))) 
                throw new TimeoutException();
#endif
        }
        
        // Adapted from https://blogs.msdn.microsoft.com/pfxteam/2012/03/05/implementing-a-simple-foreachasync-part-2/
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> body,
                                           IProgress<T> progress = null)
        {
            return Task.WhenAll
            (
                Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                           .Select(partition => Task.Run(async () =>
                           {
                               using (partition)
                               {
                                   while (partition.MoveNext())
                                   {
                                       await body(partition.Current);
                                       progress?.Report(partition.Current);
                                   }
                               }
                           }))
            );
        }
        
        /// <summary>
        /// Yield for all events to complete passing back the index from the list
        /// </summary>
        /// <param name="source"></param>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="body"></param>
        /// <param name="progress"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, int, Task> body, IProgress<T> progress = null)
        {
            int index = 0;
            return Task.WhenAll
            (
                Partitioner.Create(source).GetPartitions(degreeOfParallelism).Select(partition => Task.Run(async () =>
               {
                   using (partition)
                   {
                       while (partition.MoveNext())
                       {
                           index++;
                           await body(partition.Current, index);
                           progress?.Report(partition.Current);
                       }
                   }
               }))
            );
        }
    }
}
