using CSharpFunctionalExtensions;
using FourCreate.Domain.Entities.Employee.ValueObjects;
using Shouldly;

namespace FourCreate.Test.Domain.Employee
{
    public class EmailTests
    {
        [Fact]
        public void Email_can_not_be_null()
        {
            Result<Email> EmailResult = Email.Create(null);
            EmailResult.IsFailure.ShouldBeTrue();
            EmailResult.Error.ShouldBe("Email must have a value");
        }

        [Fact]
        public void Email_can_not_be_empty()
        {
            Result<Email> EmailResult = Email.Create(string.Empty);
            EmailResult.IsFailure.ShouldBeTrue();
            EmailResult.Error.ShouldBe("Email cannot be empty");
        }

        [Fact]
        public void Email_can_not_be_longer_than_64_chars()
        {
            Result<Email> EmailResult = Email.Create(new string('b', 65));
            EmailResult.IsFailure.ShouldBeTrue();
            EmailResult.Error.ShouldBe("Email can be up to 64 characters");
        }

        [Fact]
        public void Email_can_not_be_invalid()
        {
            Result<Email> EmailResult = Email.Create("mydemo.com");
            EmailResult.IsFailure.ShouldBeTrue();
            EmailResult.Error.ShouldBe("Email is invalid");
        }

        [Fact]
        public void Email_happy_path()
        {
            Result<Email> EmailResult = Email.Create("my@demo.com");
            EmailResult.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public void Equality_test()
        {
            Result<Email> Email1Result = Email.Create("my@demo.com");
            Result<Email> Email2Result = Email.Create("my@demo.com");
            Email1Result.Value.ShouldBe(Email2Result.Value);
        }

    }
}
