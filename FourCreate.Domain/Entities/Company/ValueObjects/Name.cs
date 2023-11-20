using CSharpFunctionalExtensions;

namespace FourCreate.Domain.Entities.Company.ValueObjects
{
    public class Name
    {
        public string Value { get; private set; }

        private Name(string value)
        {
            Value = value;
        }

        public static Name None => new Name(string.Empty);

        public static Result<Name> Create(string name)
        {
            if (name is null)
                return Result.Failure<Name>("Name must have a value");

            if (name == string.Empty)
                return Result.Failure<Name>("Name cannot be empty");

            if (name.Length > 64)
                return Result.Failure<Name>("Name can be up to 64 characters");

            return Result.Success(new Name(name));
        }

        public bool Equals(Name? other) =>
            other != null && Value == other.Value;

        public override bool Equals(object? obj) =>
            Equals(obj as Name);

        public override int GetHashCode() =>
            Value.GetHashCode();

        public static bool operator ==(Name? left, Name? right) =>
            ReferenceEquals(left, null) && ReferenceEquals(right, null) ||
            !ReferenceEquals(left, null) && left.Equals(right);

        public static bool operator !=(Name? left, Name? right) => !(left == right);

        public override string ToString() => $"{Value}";
    }
}
