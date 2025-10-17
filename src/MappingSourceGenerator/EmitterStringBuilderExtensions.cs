using System.Text;

namespace MappingSourceGenerator;

public static class EmitterStringBuilderExtensions
{
    public static StringBuilder AppendCollectionWithSeparator(
        this StringBuilder stringBuilder,
        string[] values,
        char separator)
    {
        for (var index = 0; index < values.Length; index++)
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

    public static StringBuilder AppendMappingModel(
        this StringBuilder stringBuilder,
        MappingModel model)
    {
        stringBuilder
            .AppendCollectionWithSeparator(model.ContainingNames, '.')
            .Append('.')
            .Append(model.Name);

        var isGeneric = model.TypeArguments.Length != 0;

        if (!isGeneric)
        {
            return stringBuilder;
        }

        stringBuilder.Append('<');

        for (var index = 0; index < model.TypeArguments.Length; index++)
        {
            if (index != 0)
            {
                stringBuilder.Append(", ");
            }
            
            var typeArgument = model.TypeArguments[index];
            stringBuilder.AppendMappingModel(typeArgument);
        }

        stringBuilder.Append('>');
        
        
        return stringBuilder;
    }
}