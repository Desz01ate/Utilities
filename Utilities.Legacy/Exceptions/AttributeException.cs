using System;

namespace Utilities.Legacy.Exceptions
{
    class AttributeException : Exception
    {
        public AttributeException(string attributeName) : base(string.Format("Can't find attribute [{0}] on model class.", attributeName))
        {

        }
    }
    public class InvalidMultipleAttributesException : Exception
    {
        public InvalidMultipleAttributesException(string attributeName) : base(string.Format("The attribute [{0}] can't be specified more than one property.", attributeName))
        {

        }
    }
}
