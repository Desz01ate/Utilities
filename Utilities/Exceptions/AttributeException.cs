using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Exceptions
{
    public class AttributeException : Exception
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
