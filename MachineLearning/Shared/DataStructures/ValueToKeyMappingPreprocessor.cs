using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Collections.Generic;

namespace MachineLearning.Shared.DataStructures
{
    public class ValueToKeyMappingPreprocessor
    {
        public EstimatorChain<ValueToKeyMappingTransformer> ValueToKeyMappingEstimator { get; set; }
        public IEnumerable<string> CombinedFeatures { get; set; }
    }
}
