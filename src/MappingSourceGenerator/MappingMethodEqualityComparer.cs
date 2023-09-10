using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public class MappingMethodEqualityComparer : IEqualityComparer<MappingMethod>
{
    public static MappingMethodEqualityComparer Default { get; } = new();

    public bool Equals(
        MappingMethod x,
        MappingMethod y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null))
        {
            return false;
        }

        if (ReferenceEquals(y, null))
        {
            return false;
        }
            
        return x.ClassName == y.ClassName
            && x.ClassContainingNames.SequenceEqual(y.ClassContainingNames)
            && x.Name == y.Name
            && x.Accessibility == y.Accessibility
            && x.IsPartial == y.IsPartial
            && x.ParameterTypeName == y.ParameterTypeName
            && x.ParameterTypeContainingNames.SequenceEqual(y.ParameterTypeContainingNames)
            && x.ParameterName == y.ParameterName
            && x.ReturnTypeName == y.ReturnTypeName
            && x.ReturnTypeContainingNames.SequenceEqual(y.ReturnTypeContainingNames)
            && x.Kind == y.Kind
            && 
            ((x.Kind == MappingMethodKind.Enum && x.EnumValues!.SequenceEqual(y.EnumValues!))
                ||
                (x.Kind == MappingMethodKind.Object && x.Properties!.SequenceEqual<MappingProperty>(y.Properties!)));
    }

    public int GetHashCode(MappingMethod obj)
    {
        unchecked
        {
            var hashCode = obj.ClassName.GetHashCode();
            hashCode = AddCollectionHashCode(hashCode, obj.ClassContainingNames);
            hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.Accessibility.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.IsPartial.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.ParameterTypeName.GetHashCode();
            hashCode = AddCollectionHashCode(hashCode, obj.ParameterTypeContainingNames);
            hashCode = (hashCode * 397) ^ obj.ParameterName.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.ReturnTypeName.GetHashCode();
            hashCode = AddCollectionHashCode(hashCode, obj.ReturnTypeContainingNames);
            hashCode = (hashCode * 397) ^ (int)obj.Kind;
            
            if (obj.Kind == MappingMethodKind.Enum)
            {
                hashCode = AddCollectionHashCode(hashCode, obj.EnumValues!);
            }
            else if (obj.Kind == MappingMethodKind.Object)
            {
                hashCode = AddCollectionHashCode(hashCode, obj.Properties!);
            }
            
            return hashCode;
        }
    }

    private int AddCollectionHashCode<T>(
        int hashCode,
        IReadOnlyCollection<T> collection)
        where T : notnull
    {
        return collection.Aggregate(
            hashCode, 
            (current, item) => (current * 397) ^ item.GetHashCode());
    }
}