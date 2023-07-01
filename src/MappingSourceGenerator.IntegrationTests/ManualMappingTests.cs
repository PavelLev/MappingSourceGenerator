using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
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

        using (new AssertionScope())
        {
            personWithCarWithType2.Cars.Count.Should().Be(2);
            personWithCarWithType2.Cars.ElementAt(0).Type.Should().Be(CarType2.Electric);
            personWithCarWithType2.Cars.ElementAt(1).Type.Should().Be(CarType2.Petrol);
        }
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

    [Fact]
    public void NestedCollectionWithManualMappingTest()
    {
        var personWithCarWithType1 = new PersonWithCarCollection1(
            "Bob",
            new Car1[]
            {
                new("Model S"),
                new("GT-R"),
            });
        
        var personWithCarWithType2 = personWithCarWithType1.Map();

        using (new AssertionScope())
        {
            personWithCarWithType2.Cars.Count.Should().Be(2);
            personWithCarWithType2.Cars.ElementAt(0).Model.Should().Be("Model S");
            personWithCarWithType2.Cars.ElementAt(1).Model.Should().Be("GT-R");
        }
    }
    
    public record PersonWithCarCollection1(
        string Name,
        IReadOnlyCollection<Car1> Cars);
                
    public record Car1(
        string Model);
                
    public record PersonWithCarCollection2(
        string Name,
        IReadOnlyCollection<Car2> Cars);
                
    public record Car2(
        string Model);
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
            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.IntegrationTests.ManualMappingTests.CarType1.{carType1}"),
        };
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCarCollection2 Map(this ManualMappingTests.PersonWithCarCollection1 personWithCarCollection1);
                
    public static IReadOnlyCollection<ManualMappingTests.Car2> Map(
        this IReadOnlyCollection<ManualMappingTests.Car1> car1Collection)
        => car1Collection.Select(_ => new ManualMappingTests.Car2(_.Model)).ToArray();
}