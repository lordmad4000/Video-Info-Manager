namespace VideoInfoManager.Domain.Interfaces;

public interface IEventHandler<T> where T : class
{
    public void Raise(T model);
}
