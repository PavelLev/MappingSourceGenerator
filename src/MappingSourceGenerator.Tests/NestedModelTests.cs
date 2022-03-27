using System.Collections.Generic;
using System.Linq;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.Tests;

public class NestedModelTests
{
    [Fact]
    public void NestedPropertyTest()
    {
        var personWithCar1 = new PersonWithCar1(
            "Bob",
            new("Model S"));
        var personWithCar2 = personWithCar1.Map();

        Assert.Equal(personWithCar1.Car.Model, personWithCar2.Car.Model);
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
            new Car1[] {new("Model S"), new("GT-R")});
        var personWithCar2 = personWithCar1.Map();

        Assert.Equal(personWithCar1.Cars.Select(_ => _.Model), personWithCar2.Cars.Select(_ => _.Model));
    }

    public record PersonWithCars1(
        string Name,
        IReadOnlyCollection<Car1> Cars);

    public record PersonWithCars2(
        string Name,
        IReadOnlyCollection<Car2> Cars);

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

    [Fact]
    public void NestedNullablePropertyTest()
    {
        var personWithNullableCar1 = new PersonWithNullableCar1(
            "Bob",
            new("Model S"));
        var personWithNullableCar2 = personWithNullableCar1.Map();

        Assert.Equal(personWithNullableCar1.Car!.Model, personWithNullableCar2.Car!.Model);
    }

    public record PersonWithNullableCar1(
        string Name,
        Car1? Car);

    public record PersonWithNullableCar2(
        string Name,
        Car2? Car);
}

public static partial class NestedModelTestsMapper
{
    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCar2 Map(this NestedModelTests.PersonWithCar1 personWithCar1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCars2 Map(this NestedModelTests.PersonWithCars1 personWithCars1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithCarWithType2 Map(this NestedModelTests.PersonWithCarWithType1 personWithCarWithType1);

    [GenerateMapping]
    public static partial NestedModelTests.PersonWithNullableCar2 Map(this NestedModelTests.PersonWithNullableCar1 personWithNullableCar1);
}