﻿namespace MappingSourceGenerator.UnitTests;

public static class SimpleModelTestData
{
    public static IEnumerable<object[]> Data
        => new[]
        {
            new object[]
            {
                "PrimitiveTypeTest",
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
                    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.Person2 Map(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.Person1 person1)
                        => new(
                            person1.Name,
                            person1.Age);
                }
                """
            },
            new object[]
            {
                "EnumTest",
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record PersonWithPhoneType1(
                        string Name,
                        PhoneType1 PhoneType);

                    public enum PhoneType1
                    {
                        Android,
                        Ios,
                        Other,
                    }

                    public record PersonWithPhoneType2(
                        string Name,
                        PhoneType2 PhoneType);

                    public enum PhoneType2
                    {
                        Android,
                        Ios,
                        Other,
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithPhoneType2 Map(this SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2 Map(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1 phoneType1)
                        => phoneType1 switch
                        {
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Android => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Android,
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Ios => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Ios,
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Other => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Other,
                            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.{phoneType1}"),
                        };

                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithPhoneType2 Map(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1)
                        => new(
                            personWithPhoneType1.Name,
                            personWithPhoneType1.PhoneType.Map());
                }
                """
            },
            new object[]
            {
                "NonNullableEnumToNullableEnumTest",
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record PersonWithPhoneType1(
                        string Name,
                        PhoneType1 PhoneType);

                    public enum PhoneType1
                    {
                        Android,
                        Ios,
                        Other,
                    }

                    public record PersonWithOptionalPhoneType2(
                        string Name,
                        PhoneType2? PhoneType);

                    public enum PhoneType2
                    {
                        Android,
                        Ios,
                        Other,
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithOptionalPhoneType2 MapToOptionalPhoneType(this SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2 MapToOptionalPhoneType(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1 phoneType1)
                        => phoneType1 switch
                        {
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Android => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Android,
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Ios => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Ios,
                            MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.Other => MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType2.Other,
                            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.UnitTests.SimpleModelTests.PhoneType1.{phoneType1}"),
                        };

                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalPhoneType2 MapToOptionalPhoneType(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1)
                        => new(
                            personWithPhoneType1.Name,
                            personWithPhoneType1.PhoneType.MapToOptionalPhoneType());
                }
                """
            },
            new object[]
            {
                "NullableReferenceToNullableReferenceTest",
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record PersonWithOptionalName1(
                        string? Name,
                        int Age);

                    public record PersonWithOptionalName2(
                        string? Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithOptionalName2 Map(this SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalName2 Map(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1)
                        => new(
                            personWithOptionalName1.Name,
                            personWithOptionalName1.Age);
                }
                """
            },
            new object[]
            {
                "NonNullableReferenceToNullableReferenceTest",
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
                
                    public record PersonWithOptionalName2(
                        string? Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithOptionalName2 MapToOptionalName(this SimpleModelTests.Person1 person1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalName2 MapToOptionalName(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.Person1 person1)
                        => new(
                            person1.Name,
                            person1.Age);
                }
                """
            },
            new object[]
            {
                "NonNullableValueToNullableValueTest",
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
                
                    public record PersonWithOptionalAge2(
                        string Name,
                        int? Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithOptionalAge2 MapToOptionalAge(this SimpleModelTests.Person1 person1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalAge2 MapToOptionalAge(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.Person1 person1)
                        => new(
                            person1.Name,
                            person1.Age);
                }
                """
            },
            new object[]
            {
                "EnumShouldBeMappedToSupersetEnumTest",
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public enum GeneralErrorCode
                    {
                        ErrorA,
                        ErrorB,
                        ErrorC,
                        ErrorD,
                    }

                    public enum SpecificErrorCode
                    {
                        ErrorB,
                        ErrorC,
                    }
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.GeneralErrorCode Map(this SimpleModelTests.SpecificErrorCode specificErrorCode);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.GeneralErrorCode Map(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.SpecificErrorCode specificErrorCode)
                        => specificErrorCode switch
                        {
                            MappingSourceGenerator.UnitTests.SimpleModelTests.SpecificErrorCode.ErrorB => MappingSourceGenerator.UnitTests.SimpleModelTests.GeneralErrorCode.ErrorB,
                            MappingSourceGenerator.UnitTests.SimpleModelTests.SpecificErrorCode.ErrorC => MappingSourceGenerator.UnitTests.SimpleModelTests.GeneralErrorCode.ErrorC,
                            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.UnitTests.SimpleModelTests.SpecificErrorCode.{specificErrorCode}"),
                        };
                }
                """
            },
            new object[]
            {
                "NullableReferenceToUnspecifiedReferenceTest",
                """
                #nullable enable
                using System;
                using System.Collections.Generic;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;

                public class SimpleModelTests
                {
                    public record PersonWithOptionalName1(
                        string? Name,
                        int Age);

                #nullable disable
                    public record PersonWithUnspecifiedName2(
                        string Name,
                        int Age);
                #nullable restore
                }

                public static partial class SimpleModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial SimpleModelTests.PersonWithUnspecifiedName2 MapToUnspecifiedName(this SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithUnspecifiedName2 MapToUnspecifiedName(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1)
                        => new(
                            personWithOptionalName1.Name,
                            personWithOptionalName1.Age);
                }
                """
            },
            new object[]
            {
                "ObsoleteClassesShouldBeMappedTest",
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

                    [Obsolete("Obsolete record for test purposes")]
                    public record ObsoletePerson2(
                        string Name,
                        int Age);
                }

                public static partial class SimpleModelTestsMapper
                {
                #pragma warning disable CS0618 // allow obsolete class usage for ObsoleteClassesShouldBeMappedTest
                    [GenerateMapping]
                    public static partial SimpleModelTests.ObsoletePerson2 MapToObsolete(this SimpleModelTests.Person1 person1);
                #pragma warning restore
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class SimpleModelTestsMapper
                {
                    public static partial MappingSourceGenerator.UnitTests.SimpleModelTests.ObsoletePerson2 MapToObsolete(
                        this MappingSourceGenerator.UnitTests.SimpleModelTests.Person1 person1)
                        => new(
                            person1.Name,
                            person1.Age);
                }
                """
            }
        };
}