using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.Markers;
using Xunit;

namespace MappingSourceGenerator.IntegrationTests;

public class SimpleCollectionTests
{
    [Fact]
    public void PrimitiveTypeReadOnlyCollectionTest()
    {
        var person1 = new PersonWithEmails1("Bob", new[] {"bob@gmail.com", "bob123@gmail.com"});
        
        var person2 = person1.Map();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Emails.Should().BeEquivalentTo(person1.Emails);
        }
    }

    public record PersonWithEmails1(
        string Name,
        IReadOnlyCollection<string> Emails);

    public record PersonWithEmails2(
        string Name,
        IReadOnlyCollection<string> Emails);
    
    [Fact]
    public void PrimitiveTypeListTest()
    {
        var person1 = new PersonWithEmailList1("Bob", ["bob@gmail.com", "bob123@gmail.com"]);
        
        var person2 = person1.Map();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Emails.Should().BeEquivalentTo(person1.Emails);
        }
    }

    public record PersonWithEmailList1(
        string Name,
        List<string> Emails);

    public record PersonWithEmailList2(
        string Name,
        List<string> Emails);

    public static readonly IEnumerable<object[]> EnumTestData = new List<object[]>
    {
        new object[]
        {
            "Rob",
            new[] { PhoneType1.Android, PhoneType1.Ios },
            new[] { PhoneType2.Android, PhoneType2.Ios },
        },
        new object[]
        {
            "Bob",
            new[] { PhoneType1.Other },
            new[] { PhoneType2.Other },
        },
        new object[]
        {
            "Eugene",
            Array.Empty<PhoneType1>(),
            Array.Empty<PhoneType2>(),
        },
    };

    [Theory]
    [MemberData(nameof(EnumTestData))]
    public void EnumEnumerableTest(
        string name,
        IEnumerable<PhoneType1> sourcePhoneTypes,
        IEnumerable<PhoneType2> expectedPhoneTypes)
    {
        var personWithPhoneType1 = new PersonWithPhoneTypes1(name, sourcePhoneTypes);
        
        var personWithPhoneType2 = personWithPhoneType1.Map();

        using (new AssertionScope())
        {
            personWithPhoneType2.Name.Should().Be(personWithPhoneType1.Name);
            personWithPhoneType2.PhoneTypes.Should().BeEquivalentTo(expectedPhoneTypes);
        }
    }

    public record PersonWithPhoneTypes1(
        string Name,
        IEnumerable<PhoneType1> PhoneTypes);

    public enum PhoneType1
    {
        Android,
        Ios,
        Other,
    }

    public record PersonWithPhoneTypes2(
        string Name,
        IEnumerable<PhoneType2> PhoneTypes);

    public enum PhoneType2
    {
        Android,
        Ios,
        Other,
    }

    [Fact]
    public void ReadOnlyCollectionToNullableReadOnlyCollectionTest()
    {
        var person1 = new PersonWithEmails1("Bob", new[] {"bob@gmail.com", "bob123@gmail.com"});
        
        var person2 = person1.MapToOptional();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Emails.Should().BeEquivalentTo(person1.Emails);
        }
    }

    public record PersonWithOptionalEmails2(
        string Name,
        IReadOnlyCollection<string>? Emails);

    [Fact]
    public void PrimitiveTypeArrayTest()
    {
        var person1 = new PersonWithEmailArray1("Bob", ["bob@gmail.com", "bob123@gmail.com"]);
        
        var person2 = person1.Map();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Emails.Should().BeEquivalentTo(person1.Emails);
        }
    }
    
    public record PersonWithEmailArray1(
        string Name,
        string[] Emails);
                
    public record PersonWithEmailArray2(
        string Name,
        string[] Emails);

    [Fact]
    public void PrimitiveTypeReadOnlyListToReadOnlyCollectionTest()
    {
        var person1 = new PersonWithEmailReadOnlyList1("Bob", ["bob@gmail.com", "bob123@gmail.com"]);
        
        var person2 = person1.Map();

        using (new AssertionScope())
        {
            person2.Name.Should().Be(person1.Name);
            person2.Emails.Should().BeEquivalentTo(person1.Emails);
        }
    }
    
    public record PersonWithEmailReadOnlyList1(
        string Name,
        IReadOnlyList<string> Emails);

    public record PersonWithEmailReadOnlyCollection2(
        string Name,
        IReadOnlyCollection<string> Emails);
}

public static partial class SimpleCollectionTestsMapper
{
    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithEmails2 Map(this SimpleCollectionTests.PersonWithEmails1 personWithEmails1);
    
    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithEmailList2 Map(this SimpleCollectionTests.PersonWithEmailList1 personWithEmailList1);

    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithPhoneTypes2 Map(this SimpleCollectionTests.PersonWithPhoneTypes1 personWithPhoneTypes1);

    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithOptionalEmails2 MapToOptional(this SimpleCollectionTests.PersonWithEmails1 personWithEmails1);
    
    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithEmailArray2 Map(this SimpleCollectionTests.PersonWithEmailArray1 personWithEmailArray1);
    
    [GenerateMapping]
    public static partial SimpleCollectionTests.PersonWithEmailReadOnlyCollection2 Map(this SimpleCollectionTests.PersonWithEmailReadOnlyList1 personWithEmailReadOnlyList1);
}