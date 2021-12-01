using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using System;

namespace Benchmark
{
    class Program
    {
        public const string connection = "server=localhost;user=artur;password=12345678;database=trainingdb3; AllowPublicKeyRetrieval=True;";
        static void Main(string[] args)
        {
            //UpdateBenchmark updateBenchmark = new UpdateBenchmark();
            //updateBenchmark.GlobalSetup();
            //var schoolClasses = updateBenchmark.Select();
            //updateBenchmark.Change(schoolClasses[0]);
            //updateBenchmark.MyUpdateTest(schoolClasses[0]);
            //updateBenchmark.EFCoreUpdateTest(schoolClasses[0]);
            //updateBenchmark.MyUpdate();
            //updateBenchmark.EFCoreUpdate();
            BenchmarkRunner.Run<UpdateBenchmark>();
        }
    }
}
