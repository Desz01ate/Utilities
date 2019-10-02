using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System.Collections.Generic;

namespace MachineLearning.Shared.DataStructures
{
    public class OneHotEncodingPreprocessor
    {
        public EstimatorChain<OneHotEncodingTransformer> OneHotEncodingEstimator { get; set; }
        public IEnumerable<string> CombinedFeatures { get; set; }
    }
}
