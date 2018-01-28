using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Starcounter;
using Starcounter.Linq;
using Database;

namespace Playground
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/bench/{?}/{?}/{?}/{?}", (int repeats, int threads, int count, int writesPercent) =>
            {
                StringBuilder sb = new StringBuilder();

                TotalTime(sb, repeats, threads, count, writesPercent);
                TransactionTime(sb, repeats, threads, count, writesPercent);

                return sb.ToString();
            });
        }

        public static void TotalTime(StringBuilder sb, int repeats, int threads, int count, int writesPercent)
        {
            for (int repeat = 0; repeat < repeats; repeat++)
            {
                List<Task> tasks = new List<Task>();
                Random rand = new Random();
                Stopwatch watch = new Stopwatch();

                Db.Transact(() =>
                {
                    DbLinq.Objects<Item>().DeleteAll();
                });

                watch.Start();

                for (int i = 0; i < threads; i++)
                {
                    int thread = i;

                    tasks.Add(Scheduling.RunTask(() =>
                    {
                        for (int j = 0; j < count; j++)
                        {
                            int r = rand.Next(0, 100);

                            if (r > writesPercent)
                            {
                                Item item = DbLinq.Objects<Item>().FirstOrDefault(x => x.Thread == thread && x.Index < j);
                            }
                            else
                            {
                                Db.Transact(() =>
                                {
                                    new Item() { Date = DateTime.Now, Guid = Guid.NewGuid().ToString(), Index = j, Thread = thread };
                                });
                            }
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());

                watch.Stop();

                sb.Append($"Completed in {watch.Elapsed} for {threads} thread(s), {count} item(s) per thread, {writesPercent}% writes.");
                sb.Append(Environment.NewLine);

                System.Threading.Thread.CurrentThread.Join(1000);
            }
        }

        public static void TransactionTime(StringBuilder sb, int repeats, int threads, int count, int writesPercent)
        {
            for (int repeat = 0; repeat < repeats; repeat++)
            {
                List<Task> tasks = new List<Task>();
                Random rand = new Random();
                ConcurrentBag<long> times = new ConcurrentBag<long>();

                Db.Transact(() =>
                {
                    DbLinq.Objects<Item>().DeleteAll();
                });

                for (int i = 0; i < threads; i++)
                {
                    int thread = i;

                    tasks.Add(Scheduling.RunTask(() =>
                    {
                        for (int j = 0; j < count; j++)
                        {
                            int r = rand.Next(0, 100);

                            if (r > writesPercent)
                            {
                                Item item = DbLinq.Objects<Item>().FirstOrDefault(x => x.Thread == thread && x.Index < j);
                            }
                            else
                            {
                                Stopwatch watch = new Stopwatch();

                                watch.Start();

                                Db.Transact(() =>
                                {
                                    new Item() { Date = DateTime.Now, Guid = Guid.NewGuid().ToString(), Index = j, Thread = thread };
                                });

                                watch.Start();
                                times.Add(watch.ElapsedTicks);
                            }
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());

                long time = (long)times.Average();

                sb.Append($"Completed with {time} ticks per write transaction for {threads} thread(s), {count} item(s) per thread, {writesPercent}% writes.");
                sb.Append(Environment.NewLine);

                System.Threading.Thread.CurrentThread.Join(1000);
            }
        }
    }
}