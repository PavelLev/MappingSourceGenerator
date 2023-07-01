namespace MappingSourceGenerator.UnitTests;

public static class MappingMethodDiagnosticTestData
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
                    public static SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1000",
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
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial void Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1001",
                16,
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
                    public static partial SimpleModelTests.Person2 Map(SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1002",
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
                
                public class Wrapper
                {
                    public static partial class SimpleModelTestsMapper
                    {
                        [GenerateMapping]
                        public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                    }
                }
                """,
                "MAPGEN1003",
                22,
                8
            }
        };
}