namespace MappingSourceGenerator.Benchmarks;

public static class SourceCodeConstants
{
    public static readonly string AllMappingSource = """
        using System.Collections.Generic;
        using System.Linq;
        using MappingSourceGenerator.Markers;

        namespace Benchmarks;

        public static partial class ManualMappingTestsMapper
        {
            [GenerateMapping]
            public static partial PersonWithCarWithType2 Map(this PersonWithCarWithType1 personWithCarWithType1);

            public static CarType2 Map(
                this CarType1 carType1)
                => carType1 switch
                {
                    CarType1.Electric => CarType2.Electric,
                    CarType1.Petrol => CarType2.Petrol,
                    _ => throw new InvalidOperationException($"Unable to map Benchmarks.ManualMappingTests.CarType1.{carType1}"),
                };
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

        public record PersonWithCars1(
            string Name,
            IReadOnlyCollection<Car1> Cars);

        public record PersonWithCars2(
            string Name,
            IReadOnlyCollection<Car2> Cars);

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

        public record PersonWithNullableCar1(
            string Name,
            Car1? Car);

        public record PersonWithNullableCar2(
            string Name,
            Car2? Car);

        public static partial class NestedModelTestsMapper
        {
            [GenerateMapping]
            public static partial PersonWithCar2 Map(this PersonWithCar1 personWithCar1);

            [GenerateMapping]
            public static partial PersonWithCars2 Map(this PersonWithCars1 personWithCars1);

            [GenerateMapping]
            public static partial PersonWithCarWithType2 Map(this PersonWithCarWithType1 personWithCarWithType1);

            [GenerateMapping]
            public static partial PersonWithNullableCar2 Map(this PersonWithNullableCar1 personWithNullableCar1);
        }

        public record PersonWithEmails1(
            string Name,
            IReadOnlyCollection<string> Emails);

        public record PersonWithEmails2(
            string Name,
            IReadOnlyCollection<string> Emails);

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

        public record PersonWithOptionalEmails2(
            string Name,
            IReadOnlyCollection<string>? Emails);

        public static partial class SimpleCollectionTestsMapper
        {
            [GenerateMapping]
            public static partial PersonWithEmails2 Map(this PersonWithEmails1 personWithEmails1);

            [GenerateMapping]
            public static partial PersonWithPhoneTypes2 Map(this PersonWithPhoneTypes1 personWithPhoneTypes1);

            [GenerateMapping]
            public static partial PersonWithOptionalEmails2 MapToOptional(this PersonWithEmails1 personWithEmails1);
        }

        public record Person1(
            string Name,
            int Age);

        public record Person2(
            string Name,
            int Age);

        public record PersonWithPhoneType1(
            string Name,
            PhoneType1 PhoneType);

        public record PersonWithPhoneType2(
            string Name,
            PhoneType2 PhoneType);

        public record PersonWithOptionalName1(
            string? Name,
            int Age);

        public record PersonWithOptionalName2(
            string? Name,
            int Age);

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

        #nullable disable
            public record PersonWithUnspecifiedName2(
                string Name,
                int Age);
        #nullable restore

        public static partial class SimpleModelTestsMapper
        {
            [GenerateMapping]
            public static partial Person2 Map(this Person1 person1);

            [GenerateMapping]
            public static partial PersonWithPhoneType2 Map(this PersonWithPhoneType1 personWithPhoneType1);

            [GenerateMapping]
            public static partial PersonWithOptionalName2 Map(this PersonWithOptionalName1 personWithOptionalName1);

            [GenerateMapping]
            public static partial PersonWithOptionalName2 MapToOptionalName(this Person1 person1);

            [GenerateMapping]
            public static partial GeneralErrorCode Map(this SpecificErrorCode specificErrorCode);

            [GenerateMapping]
            public static partial PersonWithUnspecifiedName2 MapToUnspecifiedName(this PersonWithOptionalName1 personWithOptionalName1);
        }
        """;
}