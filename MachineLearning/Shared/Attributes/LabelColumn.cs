using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LabelColumn : Attribute
    {
    }
}
