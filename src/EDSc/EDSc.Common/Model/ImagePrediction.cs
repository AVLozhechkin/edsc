using Microsoft.ML.Data;

namespace EDSc.Common.Model 
{
    public class ImagePrediction
    {
        [ColumnName("Score")]
        public float[] Score;

        [ColumnName("PredictedLabel")]
        public string PredictedLabel;
    }   
}