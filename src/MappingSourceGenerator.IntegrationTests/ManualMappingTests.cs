using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class ManualMappingTests
{
    [Fact]
    public void NestedPropertyWithManualMappingTest()
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
    public void NestedArrayWithManualItemMappingTest()
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
    public void NestedListWithManualItemMappingTest()
    {
        var personWithCarList1 = new PersonWithCarList1(
            "Bob",
            [
                new("Model S"),
                new("GT-R")
            ]);
        
        var personWithCarList2 = personWithCarList1.Map();

        using (new AssertionScope())
        {
            personWithCarList2.Cars.Count.Should().Be(2);
            personWithCarList2.Cars.ElementAt(0).Model.Should().Be("Model S");
            personWithCarList2.Cars.ElementAt(1).Model.Should().Be("GT-R");
        }
    }
    
    public record PersonWithCarList1(
        string Name,
        List<Car1> Cars);
    
    public record PersonWithCarList2(
        string Name,
        List<Car2> Cars);
    
    [Fact]
    public void NestedCollectionWithManualMappingTest()
    {
        var personWithCarCollection1 = new PersonWithCarCollection1(
            "Bob",
            [
                new("Model S"),
                new("GT-R")
            ]);
        
        var personWithCarCollection2 = personWithCarCollection1.Map();

        using (new AssertionScope())
        {
            personWithCarCollection2.Cars.Count.Should().Be(2);
            personWithCarCollection2.Cars.ElementAt(0).Model.Should().Be("Model S");
            personWithCarCollection2.Cars.ElementAt(1).Model.Should().Be("GT-R");
        }
    }
    
    public record PersonWithCarCollection1(
        string Name,
        IReadOnlyCollection<Car1> Cars);
                
    public record PersonWithCarCollection2(
        string Name,
        IReadOnlyCollection<Car2> Cars);

    [Fact]
    public void NestedArrayWithManualMappingTest()
    {
        var personWithManualCarArray1 = new PersonWithManualCarArray1(
            "Bob",
            [
                new("Model S"),
                new("GT-R")
            ]);
        
        var personWithManualCarArray2 = personWithManualCarArray1.Map();

        using (new AssertionScope())
        {
            personWithManualCarArray2.ManualCars.Length.Should().Be(2);
            personWithManualCarArray2.ManualCars.ElementAt(0).Model.Should().Be("Model S");
            personWithManualCarArray2.ManualCars.ElementAt(1).Model.Should().Be("GT-R");
        }
    }
    
    public record PersonWithManualCarArray1(
        string Name,
        ManualCar1[] ManualCars);
                
    public record ManualCar1(
        string Model);
    
    public record PersonWithManualCarArray2(
        string Name,
        ManualCar2[] ManualCars);
                
    public record ManualCar2(
        string Model);
    
    [Fact]
    public void NestedCollectionWithEnumPropertyTest()
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

    [Fact]
    public void NullableEnumPropertyWithManualNullableMappingTest()
    {
        var personWithAndroid1 = new PersonWithOptionalPhoneType1(
            "Rob",
            PhoneType1.Android1);
        var personWithNull1 = new PersonWithOptionalPhoneType1(
            "Bob",
            null);

        var personWithAndroid2 = personWithAndroid1.MapNullable();
        var personWithNull2 = personWithNull1.MapNullable();

        using (new AssertionScope())
        {
            personWithAndroid2.PhoneType.Should().Be(PhoneType2.Android2);
            personWithNull2.PhoneType.Should().BeNull();
        }
    }
    
    public record PersonWithOptionalPhoneType1(
        string Name,
        PhoneType1? PhoneType);
                    
    public enum PhoneType1
    {
        Android1,
    }
                    
    public record PersonWithOptionalPhoneType2(
        string Name,
        PhoneType2? PhoneType);
                    
    public enum PhoneType2
    {
        Android2,
    }

    [Fact]
    public void NullableEnumPropertyWithManualNonNullableMappingTest()
    {
        // Not using theory to keep number of Integration and Unit tests in sync
        var personWithAndroid1 = new PersonWithOptionalPhoneType1(
            "Rob",
            PhoneType1.Android1);
        var personWithNull1 = new PersonWithOptionalPhoneType1(
            "Bob",
            null);

        var personWithAndroid2 = personWithAndroid1.MapNonNullable();
        var personWithNull2 = personWithNull1.MapNonNullable();

        using (new AssertionScope())
        {
            personWithAndroid2.PhoneType.Should().Be(PhoneType2.Android2);
            personWithNull2.PhoneType.Should().BeNull();
        }
    }
}

public static partial class ManualMappingTestsMapper
{
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCar2 Map(this ManualMappingTests.PersonWithCar1 personWithCar1);
                
    public static ManualMappingTests.Car2 Map(this ManualMappingTests.Car1 car1)
        => new(car1.Model);
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCarCollection2 Map(this ManualMappingTests.PersonWithCarCollection1 personWithCarCollection1);
                
    public static IReadOnlyCollection<ManualMappingTests.Car2> Map(
        this IReadOnlyCollection<ManualMappingTests.Car1> car1Collection)
        => car1Collection.Select(_ => new ManualMappingTests.Car2(_.Model)).ToArray();
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCarArray2 Map(this ManualMappingTests.PersonWithCarArray1 personWithCarArray1);
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithCarList2 Map(this ManualMappingTests.PersonWithCarList1 personWithCarList1);
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithManualCarArray2 Map(this ManualMappingTests.PersonWithManualCarArray1 personWithManualCarArray1);
    
    public static ManualMappingTests.ManualCar2[] Map(
        this ManualMappingTests.ManualCar1[] car1Array)
        => car1Array.Select(_ => new ManualMappingTests.ManualCar2(_.Model)).ToArray();
    
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
    public static partial ManualMappingTests.PersonWithOptionalPhoneType2 MapNonNullable(
        this  ManualMappingTests.PersonWithOptionalPhoneType1 personWithOptionalPhoneType1);

    public static ManualMappingTests.PhoneType2 MapNonNullable(this ManualMappingTests.PhoneType1 phoneType1)
        => phoneType1 switch
        {
            ManualMappingTests.PhoneType1.Android1 => ManualMappingTests.PhoneType2.Android2,
            _ => throw new InvalidOperationException($"Unable to map phone type {phoneType1}")
        };
    
    [GenerateMapping]
    public static partial ManualMappingTests.PersonWithOptionalPhoneType2 MapNullable(
        this  ManualMappingTests.PersonWithOptionalPhoneType1 personWithOptionalPhoneType1);

    public static ManualMappingTests.PhoneType2? MapNullable(this ManualMappingTests.PhoneType1? phoneType1)
        => phoneType1 switch
        {
            null => null,
            ManualMappingTests.PhoneType1.Android1 => ManualMappingTests.PhoneType2.Android2,
            _ => throw new InvalidOperationException($"Unable to map phone type {phoneType1}")
        };
}