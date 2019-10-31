using System;

namespace Common.SignalR.Refs
{
    public class SimpleLazyFactory<T>
    {
        private Lazy<T> LazyInstance = null;

        private SimpleLazyFactory()
        {
        }

        public Func<T> Resolve { get; private set; }

        public SimpleLazyFactory<T> Default(Func<T> create)
        {
            if (ResetInvoked)
            {
                //if reset is invoked by di, default should ignore
                return this;
            }

            if (LazyInstance != null)
            {
                throw new InvalidOperationException("Default should not invoked more than once!");
            }

            LazyInstance = new Lazy<T>(create);
            Resolve = () => this.LazyInstance.Value;
            return this;
        }

        public static SimpleLazyFactory<T> Instance = new SimpleLazyFactory<T>();

        #region for di extensions

        public bool ResetInvoked { get; private set; }

        public SimpleLazyFactory<T> Reset(Func<T> create)
        {
            LazyInstance = new Lazy<T>(create);
            Resolve = () => this.LazyInstance.Value;
            ResetInvoked = true;
            return this;
        }

        #endregion

        #region for test only
        
        public static SimpleLazyFactory<T> CreateForTest()
        {
            return new SimpleLazyFactory<T>();
        }
        
        #endregion
    }

    #region demo

    //public interface IServiceGuard
    //{

    //}

    //public class ServiceGuard : IServiceGuard
    //{
    //    #region for di extensions

    //    public static Func<IServiceGuard> Resolve { get; } = SimpleLazyFactory<IServiceGuard>.Instance.Default(() => new ServiceGuard()).Resolve;

    //    #endregion
    //}

    #endregion
}
