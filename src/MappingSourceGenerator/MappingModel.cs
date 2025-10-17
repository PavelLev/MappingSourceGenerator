namespace MappingSourceGenerator;

public record MappingModel(
    string Name,
    string[] ContainingNames,
    MappingModel[] TypeArguments);