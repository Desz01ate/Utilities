using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Exceptions
{
    public class AttributeException : Exception
    {
        public AttributeException(string attributeName, string callerClass) : base($"Can't find attribute [{attributeName}] in {callerClass} class.")
        {

        }
    }
    public class InvalidMultipleAttributesException : Exception
    {
        public InvalidMultipleAttributesException(string attributeName, string callerClass) : base($"The attribute [{attributeName}] must specific one and only one. (error in {callerClass} class)")
        {

        }
    }
}
