namespace EDSc.Common.Services.Saving.Utils
{
    public interface IImageToDbWriter<T>
    {
        string SaveToDb(T img);
    }
}
