namespace EDSc.Common.Services.Classification.Model 
{
    using Microsoft.ML.Data;
    
    public class ImagePrediction
    {
        [ColumnName("Score")]
        public float[] Score { get; set; }

        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; }
    }   
}