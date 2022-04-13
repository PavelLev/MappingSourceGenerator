# MappingSourceGenerator

This project has source generator that allows to generate mappings without any business logic.

| Package                                                                                                                                                                      | NuGet                                                                                                                                         |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| MappingSourceGenerator                                                                                                                                                       | [![NuGet](https://img.shields.io/nuget/v/MappingSourceGenerator.svg)](https://www.nuget.org/packages/MappingSourceGenerator/)                 |
| MappingSourceGenerator.Markers | [![NuGet](https://img.shields.io/nuget/v/MappingSourceGenerator.Markers.svg)](https://www.nuget.org/packages/MappingSourceGenerator.Markers/) |

## Features:
- Extension methods
- Collections support (IEnumerable<T> or derivatives)
- Enum support
- Nested mapping support
- Using of manual mapping for nested properties

## Usage:

1. Add package references to required packages

```xml
        <PackageReference Include="MappingSourceGenerator" Version="0.1.4" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
        <PackageReference Include="MappingSourceGenerator.Markers" Version="0.1.4" />
```

2. Create method definition for source generator. It should be `static partial` method marked with `MappingSourceGenerator.Markers.GenerateMappingAttribute` in `static partial` class.

```c#
public static partial class Mapper
{
    [GenerateMapping]
    public static partial Person2 Map(this Person1 person1);
}
```

