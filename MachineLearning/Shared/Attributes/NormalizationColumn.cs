using System;

namespace MachineLearning.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class NormalizationColumn : Attribute
    {
    }
}
