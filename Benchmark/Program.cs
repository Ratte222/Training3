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
            BenchmarkRunner.Run<UpdateBenchmark>();
        }
    }
}
