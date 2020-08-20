namespace EDSc.Common.Services.Deployment.Util
{
    public interface IConfigConverter<in TInput, out TOutput>
    {
        TOutput Convert(TInput config);
    }
}
