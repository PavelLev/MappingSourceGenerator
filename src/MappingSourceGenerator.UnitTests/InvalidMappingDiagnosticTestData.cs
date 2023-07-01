namespace MappingSourceGenerator.UnitTests;

public static class InvalidMappingDiagnosticTestData
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
                        int Age1);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1030",
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
                    public enum PhoneType1
                    {
                        Android,
                        Ios,
                        Other,
                    }
                
                    public enum PhoneType2
                    {
                        Android2,
                        Ios,
                        Other,
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PhoneType2 Map(this SimpleModelTests.PhoneType1 phoneType1);
                }
                """,
                "MAPGEN1031",
                26,
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
                        string? Name,
                        int Age);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1032",
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
                        int? Age);
                
                    public record Person2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                "MAPGEN1032",
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
                    public record PersonWithNullableEmails1(
                        string Name,
                        IReadOnlyCollection<string?> Emails);

                    public record PersonWithEmails2(
                        string Name,
                        IReadOnlyCollection<string> Emails);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithEmails2 Map(this SimpleModelTests.PersonWithNullableEmails1 personWithNullableEmails1);
                }
                """,
                "MAPGEN1033",
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

                public class NestedModelTests
                {
                    public record PersonWithNullableCars1(
                        string Name,
                        IReadOnlyCollection<Car1?> Cars);

                    public record Car1(
                        string Model);

                    public record PersonWithCars2(
                        string Name,
                        IReadOnlyCollection<Car2> Cars);

                    public record Car2(
                        string Model);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithNullableCars1 personWithNullableCars1);
                }
                """,
                "MAPGEN1033",
                26,
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

                public class NestedModelTests
                {
                    public record PersonWithCars1(
                        string Name,
                        IReadOnlyCollection<Car1> Cars);
                
                    public record Car1(
                        string Model);
                
                    public record PersonWithCars2(
                        string Name,
                        Car2 Cars);
                
                    public record Car2(
                        string Model);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithCars1 personWithCars1);
                }
                """,
                "MAPGEN1034",
                26,
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

                public class NestedModelTests
                {
                    public record PersonWithCars1(
                        string Name,
                        Car1 Cars);
                
                    public record Car1(
                        string Model);
                
                    public record PersonWithCars2(
                        string Name,
                        IReadOnlyCollection<Car2> Cars);
                
                    public record Car2(
                        string Model);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithCars1 personWithCars1);
                }
                """,
                "MAPGEN1035",
                26,
                4
            }
        };
}