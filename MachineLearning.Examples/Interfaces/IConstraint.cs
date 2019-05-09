using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Examples.Interfaces
{
    public interface IConstraint
    {
        uint actual_uint_result();
        float actual_float_result();
        string actual_string_result();
    }
}
