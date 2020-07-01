using System;

namespace Common.Scopes
{
    public static class ScopeExtensions
    {
        public static T GetItemAs<T>(this ScopeContext ctx, string key, T defaultValue = default(T))
        {
            var value = ctx.GetItem(key, defaultValue);
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}