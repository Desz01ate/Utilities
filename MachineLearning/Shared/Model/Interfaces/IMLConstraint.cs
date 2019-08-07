using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.Model.Interfaces
{
    public interface IMLConstraint
    {
        uint Actual_uint_Result();
        float Actual_float_Result();
        string Actual_string_Result();
    }
}
