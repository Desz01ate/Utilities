using System;

namespace MachineLearning.Shared.Constants.IDataView
{
    public struct Regression
    {
        /// <summary>
        /// Label: Original regression value of the example.
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// Score: Predicted regression value.
        /// </summary>
        public const string Score = "Score";
        /// <summary>
        /// PCAFeatures : Principal-Component-Analysis features.
        /// </summary>
        public const string PCAFeatures = "PCAFeatures";
        /// <summary>
        /// Unverified column name.
        /// </summary>
        [Obsolete("Unverified column name.")]
        public const string LastName = "LastName";
    }
    public struct BinaryClassification
    {
        /// <summary>
        /// Label: Original Label of the example.
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// Score: Raw score from the learner (e.g. value before applying sigmoid function to get probability).
        /// </summary>
        public const string Score = "Score";
        /// <summary>
        /// Probability: Probability of being in certain class
        /// </summary>
        public const string Probability = "Probability";
        /// <summary>
        /// PredictedLabel: Predicted class.
        /// </summary>
        public const string PredictedLabel = "PredictedLabel";
        /// <summary>
        /// PCAFeatures : Principal-Component-Analysis features.
        /// </summary>
        public const string PCAFeatures = "PCAFeatures";
        /// <summary>
        /// Unverified column name.
        /// </summary>
        [Obsolete("Unverified column name.")]
        public const string LastName = "LastName";
    }
    public struct MulticlassClassification
    {
        /// <summary>
        /// Label: Original Label of the example.
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// Score: Its an array whose length is equal to number of classes and contains probability for each class.
        /// </summary>
        public const string Score = "Score";
        /// <summary>
        /// PredictedLabel: Predicted class.
        /// </summary>
        public const string PredictedLabel = "PredictedLabel";
        /// <summary>
        /// PCAFeatures : Principal-Component-Analysis features.
        /// </summary>
        public const string PCAFeatures = "PCAFeatures";
        /// <summary>
        /// Unverified column name.
        /// </summary>
        [Obsolete("Unverified column name.")]
        public const string LastName = "LastName";
    }
    public struct Clustering
    {
        /// <summary>
        /// Original cluster Id of the example.
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// Score: Its an array whose length is equal to number of clusters. It contains square distance from the cluster centeriod.
        /// </summary>
        public const string Score = "Score";
        /// <summary>
        /// PredictedLabel: Predicted cluster Id.
        /// </summary>
        public const string PredictedLabel = "PredictedLabel";
        /// <summary>
        /// PCAFeatures : Principal-Component-Analysis features.
        /// </summary>
        public const string PCAFeatures = "PCAFeatures";
        /// <summary>
        /// Unverified column name.
        /// </summary>
        [Obsolete("Unverified column name.")]
        public const string LastName = "LastName";
    }
}
