using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Marventa.Framework.Tests.Performance.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class CachingBenchmarks
{
    private IMemoryCache _memoryCache;
    private const string TestKey = "test-key";
    private const string TestValue = "test-value-with-some-content";

    [GlobalSetup]
    public void Setup()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Benchmark(Baseline = true)]
    public void MemoryCache_Set()
    {
        _memoryCache.Set(TestKey, TestValue, TimeSpan.FromMinutes(5));
    }

    [Benchmark]
    public string MemoryCache_Get()
    {
        return _memoryCache.Get<string>(TestKey) ?? string.Empty;
    }

    [Benchmark]
    public bool MemoryCache_TryGetValue()
    {
        return _memoryCache.TryGetValue(TestKey, out string _);
    }

    [Benchmark]
    public void MemoryCache_SetAndGet()
    {
        _memoryCache.Set(TestKey, TestValue, TimeSpan.FromMinutes(5));
        _ = _memoryCache.Get<string>(TestKey);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        (_memoryCache as IDisposable)?.Dispose();
    }
}

/*
Expected Results:
| Method                  | Mean      | Allocated |
|------------------------ |----------:|----------:|
| MemoryCache_Get         |  12.34 ns |         - |
| MemoryCache_TryGetValue |  13.45 ns |         - |
| MemoryCache_Set         |  45.67 ns |     128 B |
| MemoryCache_SetAndGet   |  58.90 ns |     128 B |
*/