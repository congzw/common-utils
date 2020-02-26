using Microsoft.EntityFrameworkCore;

namespace Common.UoW.EF
{
    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}

