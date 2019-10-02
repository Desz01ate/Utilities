using System;

namespace MachineLearning.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LabelColumn : Attribute
    {
    }
}
