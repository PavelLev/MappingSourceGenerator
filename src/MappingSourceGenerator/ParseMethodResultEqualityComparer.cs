namespace MappingSourceGenerator;

public class ParseMethodResultEqualityComparer : IEqualityComparer<ParseMethodResult>
{
    public static ParseMethodResultEqualityComparer Default { get; } = new();
    
    public bool Equals(
        ParseMethodResult x,
        ParseMethodResult y)
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
        
        return x.MappingMethods.SequenceEqual(y.MappingMethods, MappingMethodEqualityComparer.Default) 
            && x.Diagnostics.SequenceEqual(y.Diagnostics);
    }

    public int GetHashCode(ParseMethodResult obj)
    {
        unchecked
        {
            var hashCode = 0;
            
            hashCode = obj.MappingMethods!.Aggregate(
                hashCode, 
                (current, mappingMethod) => (current * 397) ^ MappingMethodEqualityComparer.Default.GetHashCode(mappingMethod));
            
            hashCode = obj.Diagnostics!.Aggregate(
                hashCode, 
                (current, diagnostic) => (current * 397) ^ diagnostic.GetHashCode());
            
            return hashCode;
        }
    }
}