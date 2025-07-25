# MappingSourceGenerator

This project has source generator that allows to generate constructor mappings without any business logic. Main use case is considered to be immutable models (records).

| Package                                                                                                                                                                      | NuGet                                                                                                                                         |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| MappingSourceGenerator                                                                                                                                                       | [![NuGet](https://img.shields.io/nuget/v/MappingSourceGenerator.svg)](https://www.nuget.org/packages/MappingSourceGenerator/)                 |

## Features:
- Extension methods
- Collections support (arrays, `List<T>` and any interface implemented by them)
- Enum support
- Nested mapping support
- Using of manual mapping for nested properties

For examples please check out `src/MappingSourceGenerator.IntegrationTests` project.

## Usage:

1. Add package references to required packages

```xml
        <PackageReference Include="MappingSourceGenerator" Version="0.1.13" PrivateAssets="all" ExcludeAssets="runtime" />
```

2. Create method definition for source generator. It should be `static partial` method marked with `MappingSourceGenerator.Markers.GenerateMappingAttribute` in `static partial` class.

```c#
public static partial class Mapper
{
    [GenerateMapping]
    public static partial Person2 Map(this Person1 person1);
}
```

## Support

Feel free to create issues for bugs or feature requests. General improvement suggestions (e.g. regarding tests) are appreciated.
