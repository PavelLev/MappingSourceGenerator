namespace MappingSourceGenerator.Markers;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class GenerateMappingAttribute(params string[] usableMapMethodNames) : Attribute
{
    /// <summary>
    /// Map method names to look through before attempting to generate own mapping.
    /// </summary>
    public string[] UsableMapMethodNames { get; set; } = usableMapMethodNames;
}