using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.Tests;

public class SimpleModelTests
{
    [Fact]
    public void PrimitiveTypeTest()
    {
        var person1 = new Person1("Bob", 26);
        var person2 = person1.Map();

        Assert.Equal(person1.Name, person2.Name);
        Assert.Equal(person1.Age, person2.Age);
    }

    public record Person1(
        string Name,
        int Age);

    public record Person2(
        string Name,
        int Age);

    [Theory]
    [InlineData("Rob", PhoneType1.Android, PhoneType2.Android)]
    [InlineData("Bob", PhoneType1.Ios, PhoneType2.Ios)]
    [InlineData("Eugene", PhoneType1.Other, PhoneType2.Other)]
    public void EnumTest(string name, PhoneType1 sourcePhoneType, PhoneType2 expectedPhoneType)
    {
        var personWithPhoneType1 = new PersonWithPhoneType1(name, sourcePhoneType);
        var personWithPhoneType2 = personWithPhoneType1.Map();

        Assert.Equal(personWithPhoneType1.Name, personWithPhoneType2.Name);
        Assert.Equal(expectedPhoneType, personWithPhoneType2.PhoneType);
    }

    public record PersonWithPhoneType1(
        string Name,
        PhoneType1 PhoneType);

    public record PersonWithPhoneType2(
        string Name,
        PhoneType2 PhoneType);

    public enum PhoneType1
    {
        Android,
        Ios,
        Other,
    }

    public enum PhoneType2
    {
        Android,
        Ios,
        Other,
    }

    [Fact]
    public void NullableReferenceToNullableReferenceTest()
    {
        var person1 = new Person1("Bob", 26);
        var person2 = person1.Map();

        Assert.Equal(person1.Name, person2.Name);
        Assert.Equal(person1.Age, person2.Age);
    }

    public record PersonWithOptionalName1(
        string? Name,
        int Age);

    public record PersonWithOptionalName2(
        string? Name,
        int Age);

    [Fact]
    public void NonNullableReferenceToNullableReferenceTest()
    {
        var person1 = new Person1("Bob", 26);
        var person2 = person1.MapToOptionalName();

        Assert.Equal(person1.Name, person2.Name);
        Assert.Equal(person1.Age, person2.Age);
    }

    [Fact]
    public void EnumShouldBeMappedToSupersetEnumTest()
    {
        var specificErrorCode = SpecificErrorCode.ErrorB;
        var generalErrorCode = specificErrorCode.Map();
    }

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
    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithPhoneType2 Map(this SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalName2 Map(this SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalName2 MapToOptionalName(this SimpleModelTests.Person1 person1);

    [GenerateMapping]
    public static partial SimpleModelTests.GeneralErrorCode Map(this SimpleModelTests.SpecificErrorCode specificErrorCode);
}