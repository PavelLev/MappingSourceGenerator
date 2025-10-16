using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class UsableMapMethodNamesTests
{
    [Fact]
    public void NestedPropertyWithUsableMapMethodNameTest()
    {
        var personWithCar1 = new PersonWithCar1(
            "Bob",
            new("Model S"));
        
        var personWithCar2 = personWithCar1.Map();

        using (new AssertionScope())
        {
            personWithCar2.Car.Model.Should().Be("Model S");
        }
    }
    
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

    [Fact]
    public void NestedArrayWithWithUsableMapMethodNameItemMappingTest()
    {
        var personWithCarArray1 = new PersonWithCarArray1(
            "Bob",
            [
                new("Model S"),
                new("GT-R")
            ]);
        
        var personWithCarArray2 = personWithCarArray1.Map();

        using (new AssertionScope())
        {
            personWithCarArray2.Cars.Length.Should().Be(2);
            personWithCarArray2.Cars.ElementAt(0).Model.Should().Be("Model S");
            personWithCarArray2.Cars.ElementAt(1).Model.Should().Be("GT-R");
        }
    }
    
    public record PersonWithCarArray1(
        string Name,
        Car1[] Cars);
    
    public record PersonWithCarArray2(
        string Name,
        Car2[] Cars);

    [Fact]
    public void NestedCollectionWithEnumPropertyUsableMapMethodNameTest()
    {
        var personWithCarWithType1 = new PersonWithCarWithType1(
            "Bob",
            [
                new("Model S", CarType1.Electric),
                new("GT-R", CarType1.Petrol)
            ]);
        
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
}
    
public static partial class UsableMapMethodNamesTestsMapper
{
    [GenerateMapping("Map1")]
    public static partial UsableMapMethodNamesTests.PersonWithCar2 Map(this UsableMapMethodNamesTests.PersonWithCar1 personWithCar1);
                
    public static UsableMapMethodNamesTests.Car2 Map1(
        this UsableMapMethodNamesTests.Car1 car1)
        => new(
            car1.Model);
    
    [GenerateMapping("Map1")]
    public static partial UsableMapMethodNamesTests.PersonWithCarArray2 Map(this UsableMapMethodNamesTests.PersonWithCarArray1 personWithCarArray1);
    
    [GenerateMapping("Map1")]
    public static partial UsableMapMethodNamesTests.PersonWithCarWithType2 Map(this UsableMapMethodNamesTests.PersonWithCarWithType1 personWithCarWithType1);
                
    public static UsableMapMethodNamesTests.CarType2 Map1(
        this UsableMapMethodNamesTests.CarType1 carType1)
        => carType1 switch
        {
            UsableMapMethodNamesTests.CarType1.Electric => UsableMapMethodNamesTests.CarType2.Electric,
            UsableMapMethodNamesTests.CarType1.Petrol => UsableMapMethodNamesTests.CarType2.Petrol,
            _ => throw new InvalidOperationException($"Unable to map MappingSourceGenerator.UnitTests.UsableMapMethodNamesTests.CarType1.{carType1}"),
        };
}