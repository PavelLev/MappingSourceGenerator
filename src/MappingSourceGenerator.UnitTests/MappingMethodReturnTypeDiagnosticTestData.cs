namespace MappingSourceGenerator.UnitTests;

public static class MappingMethodReturnTypeDiagnosticTestData
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
                        int Age)
                    {
                        public Person2(
                            int age,
                            string name)
                        {
                            Age = age;
                            Name = name;
                        }
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1020",
                29,
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
                
                    public class Person2
                    {
                        public string Name { get; set; }
                        
                        public int Age { get; set; }
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1021",
                23,
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
                    public static partial SimpleModelTests.Person2[] Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1022",
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
                    public static partial IReadOnlyCollection<SimpleModelTests.Person2> Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1022",
                20,
                4
            }
        };
}