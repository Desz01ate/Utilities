using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities.MachineLearning
{
    public static class Global
    {
        public static void SaveModel(this ITransformer model, Stream stream)
        {
            var context = new MLContext();
            context.Model.Save(model, stream);
        }
    }
}
