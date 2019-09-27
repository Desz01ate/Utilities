using System;

namespace Utilities.Exceptions
{
    /// <summary>
    /// Express attribute exception throw for internal details.
    /// </summary>
    public class AttributeException : Exception
    {
        /// <summary>
        /// Type that cause the exception.
        /// </summary>
        public Type CausedByAttribute { get; }

        /// <summary>
        /// Occured on caller class.
        /// </summary>
        public string CallerClass { get; }

        /// <summary>
        /// Inner exception details.
        /// </summary>
        public new Exception InnerException { get; }

        //public AttributeException(string attributeName, string callerClass) : base($"Can't find attribute [{attributeName}] in {callerClass} class.")
        //{
        //}
        public AttributeException(Type attribute, string callerClass, Exception innerException) : base($"Can't find attribute [{attribute?.Name}] in {callerClass} class.")
        {
            CausedByAttribute = attribute;
            CallerClass = callerClass;
            InnerException = innerException;
        }
    }

    /// <summary>
    /// Express non-multiple attribute exception throw for internal details.
    /// </summary>
    public class InvalidMultipleAttributesException : Exception
    {
        /// <summary>
        /// Type that cause the exception.
        /// </summary>
        public Type CausedByAttribute { get; }

        /// <summary>
        /// Occured on caller class.
        /// </summary>
        public string CallerClass { get; }

        /// <summary>
        /// Inner exception details.
        /// </summary>
        public new Exception InnerException { get; }

        public InvalidMultipleAttributesException(Type attribute, string callerClass, Exception innerException) : base($"The attribute [{attribute?.Name}] must specific one and only one. (error in {callerClass} class)")
        {
            CausedByAttribute = attribute;
            CallerClass = callerClass;
            InnerException = innerException;
        }
    }
}