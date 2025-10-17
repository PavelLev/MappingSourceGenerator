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
            && MappingModelEqualityComparer.Default.Equals(x.ParameterTypeModel, y.ParameterTypeModel)
            && x.ParameterName == y.ParameterName
            && MappingModelEqualityComparer.Default.Equals(x.ReturnTypeModel, y.ReturnTypeModel)
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
            hashCode = hashCode.AddCollectionHashCode(obj.ClassContainingNames);
            hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.Accessibility.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.IsPartial.GetHashCode();
            hashCode = (hashCode * 397) ^ MappingModelEqualityComparer.Default.GetHashCode(obj.ParameterTypeModel);
            hashCode = (hashCode * 397) ^ obj.ParameterName.GetHashCode();
            hashCode = (hashCode * 397) ^ MappingModelEqualityComparer.Default.GetHashCode(obj.ReturnTypeModel);
            hashCode = (hashCode * 397) ^ (int)obj.Kind;
            
            if (obj.Kind == MappingMethodKind.Enum)
            {
                hashCode = hashCode.AddCollectionHashCode(obj.EnumValues!);
            }
            else if (obj.Kind == MappingMethodKind.Object)
            {
                hashCode = hashCode.AddCollectionHashCode(obj.Properties!);
            }
            
            return hashCode;
        }
    }
}