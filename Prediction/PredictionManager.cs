namespace Divine.SDK.Prediction
{
    public static class PredictionManager
    {
        private static readonly Prediction Prediction = new Prediction();

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return Prediction.GetPrediction(input);
        }
    }
}