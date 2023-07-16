using BenchmarkDotNet.Running;

BenchmarkSwitcher benchmark = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);

if (args.Length > 0)
    benchmark.Run(args);
else
    benchmark.RunAll();