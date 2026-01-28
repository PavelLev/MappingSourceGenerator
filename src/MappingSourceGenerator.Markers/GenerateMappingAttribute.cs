using System.Diagnostics;

namespace MappingSourceGenerator.Markers;

[Conditional("MAPPING_SOURCE_GENERATOR_USAGES")]
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class GenerateMappingAttribute(params string[] usableMapMethodNames) : Attribute
{
    /// <summary>
    /// Map method names to look through before attempting to generate own mapping.
    /// </summary>
    public string[] UsableMapMethodNames { get; set; } = usableMapMethodNames;
}