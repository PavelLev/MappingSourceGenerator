using System;
using System.Collections.Generic;
using System.Linq;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.Tests;

public class ManualMappingTests
{
    [Fact]
    public void NestedCollectionWithEnumPropertyTest()
    {
        var personWithCarWithType1 = new PersonWithCarWithType1(
            "Bob",
            new CarWithType1[]
            {
                new("Model S", CarType1.Electric),
                new("GT-R", CarType1.Petrol),
            });
        var personWithCarWithType2 = personWithCarWithType1.Map();

        Assert.Equal(CarType2.Electric, personWithCarWithType2.Cars.ElementAt(0).Type);
        Assert.Equal(CarType2.Petrol, personWithCarWithType2.Cars.ElementAt(1).Type);
    }

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

public static partial class ManualMappingTestsMapper
{
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCarWithType2 Map(this ManualMappingTests.PersonWithCarWithType1 personWithCarWithType1);

    public static ManualMappingTests.CarType2 Map(
        this ManualMappingTests.CarType1 carType1)
        => carType1 switch
        {
            ManualMappingTests.CarType1.Electric => ManualMappingTests.CarType2.Electric,
            ManualMappingTests.CarType1.Petrol => ManualMappingTests.CarType2.Petrol,
            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.Tests.ManualMappingTests.CarType1.{carType1}"),
        };
}