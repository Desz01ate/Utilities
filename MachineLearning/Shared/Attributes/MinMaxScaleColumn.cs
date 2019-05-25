using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    class MinMaxScaleColumn : Attribute
    {
    }
}
