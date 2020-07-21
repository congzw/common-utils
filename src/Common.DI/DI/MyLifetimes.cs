// ReSharper disable once CheckNamespace
namespace Common
{
    public interface IMyLifetime
    {
    }

    public interface IMySingleton: IMyLifetime
    {

    }

    public interface IMyScoped: IMyLifetime
    {

    }

    public interface IMyTransient: IMyLifetime
    {

    }
}