namespace MappingSourceGenerator.Markers;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class GenerateMappingAttribute : Attribute
{
}