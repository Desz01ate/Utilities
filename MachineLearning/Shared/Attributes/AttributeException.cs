using System;

namespace MachineLearning.Shared.Attributes
{
    class AttributeException : Exception
    {
        public AttributeException(string attributeName) : base(String.Format("Can't find attribute [{0}] on model class.", attributeName))
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
