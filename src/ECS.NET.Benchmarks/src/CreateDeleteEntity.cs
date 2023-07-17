using BenchmarkDotNet.Attributes;
using ECS.NET.Core;

namespace ECS.NET.Benchmarks
{
    public class CreateDeleteEntityBenchmark
    {
        private World _world;

        [Params(100000)] public int Count;

        [GlobalSetup]
        public void Setup()
        {
            _world = World.Init();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _world.Dispose();
        }

        [Benchmark]
        public Entity CreateDeleteEntity()
        {
            Entity id = default;
            for (int i = 0; i < Count; i++)
            {
                id = _world.Entity();
                id.Delete();
            }

            return id;
        }
    }
}
