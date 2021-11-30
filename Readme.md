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
  Job-XLRVLZ : .NET 5.0.11 (5.0.1121.47308), X64 RyuJIT

IterationCount=1000  RunStrategy=ColdStart  

```
|            Method |      Mean |    Error |    StdDev |   Median |      Min |        Max | Allocated |
|------------------ |----------:|---------:|----------:|---------:|---------:|-----------:|----------:|
|      EFCoreUpdate |  5.843 ms | 4.059 ms | 38.895 ms | 4.528 ms | 3.571 ms | 1,234.4 ms |     81 KB |
|          MyUpdate | 10.788 ms | 4.168 ms | 39.940 ms | 6.640 ms | 5.248 ms | 1,216.8 ms |    369 KB |
| EFCoreUpdateRange |  6.131 ms | 3.799 ms | 36.405 ms | 4.912 ms | 4.082 ms | 1,156.0 ms |     95 KB |
|     MyUpdateRange | 12.536 ms | 4.097 ms | 39.258 ms | 8.742 ms | 6.990 ms | 1,205.7 ms |    659 KB |
# Benchmarck result (100 iteration)

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1348 (20H2/October2020Update)
Intel Core i5-3570K CPU 3.40GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 5.0.11 (5.0.1121.47308), X64 RyuJIT
  Job-XANXAH : .NET 5.0.11 (5.0.1121.47308), X64 RyuJIT

IterationCount=100  RunStrategy=ColdStart  

```
|            Method |     Mean |    Error |    StdDev |   Median |      Min |        Max | Allocated |
|------------------ |---------:|---------:|----------:|---------:|---------:|-----------:|----------:|
|      EFCoreUpdate | 28.46 ms | 43.78 ms | 129.08 ms | 4.847 ms | 3.863 ms | 1,238.7 ms |     81 KB |
|          MyUpdate | 19.26 ms | 39.76 ms | 117.23 ms | 7.387 ms | 5.258 ms | 1,179.8 ms |    369 KB |
| EFCoreUpdateRange | 17.16 ms | 38.90 ms | 114.68 ms | 5.545 ms | 4.224 ms | 1,152.4 ms |     94 KB |
|     MyUpdateRange | 21.27 ms | 39.12 ms | 115.33 ms | 9.396 ms | 7.207 ms | 1,163.0 ms |    659 KB |
