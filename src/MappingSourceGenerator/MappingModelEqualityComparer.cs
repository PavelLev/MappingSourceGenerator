namespace MappingSourceGenerator;

public class MappingModelEqualityComparer : IEqualityComparer<MappingModel>
{
    public static MappingModelEqualityComparer Default { get; } = new();

    public bool Equals(MappingModel x, MappingModel y)
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
        
        return x.Name == y.Name
            && x.ContainingNames.SequenceEqual(y.ContainingNames)
            && x.TypeArguments.SequenceEqual(y.TypeArguments, this);
    }

    public int GetHashCode(MappingModel obj)
    {
        var hashCode = obj.Name.GetHashCode();
        hashCode.AddCollectionHashCode(obj.ContainingNames);
        hashCode.AddCollectionHashCode(obj.TypeArguments, this);
        
        return hashCode;
    }
}