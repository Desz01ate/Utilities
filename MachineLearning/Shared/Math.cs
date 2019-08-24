using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearning.Shared
{
    public static class Math
    {
        public static float Range(this IEnumerable<float> dataset)
        {
            var max = dataset.Max();
            var min = dataset.Min();
            return max - min;
        }
        public static double Range(this IEnumerable<double> dataset)
        {
            var max = dataset.Max();
            var min = dataset.Min();
            return max - min;
        }
        public static int Range(this IEnumerable<int> dataset)
        {
            var max = dataset.Max();
            var min = dataset.Min();
            return max - min;
        }
        public static float Mean(this IEnumerable<float> dataset) => dataset.Average();
        public static double Mean(this IEnumerable<double> dataset) => dataset.Average();
        public static double Mean(this IEnumerable<int> dataset) => dataset.Average();
        public static float Median(this IEnumerable<float> dataset)
        {
            var sortedDataset = dataset.OrderBy(x => x);
            var elementsCount = sortedDataset.Count();
            var halfIdx = elementsCount / 2;
            if (elementsCount % 2 == 0)
            {
                var left = sortedDataset.ElementAt(halfIdx);
                var right = sortedDataset.ElementAt(halfIdx - 1);
                return (left + right) / 2;
            }
            return sortedDataset.ElementAt(halfIdx);
        }
        public static double Median(this IEnumerable<double> dataset)
        {
            var sortedDataset = dataset.OrderBy(x => x);
            var elementsCount = sortedDataset.Count();
            var halfIdx = elementsCount / 2;
            if (elementsCount % 2 == 0)
            {
                var left = sortedDataset.ElementAt(halfIdx);
                var right = sortedDataset.ElementAt(halfIdx - 1);
                return (left + right) / 2;
            }
            return sortedDataset.ElementAt(halfIdx);
        }
        public static int Median(this IEnumerable<int> dataset)
        {
            var sortedDataset = dataset.OrderBy(x => x);
            var elementsCount = sortedDataset.Count();
            var halfIdx = elementsCount / 2;
            if (elementsCount % 2 == 0)
            {
                var left = sortedDataset.ElementAt(halfIdx);
                var right = sortedDataset.ElementAt(halfIdx - 1);
                return (left + right) / 2;
            }
            return sortedDataset.ElementAt(halfIdx);
        }
        public static float Mode(this IEnumerable<float> dataset)
        {
            return dataset.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }
        public static double Mode(this IEnumerable<double> dataset)
        {
            return dataset.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }
        public static int Mode(this IEnumerable<int> dataset)
        {
            return dataset.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }
    }
}
