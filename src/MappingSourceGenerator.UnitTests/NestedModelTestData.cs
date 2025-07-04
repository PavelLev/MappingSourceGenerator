﻿namespace MappingSourceGenerator.UnitTests;

public static class NestedModelTestData
{
    public static IEnumerable<object[]> Data
        => new[]
        {
            new object[]
            {
                "NestedPropertyTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;
                public class NestedModelTests
                {
                    public record PersonWithCar1(
                        string Name,
                        Car1 Car);
                
                    public record Car1(
                        string Model);
                
                    public record PersonWithCar2(
                        string Name,
                        Car2 Car);
                
                    public record Car2(
                        string Model);
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCar2 Map(this NestedModelTests.PersonWithCar1 personWithCar1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.Car2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.Car1 car1)
                        => new(
                            car1.Model);
                
                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCar2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCar1 personWithCar1)
                        => new(
                            personWithCar1.Name,
                            personWithCar1.Car.Map());
                }
                """
            },
            new object[]
            {
                "NestedCollectionTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
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
                        IReadOnlyCollection<Car2> Cars);
                
                    public record Car2(
                        string Model);
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithCars1 personWithCars1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.Car2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.Car1 car1)
                        => new(
                            car1.Model);

                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCars2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCars1 personWithCars1)
                        => new(
                            personWithCars1.Name,
                            personWithCars1.Cars.Select(Map).ToArray());
                }
                """
            },
            new object[]
            {
                "NestedListTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;
                public class NestedModelTests
                {
                    public record PersonWithCarList1(
                        string Name,
                        List<Car1> Cars);
                
                    public record Car1(
                        string Model);

                    public record PersonWithCarList2(
                        string Name,
                        List<Car2> Cars);
                
                    public record Car2(
                        string Model);
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCarList2 Map(this NestedModelTests.PersonWithCarList1 personWithCarList1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.Car2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.Car1 car1)
                        => new(
                            car1.Model);

                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarList2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarList1 personWithCarList1)
                        => new(
                            personWithCarList1.Name,
                            personWithCarList1.Cars.Select(Map).ToList());
                }
                """
            },
            new object[]
            {
                "NestedCollectionWithEnumPropertyTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;
                public class NestedModelTests
                {
                    public record PersonWithCarWithType1(
                        string Name,
                        IReadOnlyCollection<CarWithType1> Cars);

                    public record CarWithType1(
                        string Model,
                        CarType1 Type);

                    public enum CarType1
                    {
                        Electric,
                        Petrol,
                    }

                    public record PersonWithCarWithType2(
                        string Name,
                        IReadOnlyCollection<CarWithType2> Cars);

                    public record CarWithType2(
                        string Model,
                        CarType2 Type);

                    public enum CarType2
                    {
                        Electric,
                        Petrol,
                    }
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCarWithType2 Map(this NestedModelTests.PersonWithCarWithType1 personWithCarWithType1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.CarType2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.CarType1 carType1)
                        => carType1 switch
                        {
                            MappingSourceGenerator.UnitTests.NestedModelTests.CarType1.Electric => MappingSourceGenerator.UnitTests.NestedModelTests.CarType2.Electric,
                            MappingSourceGenerator.UnitTests.NestedModelTests.CarType1.Petrol => MappingSourceGenerator.UnitTests.NestedModelTests.CarType2.Petrol,
                            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.UnitTests.NestedModelTests.CarType1.{carType1}"),
                        };

                    public static MappingSourceGenerator.UnitTests.NestedModelTests.CarWithType2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.CarWithType1 carWithType1)
                        => new(
                            carWithType1.Model,
                            carWithType1.Type.Map());

                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarWithType2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarWithType1 personWithCarWithType1)
                        => new(
                            personWithCarWithType1.Name,
                            personWithCarWithType1.Cars.Select(Map).ToArray());
                }
                """
            },
            new object[]
            {
                "NestedNullablePropertyTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;
                public class NestedModelTests
                {
                    public record PersonWithNullableCar1(
                        string Name,
                        Car1? Car);
                
                    public record Car1(
                        string Model);

                    public record PersonWithNullableCar2(
                        string Name,
                        Car2? Car);
                
                    public record Car2(
                        string Model);
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithNullableCar2 Map(this NestedModelTests.PersonWithNullableCar1 personWithNullableCar1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.Car2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.Car1 car1)
                        => new(
                            car1.Model);

                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithNullableCar2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithNullableCar1 personWithNullableCar1)
                        => new(
                            personWithNullableCar1.Name,
                            personWithNullableCar1.Car?.Map());
                }
                """
            },
            new object[]
            {
                "NestedArrayTest",
                """
                #nullable enable
                using System.Collections.Generic;
                using System.Linq;
                using MappingSourceGenerator.Markers;

                namespace MappingSourceGenerator.UnitTests;
                public class NestedModelTests
                {
                    public record PersonWithCarArray1(
                        string Name,
                        Car1[] Cars);
                
                    public record Car1(
                        string Model);

                    public record PersonWithCarArray2(
                        string Name,
                        Car2[] Cars);
                
                    public record Car2(
                        string Model);
                }

                public static partial class NestedModelTestsMapper
                {
                    [GenerateMapping]
                    public static partial NestedModelTests.PersonWithCarArray2 Map(this NestedModelTests.PersonWithCarArray1 personWithCarArray1);
                }
                """,
                """
                // <auto-generated/>
                #pragma warning disable
                #nullable enable
                using System;
                using System.Linq;

                namespace MappingSourceGenerator.UnitTests;

                partial class NestedModelTestsMapper
                {
                    public static MappingSourceGenerator.UnitTests.NestedModelTests.Car2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.Car1 car1)
                        => new(
                            car1.Model);

                    public static partial MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarArray2 Map(
                        this MappingSourceGenerator.UnitTests.NestedModelTests.PersonWithCarArray1 personWithCarArray1)
                        => new(
                            personWithCarArray1.Name,
                            personWithCarArray1.Cars.Select(Map).ToArray());
                }
                """
            },
        };
}