using System;
using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class SimpleModelTests
{
    [Fact]
    public void PrimitiveTypeTest()
    {
        var person1 = new Person1("Bob", 26);
        
        var person2 = person1.Map();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Age.Should().Be(person1.Age);
        }
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

        personWithPhoneType2.PhoneType.Should().Be(expectedPhoneType);
    }

    public record PersonWithPhoneType1(
        string Name,
        PhoneType1 PhoneType);

    public enum PhoneType1
    {
        Android,
        Ios,
        Other,
    }

    public record PersonWithPhoneType2(
        string Name,
        PhoneType2 PhoneType);

    public enum PhoneType2
    {
        Android,
        Ios,
        Other,
    }
    
    [Theory]
    [InlineData("Rob", PhoneType1.Android, PhoneType2.Android)]
    [InlineData("Bob", PhoneType1.Ios, PhoneType2.Ios)]
    [InlineData("Eugene", PhoneType1.Other, PhoneType2.Other)]
    public void NonNullableEnumToNullableEnumTest(string name, PhoneType1 sourcePhoneType, PhoneType2 expectedPhoneType)
    {
        var personWithPhoneType1 = new PersonWithPhoneType1(name, sourcePhoneType);
        
        var personWithPhoneType2 = personWithPhoneType1.MapToOptionalPhoneType();

        personWithPhoneType2.PhoneType.Should().Be(expectedPhoneType);
    }

    public record PersonWithOptionalPhoneType2(
        string Name,
        PhoneType2? PhoneType);

    [Fact]
    public void NullableReferenceToNullableReferenceTest()
    {
        var personWithOptionalName1 = new PersonWithOptionalName1("Bob", 26);
        
        var personWithOptionalName2 = personWithOptionalName1.Map();

        using (new AssertionScope())
        {
            personWithOptionalName2.Name.Should().Be(personWithOptionalName1.Name);
            personWithOptionalName2.Age.Should().Be(personWithOptionalName1.Age);
        }
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

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Age.Should().Be(person1.Age);
        }
    }

    [Fact]
    public void NonNullableValueToNullableValueTest()
    {
        var person1 = new Person1("Bob", 26);
        
        var person2 = person1.MapToOptionalAge();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Age.Should().Be(person1.Age);
        }
    }

    public record PersonWithOptionalAge2(
        string Name,
        int? Age);

    [Fact]
    public void EnumShouldBeMappedToSupersetEnumTest()
    {
        var specificErrorCode = SpecificErrorCode.ErrorB;
        
        var generalErrorCode = specificErrorCode.Map();

        generalErrorCode.Should().Be(GeneralErrorCode.ErrorB);
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

    [Fact]
    public void NullableReferenceToUnspecifiedReferenceTest()
    {
        var person1 = new PersonWithOptionalName1("Bob", 26);
        
        var person2 = person1.MapToUnspecifiedName();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Age.Should().Be(person1.Age);
        }
    }
    
#nullable disable
    public record PersonWithUnspecifiedName2(
        string Name,
        int Age);
#nullable restore

    [Fact]
    public void ObsoleteClassesShouldBeMappedTest()
    {
        var person1 = new Person1(
            "Bob",
            26);

        var obsoletePerson2 = person1.MapToObsolete();

        using (new AssertionScope())
        {
            obsoletePerson2.Name.Should().Be(person1.Name);
            obsoletePerson2.Age.Should().Be(person1.Age);
        }
    }
    
    [Obsolete("Obsolete record for test purposes")]
    public record ObsoletePerson2(
        string Name,
        int Age);
}

public static partial class SimpleModelTestsMapper
{
    [GenerateMapping]
    public static partial SimpleModelTests.Person2 Map(this SimpleModelTests.Person1 person1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithPhoneType2 Map(this SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalPhoneType2 MapToOptionalPhoneType(this SimpleModelTests.PersonWithPhoneType1 personWithPhoneType1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalName2 Map(this SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalName2 MapToOptionalName(this SimpleModelTests.Person1 person1);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithOptionalAge2 MapToOptionalAge(this SimpleModelTests.Person1 person1);

    [GenerateMapping]
    public static partial SimpleModelTests.GeneralErrorCode Map(this SimpleModelTests.SpecificErrorCode specificErrorCode);

    [GenerateMapping]
    public static partial SimpleModelTests.PersonWithUnspecifiedName2 MapToUnspecifiedName(this SimpleModelTests.PersonWithOptionalName1 personWithOptionalName1);
    
#pragma warning disable CS0618 // allow obsolete class usage for ObsoleteClassesShouldBeMappedTest
    [GenerateMapping]
    public static partial SimpleModelTests.ObsoletePerson2 MapToObsolete(this SimpleModelTests.Person1 person1);
#pragma warning restore
}