// ReSharper disable CheckNamespace

using System.Collections.Generic;

namespace Common
{
    public interface ICombineKey<T> where T : ICombineKey<T>
    {
        IList<string> IgnorePropertyNames { get; set; }
        string Key { get; set; }
        void SetKeyByProperties();
        void SetPropertiesByKey();
    }

    public abstract class BaseCombineKey<T> : ICombineKey<T> where T : ICombineKey<T>
    {
        protected BaseCombineKey()
        {
            IgnorePropertyNames = new List<string>();
            IgnorePropertyNames.Add(nameof(Key));
            IgnorePropertyNames.Add(nameof(IgnorePropertyNames));
        }

        public IList<string> IgnorePropertyNames { get; set; }
        public string Key { get; set; }

        public virtual void SetKeyByProperties()
        {
            var ignoreNames = this.IgnorePropertyNames ?? new List<string>();
            if (!ignoreNames.Contains("Key"))
            {
                ignoreNames.Add("Key");
            }

            var iniString = this.AsIniString(ignoreNames);
            this.Key = iniString;
        }

        public virtual void SetPropertiesByKey()
        {
            var key = this.Key;
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            var items = key.AsIniDic();
            MyModelHelper.SetPropertiesWithDictionary(items, this);
        }
    }

    //public class DemoCombineModel : BaseCombineKey<DemoCombineModel>
    //{
    //    public DemoCombineModel()
    //    {
    //        this.IgnorePropertyNames.Add("Key");
    //        this.IgnorePropertyNames.Add("BlahId");
    //    }

    //    public string FooId { get; set; }
    //    public string BarId { get; set; }
    //    public string BlahId { get; set; }
    //}
}
