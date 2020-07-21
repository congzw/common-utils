using System;

namespace Common.DI
{
    public interface ILifetimeDesc : IMyLifetime
    {

    }

    public interface IMySingletonDesc : IMySingleton
    {

    }

    public interface IMyScopedDesc : IMyScoped
    {

    }

    public interface IMyTransientDesc : IMyTransient
    {

    }

    public abstract class LifetimeDesc
    {
        public override string ToString()
        {
            return this.GetHashCode().ToString();
        }

        public static string ShowDiff(ILifetimeDesc desc, ILifetimeDesc desc2)
        {
            return string.Format("[{0}, {1}] Same: {2}"
                , desc == null ? "NULL" : desc.ToString()
                , desc2 == null ? "NULL" : desc.ToString()
                , object.ReferenceEquals(desc, desc2));
        }
    }

    public interface IFooSingleton : IMySingletonDesc
    {

    }

    public class FooSingleton : LifetimeDesc, IFooSingleton
    {

    }

    public class FooScope : LifetimeDesc, IMyScoped, IDisposable
    {
        public Action DisposeInvoked { get; set; }

        public void Dispose()
        {
            DisposeInvoked?.Invoke();
        }
    }

    public class FooTransient : IMyTransient
    {

    }

    public class FooDefault : IMyLifetime
    {

    }

    public class FooIgnore : IMyTransient, IMyLifetimeIgnore
    {

    }

    public class Foo
    {

    }
}
