using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadCancellation
{
    class Program
    {
        static void Main(string[] args)
        {

            // .NET 4,5
            //prima via per avviare i Task
            //Task.Run(() => Print());
            //Task.Run(() => Print());

            var cts = new CancellationTokenSource();

            //Altro metodo
            // Questo overload è esattamente quello che fa Task.Run();
            Task<int> t1 = Task.Factory.StartNew(() => Print(true, cts.Token),
                cts.Token,// Impossibile annullare il token
                TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning,//impossibile creare Task figli
                TaskScheduler.Default); //usa lo scheduling di default

            Task<int> t2 = Task.Factory.StartNew(() => Print(false, cts.Token), cts.Token);

            Task t3 = new Task(() => Print(true, cts.Token), cts.Token);
            Thread.Sleep(10);
            cts.Cancel();

            Console.WriteLine($"t1 Stato:{t1.Status}");
            Console.WriteLine($"t2 Stato:{t2.Status}");
            Console.WriteLine($"t3 Stato:{t3.Status}");

            Thread.Sleep(1000);

            Console.WriteLine($"t1 Stato:{t1.Status}");
            Console.WriteLine($"t2 Stato:{t2.Status}");
            Console.WriteLine($"t3 Stato:{t3.Status}");

            Console.ReadLine();

        }


        private static int Print(bool isEven, CancellationToken token)
        {
            try
            {
                int total = 0;
                Console.WriteLine($"id={Task.CurrentId}. Is thread pool thread {Thread.CurrentThread.IsThreadPoolThread}");
                if (isEven)
                {
                    for (int i = 2; i < 100; i += 2)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("E' richiesto un annullamento");
                            token.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine($"CurrentTask id={Task.CurrentId}. Value={i}.");
                        total++;
                    }
                }
                else
                {
                    for (int i = 1; i < 100; i += 2)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("E' richiesto un annullamento");
                            token.ThrowIfCancellationRequested();
                        }
                        Console.WriteLine($"CurrentTask id={Task.CurrentId}. Value={i}.");
                        total++;
                    }
                }
                return total;
            }
            catch (OperationCanceledException ex)
            {
                return -1;
            }
        }
    }
}
