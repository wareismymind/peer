using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Peer.Domain
{
    [ExcludeFromCodeCoverage]
    public class Validators
    {
        public const string NotLessThanOrEqualToZero = "Cannot be less than or equal to zero";
        public const string NotEmptyOrWhitespace = "Cannot be string.Empty or only whitespace";
        public const string NotGuidEmpty = "Cannot be Guid.Empty";
        public const string NotEmpty = "Cannot be empty";
        public const string UndefinedEnum = "Enum value cannot be undefined";

        public static void ArgNotLessThanOrEqualToZero(int value, string name)
        {
            if (value <= 0)
                throw new ArgumentException(NotLessThanOrEqualToZero, name);
        }

        public static void ArgIsNotNullEmptyOrWhitespace(string value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(NotEmptyOrWhitespace, name);
        }

        public static void ArgIsNotNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void ArgIsNotEmpty(Guid value, string name)
        {
            if (value == Guid.Empty)
                throw new ArgumentException(NotGuidEmpty, name);
        }

        public static void ArgIsNotEmpty(ICollection collection, string name)
        {
            if (collection?.Count == 0)
                throw new ArgumentException(NotEmpty, name);
        }

        public static void ArgIsNotNullOrEmpty(ICollection collection, string name)
        {
            ArgIsNotNull(collection, name);
            ArgIsNotEmpty(collection, name);
        }

        public static void ArgIsDefined<T>(T value, string name) where T : struct, Enum
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(UndefinedEnum, name);
        }
    }
}
