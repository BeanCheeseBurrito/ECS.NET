using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

[assembly: SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 10)]
[assembly: HardwareCounters(HardwareCounter.CacheMisses)]

namespace ECS.NET.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher benchmark = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);

            if (args.Length > 0)
                benchmark.Run(args);
            else
                benchmark.RunAll();
        }
    }
}
