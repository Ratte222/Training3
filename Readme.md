# What is done
- added service "QueryService" 
    - a sql query with nesting (Include) and condition is written
    - a linq query with nesting (Include) and condition is written
    - added function to update nested entity
    - work with reflection
- added benchmark for comparisons my function update with default function update EF Core

# Benchmarck result (1000 iteration)
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1348 (20H2/October2020Update)
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.11 (5.0.1121.47308), X64 RyuJIT
  Job-ULXKKJ : .NET 5.0.11 (5.0.1121.47308), X64 RyuJIT

IterationCount=1000  RunStrategy=ColdStart  

```
|            Method |      Mean |    Error |    StdDev |   Median |      Min |        Max | Allocated |
|------------------ |----------:|---------:|----------:|---------:|---------:|-----------:|----------:|
|      EFCoreUpdate |  6.385 ms | 4.124 ms | 39.513 ms | 4.941 ms | 3.636 ms | 1,253.8 ms |     83 KB |
|          MyUpdate |  8.124 ms | 4.043 ms | 38.738 ms | 6.710 ms | 5.354 ms | 1,231.3 ms |    369 KB |
| EFCoreUpdateRange |  9.500 ms | 4.266 ms | 40.871 ms | 5.472 ms | 4.488 ms | 1,227.6 ms |     96 KB |
|     MyUpdateRange | 13.138 ms | 4.271 ms | 40.921 ms | 9.025 ms | 7.604 ms | 1,202.6 ms |    659 KB |
