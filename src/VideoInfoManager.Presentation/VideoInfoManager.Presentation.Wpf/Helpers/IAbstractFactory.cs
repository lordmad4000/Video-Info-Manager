namespace VideoInfoManager.Presentation.Wpf.Helpers
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}