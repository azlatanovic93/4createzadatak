using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace FourCreate.Domain.Entities.Common.Enumerations
{
    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class Enumeration<TEnumeration> : Enumeration<TEnumeration, int>
        where TEnumeration : Enumeration<TEnumeration>
    {
        protected Enumeration(int value, string displayName) : base(value, displayName)
        {
        }

        public static TEnumeration FromInt32(int value) => FromValue(value);

        public static bool TryFromInt32(int listItemValue, out TEnumeration result) => TryParse(listItemValue, out result);
    }

    [Serializable]
    [DebuggerDisplay("{DisplayName} - {Value}")]
    public abstract class Enumeration<TEnumeration, TValue> : IComparable<TEnumeration>, IEquatable<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration, TValue>
        where TValue : IComparable
    {
        private static readonly Lazy<TEnumeration[]> Enumerations = new Lazy<TEnumeration[]>(GetEnumerations);

        [DataMember(Order = 1)] readonly string _displayName;

        [DataMember(Order = 0)] readonly TValue _value;

        protected Enumeration(TValue value, string displayName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
            _displayName = displayName;
        }

        public TValue Value
        {
            get { return _value; }
        }

        public string DisplayName => _displayName;

        public int CompareTo(TEnumeration other) => Value.CompareTo(other == default(TEnumeration) ? default : other.Value);

        public override sealed string ToString() => DisplayName;

        public static TEnumeration[] GetAll() => Enumerations.Value;

        private static TEnumeration[] GetEnumerations()
        {
            Type enumerationType = typeof(TEnumeration);
            return enumerationType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => enumerationType.IsAssignableFrom(info.FieldType))
                .Select(info => info.GetValue(null))
                .Cast<TEnumeration>()
                .ToArray();
        }

        public override bool Equals(object obj) => Equals(obj as TEnumeration);

        public bool Equals(TEnumeration other) => other != null && ValueEquals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Enumeration<TEnumeration, TValue> left, Enumeration<TEnumeration, TValue> right) => Equals(left, right);

        public static bool operator !=(Enumeration<TEnumeration, TValue> left, Enumeration<TEnumeration, TValue> right) => !Equals(left, right);

        /// <summary>
        /// Returns <typeparamref name="TEnumeration"/> if a value exists.
        /// If the value doesn't exist, it will raise an <seealso cref="ArgumentException"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnumeration FromValue(TValue value) => Parse(value, "value", item => item.Value.Equals(value));

        /// <summary>
        /// Returns <typeparamref name="TEnumeration"/> if a value exists.
        /// If the value doesn't exist, it will return null. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnumeration FromNullableValue(TValue value) => ParseFromNullable(value, "value", item => item.Value.Equals(value));

        /// <summary>
        /// Returns <typeparamref name="TEnumeration"/> if a name exists.
        /// If the name doesn't exist, it will return null. 
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static TEnumeration FromNullableName(string displayName) => ParseFromNullable(displayName, "display name", item => item.DisplayName == displayName);

        /// <summary>
        /// Returns <typeparamref name="TEnumeration"/> if a name exists.
        /// If the name doesn't exist, it will raise an <seealso cref="ArgumentException"/>.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static TEnumeration ParseFromName(string displayName) => Parse(displayName, "display name", item => item.DisplayName == displayName);

        static bool TryParse(Func<TEnumeration, bool> predicate, out TEnumeration result)
        {
            result = GetAll().FirstOrDefault(predicate);
            return result != null;
        }

        private static TEnumeration Parse(object value, string description, Func<TEnumeration, bool> predicate)
        {
            TEnumeration result;

            if (!TryParse(predicate, out result))
            {
                string message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(TEnumeration));
                throw new ArgumentException(message, "value");
            }

            return result;
        }

        private static TEnumeration ParseFromNullable(object value, string description, Func<TEnumeration, bool> predicate)
        {
            TEnumeration result;

            if (!TryParse(predicate, out result))
                return null;

            return result;
        }

        public static bool TryParse(TValue value, out TEnumeration result) => TryParse(e => e.ValueEquals(value), out result);

        public static bool TryParse(string displayName, out TEnumeration result) => TryParse(e => e.DisplayName == displayName, out result);

        protected virtual bool ValueEquals(TValue value) => Value.Equals(value);
    }

}


