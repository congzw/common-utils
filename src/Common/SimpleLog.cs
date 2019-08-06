using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common
{
    //内部使用的简单日志
    public interface ISimpleLog
    {
        SimpleLogLevel EnabledLevel { get; set; }
        Task Log(object message, SimpleLogLevel level);
    }

    public enum SimpleLogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

    internal class SimpleLog : ISimpleLog
    {
        public string Category { get; set; }

        public SimpleLogLevel EnabledLevel { get; set; }

        public Task Log(object message, SimpleLogLevel level)
        {
            if (this.ShouldLog(level))
            {
                LogMessage(message, level);
            }
            return Task.FromResult(0);
        }

        protected virtual void LogMessage(object message, SimpleLogLevel level)
        {
            Trace.WriteLine(string.Format("[{0}][{1}][{2}] {3}", "SimpleLog", Category, level.ToString(), message));
        }
    }
    
    public static class SimpleLogExtensions
    {
        public static bool ShouldLog(this ISimpleLog simpleLog, SimpleLogLevel level)
        {
            return level >= simpleLog.EnabledLevel;
        }
    }

    #region factory and settings

    public interface ISimpleLogFactory
    {
        ISimpleLog Create(string category);
    }

    public class SimpleLogFactory : ISimpleLogFactory
    {
        public SimpleLogFactory(SimpleLogSettings settings)
        {
            Settings = settings;
        }

        public SimpleLogSettings Settings { get; set; }

        public ISimpleLog Create(string category)
        {
            var tryFixCategory = Settings.TryFixCategory(category);
            var simpleLogLevel = Settings.GetEnabledLevel(tryFixCategory);
            return new SimpleLog() { Category = tryFixCategory, EnabledLevel = simpleLogLevel };
        }

        #region for di extensions

        private static Lazy<ISimpleLogFactory> LazyInstance = new Lazy<ISimpleLogFactory>(() => new SimpleLogFactory(new SimpleLogSettings()));
        public static Func<ISimpleLogFactory> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    public class SimpleLogSettings
    {
        public SimpleLogSettings()
        {
            Items = new ConcurrentDictionary<string, SimpleLogSetting>(StringComparer.OrdinalIgnoreCase);
            Default = new SimpleLogSetting() { Category = DefaultCategory, EnabledLevel = SimpleLogLevel.Trace };
        }

        public void SetEnabledLevel(string category, SimpleLogLevel level)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            var key = category.Trim();
            var tryGetValue = Items.TryGetValue(key, out var setting);
            if (!tryGetValue || setting == null)
            {
                setting = new SimpleLogSetting();
                setting.Category = key;
                Items.Add(key, setting);
            }
            setting.EnabledLevel = level;
        }

        public SimpleLogLevel GetEnabledLevel(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            var key = category.Trim();
            var tryGetValue = Items.TryGetValue(key, out var setting);
            if (!tryGetValue || setting == null)
            {
                //todo:try find first by key start with?
                return Default.EnabledLevel;
            }
            return setting.EnabledLevel;
        }

        public string TryFixCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return DefaultCategory;
            }

            return category.Trim();
        }

        private SimpleLogSetting _default;

        public SimpleLogSetting Default
        {
            get { return _default; }
            set
            {
                if (value == null || value.Category != DefaultCategory)
                {
                    throw new InvalidOperationException();
                }

                _default = value;
                SetEnabledLevel(DefaultCategory, value.EnabledLevel);
            }
        }

        public static string DefaultCategory = "Default";
        public IDictionary<string, SimpleLogSetting> Items { get; set; }
    }

    public class SimpleLogSetting
    {
        public string Category { get; set; }
        public SimpleLogLevel EnabledLevel { get; set; }
    }
    
    public static class SimpleLogFactoryExtensions
    {
        public static ISimpleLog CreateLogFor(this SimpleLogFactory factory, Type type)
        {
            return factory.Create(type.Name);
        }

        public static ISimpleLog CreateLogFor<T>(this SimpleLogFactory factory)
        {
            return factory.CreateLogFor(typeof(T));
        }

        public static ISimpleLog CreateLogFor(this SimpleLogFactory factory, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (instance is Type type)
            {
                return factory.CreateLogFor(type);
            }
            return factory.CreateLogFor(instance.GetType());
        }
    }

    #endregion
}
