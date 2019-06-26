using MachineLearning.Shared.Attributes;
using MachineLearning.Shared.DataStructures;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MachineLearning.Shared
{
    public static class Preprocessing
    {
        public static PropertyInfo LabelColumn(this IEnumerable<PropertyInfo> properties)
        {
            var labelProperty = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<LabelColumn>(true);
                if (attribute != null) return true;
                return false;
            });
            if (labelProperty == null) throw new AttributeException("LabelColumn");
            if (labelProperty.Count() > 1) throw new InvalidMultipleAttributesException("LabelColumn");
            return labelProperty.First();
        }
        public static IEnumerable<PropertyInfo> ExcludeColumns(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(property =>
            {
                var excludeAttr = property.GetCustomAttribute<ExcludeColumn>(true);
                var labelAttr = property.GetCustomAttribute<LabelColumn>(true);
                if (excludeAttr == null && labelAttr == null) return true;
                return false;
            });
        }
        public static OneHotEncodingPreprocessor OneHotEncoding(this MLContext context, IEnumerable<PropertyInfo> properties, string encodedFormat = "{0}_encoded")
        {
            var ohePreprocessor = new OneHotEncodingPreprocessor();
            var features = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<OneHotEncodingColumn>(true);
                if (attribute == null) return true;
                return false;
            }).Select(property => property.Name);

            var needToEncodeFeatures = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<OneHotEncodingColumn>(true);
                if (attribute != null) return true;
                return false;
            }).Select(property => property.Name);

            var oheEstimator = new EstimatorChain<OneHotEncodingTransformer>();
            List<CombinedFeature> combinedFeatures = new List<CombinedFeature>();
            foreach (var feature in needToEncodeFeatures)
            {
                var encoded = string.Format(encodedFormat, feature);
                oheEstimator = oheEstimator.Append(context.Transforms.Categorical.OneHotEncoding(inputColumnName: feature, outputColumnName: encoded));
                combinedFeatures.Add(new CombinedFeature
                {
                    Feature = feature,
                    EncodedFeature = encoded
                });
            }
            ohePreprocessor.OneHotEncodingEstimator = oheEstimator;
            ohePreprocessor.CombinedFeatures = Shared.Enumerator.CombineEnumerable(features, combinedFeatures.Select(x => x.EncodedFeature));
            return ohePreprocessor;
        }
        public static KeyToValueMappingPreprocessor KeyToValueMapping(this MLContext context, IEnumerable<PropertyInfo> properties, string encodedFormat = "{0}")
        {
            var ktvPreprocessor = new KeyToValueMappingPreprocessor();
            var features = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<KeyToValueColumn>(true);
                if (attribute == null) return true;
                return false;
            }).Select(property => property.Name);

            var needToEncodeFeatures = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<KeyToValueColumn>(true);
                if (attribute != null) return true;
                return false;
            }).Select(property => property.Name);

            var vtkEstimator = new EstimatorChain<KeyToValueMappingTransformer>();
            List<CombinedFeature> combinedFeatures = new List<CombinedFeature>();
            foreach (var feature in needToEncodeFeatures)
            {
                var encoded = string.Format(encodedFormat, feature);
                vtkEstimator = vtkEstimator.Append(context.Transforms.Conversion.MapKeyToValue(inputColumnName: feature, outputColumnName: encoded));
                combinedFeatures.Add(new CombinedFeature
                {
                    Feature = feature,
                    EncodedFeature = encoded
                });
            }
            ktvPreprocessor.KeyToValueMappingEstimator = vtkEstimator;
            ktvPreprocessor.CombinedFeatures = Shared.Enumerator.CombineEnumerable(features, combinedFeatures.Select(x => x.EncodedFeature));
            return ktvPreprocessor;
        }
        public static ValueToKeyMappingPreprocessor ValueToKeyMapping(this MLContext context, IEnumerable<PropertyInfo> properties, string encodedFormat = "{0}")
        {
            var vtkPreprocessor = new ValueToKeyMappingPreprocessor();
            var features = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<ValueToKeyColumn>(true);
                if (attribute == null) return true;
                return false;
            }).Select(property => property.Name);

            var needToEncodeFeatures = properties.Where(property =>
            {
                var attribute = property.GetCustomAttribute<ValueToKeyColumn>(true);
                if (attribute != null) return true;
                return false;
            }).Select(property => property.Name);

            var vtkEstimator = new EstimatorChain<ValueToKeyMappingTransformer>();
            List<CombinedFeature> combinedFeatures = new List<CombinedFeature>();
            foreach (var feature in needToEncodeFeatures)
            {
                var encoded = string.Format(encodedFormat, feature);
                vtkEstimator = vtkEstimator.Append(context.Transforms.Conversion.MapValueToKey(inputColumnName: feature, outputColumnName: encoded));
                combinedFeatures.Add(new CombinedFeature
                {
                    Feature = feature,
                    EncodedFeature = encoded
                });
            }
            vtkPreprocessor.ValueToKeyMappingEstimator = vtkEstimator;
            vtkPreprocessor.CombinedFeatures = Shared.Enumerator.CombineEnumerable(features, combinedFeatures.Select(x => x.EncodedFeature));
            return vtkPreprocessor;
        }
        public static IEnumerable<T> MinMaxScale<T>(this IEnumerable<T> dataset)
        {
            var minMaxArray = dataset.ToArray();
            foreach (var property in typeof(T).GetProperties())
            {
                var labelColumn = property.GetCustomAttribute<MinMaxScaleColumn>(true);
                if ((property.PropertyType == typeof(float) || property.PropertyType == typeof(float?)) && labelColumn != null)
                {
                    var min = dataset.Min(x => (float)property.GetValue(x));
                    var max = dataset.Max(x => (float)property.GetValue(x));
                    for (var index = 0; index < minMaxArray.Length; index++)
                    {
                        var element = minMaxArray[index];
                        var scaled = ((float)property.GetValue(element) - min) / (max - min);
                        property.SetValue(element, scaled);
                    }
                }
            }
            return minMaxArray;
        }
        public static IEnumerable<T> Normalization<T>(this IEnumerable<T> dataset)
        {
            var normArray = dataset.ToArray();
            foreach (var property in typeof(T).GetProperties())
            {
                var normColumn = property.GetCustomAttribute<NormalizationColumn>(true);
                if ((property.PropertyType == typeof(float) || property.PropertyType == typeof(float?)) && normColumn != null)
                {
                    var min = dataset.Min(x => (float)property.GetValue(x));
                    var max = dataset.Max(x => (float)property.GetValue(x));
                    var mean = dataset.Average(x => (float)property.GetValue(x));
                    for (var index = 0; index < normArray.Length; index++)
                    {
                        var element = normArray[index];
                        var norm = ((float)property.GetValue(element) - mean) / (max - min);
                        property.SetValue(element, norm);
                    }
                }
            }
            return normArray;
        }
    }
}
