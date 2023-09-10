namespace MappingSourceGenerator;

public interface IEmitter
{
    void Emit(
        Action<string, string> addSource,
        IReadOnlyCollection<MappingMethod> mappingMethods,
        CancellationToken cancellationToken);
}