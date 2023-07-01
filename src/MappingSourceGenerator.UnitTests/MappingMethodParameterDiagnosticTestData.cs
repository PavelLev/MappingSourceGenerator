namespace MappingSourceGenerator.UnitTests;

public static class MappingMethodParameterDiagnosticTestData
{
    public static IEnumerable<object[]> Data
        => new[]
        {
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record Person1(
                        string Name,
                        int Age);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1, int age);
                }
                """,
                "MAPGEN1010",
                20,
                4
            },
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record Person1(
                        string Name,
                        int Age);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map<T>(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1011",
                20,
                4
            },
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record Person1(
                        string Name,
                        int Age);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1? person1);
                }
                """,
                "MAPGEN1012",
                20,
                4
            },
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record struct Person1(
                        string Name,
                        int Age);
                
                    public record struct Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1? person1);
                }
                """,
                "MAPGEN1012",
                20,
                4
            },
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record struct Person1(
                        string Name,
                        int Age);
                
                    public record struct Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1[] person1Array);
                }
                """,
                "MAPGEN1013",
                20,
                4
            },
            new object[]
            {
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record struct Person1(
                        string Name,
                        int Age);
                
                    public record struct Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this IEnumerable<SimpleModelTests.Person1> person1Enumerable);
                }
                """,
                "MAPGEN1013",
                20,
                4
            }
        };
}