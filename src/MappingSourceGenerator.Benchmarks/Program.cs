using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MappingSourceGenerator.Benchmarks;

var config = default(IConfig);
// config = new CustomDebugInProcessConfig(_ => _.WithInvocationCount(2048).WithWarmupCount(8).WithIterationCount(16));

BenchmarkRunner.Run<Benchmarks>(config);