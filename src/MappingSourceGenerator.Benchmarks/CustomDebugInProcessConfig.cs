using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace MappingSourceGenerator.Benchmarks;

public class CustomDebugInProcessConfig : DebugConfig
{
    private readonly Func<Job, Job> _customModify;

    public CustomDebugInProcessConfig(Func<Job, Job> customModify)
    {
        _customModify = customModify;
    }
    
    public override IEnumerable<Job> GetJobs() => new Job[]
    {
        _customModify(JobMode<Job>.Default)
            .WithToolchain(new InProcessEmitToolchain(TimeSpan.FromHours(1.0), true))
    };
}