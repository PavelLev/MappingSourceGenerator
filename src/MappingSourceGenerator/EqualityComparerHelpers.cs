namespace MappingSourceGenerator;

public static class EqualityComparerHelpers
{
    public static int AddCollectionHashCode<T>(
        this int hashCode,
        IReadOnlyCollection<T> collection,
        IEqualityComparer<T>? equalityComparer = null)
        where T : notnull
    {
        equalityComparer ??= EqualityComparer<T>.Default;
        
        unchecked
        {
            foreach (var item in collection)
            {
                hashCode = (hashCode * 397) ^ equalityComparer.GetHashCode(item);
            }
        }
        
        return hashCode;
    }
}