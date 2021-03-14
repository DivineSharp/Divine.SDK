namespace Divine.SDK.Prediction
{
    public interface IPrediction
    {
        PredictionOutput GetPrediction(PredictionInput input);
    }
}