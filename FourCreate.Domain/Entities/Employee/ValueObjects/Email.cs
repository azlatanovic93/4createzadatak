using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace FourCreate.Domain.Entities.Employee.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        public string Value { get; private set; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email None => new Email(string.Empty);

        public static Result<Email> Create(string email)
        {
            if (email is null)
                return Result.Failure<Email>("Email must have a value");

            if (email == string.Empty)
                return Result.Failure<Email>("Email cannot be empty");

            if (email.Length > 64)
                return Result.Failure<Email>("Email can be up to 64 characters");

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return Result.Failure<Email>("Email is invalid");


            return Result.Success(new Email(email.ToUpper()));
        }


        public bool Equals(Email? other) =>
            other != null && Value == other.Value;

        public override bool Equals(object? obj) =>
            Equals(obj as Email);

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static bool operator ==(Email? left, Email? right) =>
            ReferenceEquals(left, null) && ReferenceEquals(right, null) ||
            !ReferenceEquals(left, null) && left.Equals(right);

        public static bool operator !=(Email? left, Email? right) => !(left == right);

        public override string ToString() => $"{Value}";

    }
}
