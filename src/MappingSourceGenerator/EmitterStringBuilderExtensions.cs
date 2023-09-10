using System.Text;

namespace MappingSourceGenerator;

public static class EmitterStringBuilderExtensions
{
    public static StringBuilder AppendCollectionWithSeparator(
        this StringBuilder stringBuilder,
        IReadOnlyList<string> values,
        char separator)
    {
        for (var index = 0; index < values.Count; index++)
        {
            var containingName = values[index];

            if (index != 0)
            {
                stringBuilder.Append(separator);
            }
            
            stringBuilder.Append(containingName);
        }

        return stringBuilder;
    }
    
    public static StringBuilder AppendRepeated(
        this StringBuilder stringBuilder,
        string value,
        int count)
    {
        for (var index = 0; index < count; index++)
        {
            stringBuilder.Append(value);
        }

        return stringBuilder;
    }
}