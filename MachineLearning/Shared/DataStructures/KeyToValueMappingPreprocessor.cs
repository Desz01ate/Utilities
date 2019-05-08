using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Shared.DataStructures
{
    public class KeyToValueMappingPreprocessor
    {
        public EstimatorChain<KeyToValueMappingTransformer> KeyToValueMappingEstimator { get; set; }
        public IEnumerable<string> CombinedFeatures { get; set; }
    }
}
