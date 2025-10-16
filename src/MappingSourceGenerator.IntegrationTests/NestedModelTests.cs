using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class NestedModelTests
{
    [Fact]
    public void NestedPropertyTest()
    {
        var personWithCar1 = new PersonWithCar1(
            "Bob",
            new("Model S"));
        
        var personWithCar2 = personWithCar1.Map();

        personWithCar2.Car.Model.Should().Be(personWithCar1.Car.Model);
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
    public void NestedCollectionTest()
    {
        var personWithCar1 = new PersonWithCars1(
            "Bob",
            [new("Model S"), new("GT-R")]);
        
        var personWithCar2 = personWithCar1.Map();

        personWithCar2.Cars.Select(_ => _.Model).Should().BeEquivalentTo(personWithCar1.Cars.Select(_ => _.Model));
    }

    public record PersonWithCars1(
        string Name,
        IReadOnlyCollection<Car1> Cars);

    public record PersonWithCars2(
        string Name,
        IReadOnlyCollection<Car2> Cars);

    [Fact]
    public void NestedListTest()
    {
        var personWithCarList1 = new PersonWithCarList1(
            "Bob",
            [new("Model S"), new("GT-R")]);
        
        var personWithCar2 = personWithCarList1.Map();

        personWithCar2.Cars.Select(_ => _.Model).Should().BeEquivalentTo(personWithCarList1.Cars.Select(_ => _.Model));
    }

    public record PersonWithCarList1(
        string Name,
        List<Car1> Cars);

    public record PersonWithCarList2(
        string Name,
        List<Car2> Cars);

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
    public void NestedNullablePropertyTest()
    {
        // Not using theory to keep number of Integration and Unit tests in sync
        var personWithNonNullCar1 = new PersonWithNullableCar1(
            "Bob",
            new("Model S"));
        var personWithNullCar1 = new PersonWithNullableCar1(
            "Bob",
            null);

        var personWithNonNullCar2 = personWithNonNullCar1.Map();
        var personWithNullCar2 = personWithNullCar1.Map();
        
        personWithNonNullCar2.Car.Should().Be(new Car2("Model S"));
        personWithNullCar2.Car.Should().BeNull();
    }

    public record PersonWithNullableCar1(
        string Name,
        Car1? Car);

    public record PersonWithNullableCar2(
        string Name,
        Car2? Car);

    [Fact]
    public void NestedArrayTest()
    {
        var personWithCarArray1 = new PersonWithCarArray1(
            "Bob",
            [new("Model S"), new("GT-R")]);
        
        var personWithCar2 = personWithCarArray1.Map();

        personWithCar2.Cars.Select(_ => _.Model).Should().BeEquivalentTo(personWithCarArray1.Cars.Select(_ => _.Model));
    }
    
    public record PersonWithCarArray1(
        string Name,
        Car1[] Cars);
    
    public record PersonWithCarArray2(
        string Name,
        Car2[] Cars);
}

public static partial class NestedModelTestsMapper
{
    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCar2 Map(this NestedModelTests.PersonWithCar1 personWithCar1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithCars1 personWithCars1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCarList2 Map(this NestedModelTests.PersonWithCarList1 personWithCarList1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCarWithType2 Map(this NestedModelTests.PersonWithCarWithType1 personWithCarWithType1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithNullableCar2 Map(this NestedModelTests.PersonWithNullableCar1 personWithNullableCar1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCarArray2 Map(this NestedModelTests.PersonWithCarArray1 personWithCarArray1);
}