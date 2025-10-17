using FluentAssertions;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class GenericModelMappingTests
{
    [Fact]
    public void NestedGenericPropertyTest()
    {
        var personWithCar1 = new PersonWithCar1<Car1>(
            "Rob",
            new("Model S"));

        var personWithCar2 = personWithCar1.Map();

        personWithCar2.Car.Model.Should().Be("Model S");
    }
    
    public record PersonWithCar1<TCar>(
        string Name,
        TCar Car);
                
    public record Car1(
        string Model);
                
    public record PersonWithCar2<TCar>(
        string Name,
        TCar Car);
                
    public record Car2(
        string Model);

    [Fact]
    public void NestedGenericPropertyWithManualMappingTest()
    {
        var personWithManualCar1 = new PersonWithCar1<ManualCar1>(
            "Rob",
            new("Model S"));

        var personWithManualCar2 = personWithManualCar1.Map();
        
        personWithManualCar2.Car.Model2.Should().Be("Model S");
    }
                
    public record ManualCar1(
        string Model1);
                
    public record ManualCar2(
        string Model2);
}

public static partial class GenericModelMappingTestsMapper
{
    [GenerateMapping]
    public static partial GenericModelMappingTests.PersonWithCar2<GenericModelMappingTests.Car2> Map(
        this GenericModelMappingTests.PersonWithCar1<GenericModelMappingTests.Car1> personWithCar1);
    
    [GenerateMapping]
    public static partial GenericModelMappingTests.PersonWithCar2<GenericModelMappingTests.ManualCar2> Map(
        this GenericModelMappingTests.PersonWithCar1<GenericModelMappingTests.ManualCar1> personWithCar1);

    public static GenericModelMappingTests.ManualCar2 Map(this GenericModelMappingTests.ManualCar1 manualCar1)
        => new(
            manualCar1.Model1);
}