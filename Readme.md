# What is done
- added service "QueryService" 
    - a sql query with nesting (Include) and condition is written
    - a linq query with nesting (Include) and condition is written
- added function to update nested entity AuxiliaryLib.Extensions.QueryableExtensions  
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
|      EFCoreUpdate |  6.221 ms | 4.059 ms | 38.893 ms | 4.928 ms | 3.680 ms | 1,234.6 ms |     83 KB |
|          MyUpdate |  9.563 ms | 4.340 ms | 41.581 ms | 5.206 ms | 4.194 ms | 1,253.2 ms |    145 KB |
| EFCoreUpdateRange | 10.661 ms | 6.804 ms | 65.195 ms | 5.258 ms | 4.218 ms | 1,418.4 ms |     96 KB |
|     MyUpdateRange |  9.859 ms | 4.106 ms | 39.343 ms | 5.988 ms | 4.835 ms | 1,186.6 ms |    215 KB |

# Benchmarck

|      Method |Time building string |Time executing script |Common time executing |
|------------ |--------------------:|---------------------:|---------------------:|
| MyUpdate    |     157.557 ms      |   30.443 ms          | 188 ms               |
| EFCoreUpdate|     102.21 ms       |   29.79 ms           | 132 ms               |