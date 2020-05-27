namespace EDSc.Common.Services.Deployment
{
    public interface IConfigConverter<TInput, TOutput>
    {
        TOutput Convert(TInput config);
    }
}
