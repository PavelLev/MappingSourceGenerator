using FluentAssertions;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class GenericModelTests
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
    public static partial GenericModelTests.PersonWithCar2<GenericModelTests.Car2> Map(
        this GenericModelTests.PersonWithCar1<GenericModelTests.Car1> personWithCar1);
    
    [GenerateMapping]
    public static partial GenericModelTests.PersonWithCar2<GenericModelTests.ManualCar2> Map(
        this GenericModelTests.PersonWithCar1<GenericModelTests.ManualCar1> personWithCar1);

    public static GenericModelTests.ManualCar2 Map(this GenericModelTests.ManualCar1 manualCar1)
        => new(
            manualCar1.Model1);
}