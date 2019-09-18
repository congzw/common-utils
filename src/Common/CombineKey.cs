// ReSharper disable CheckNamespace

using System.Collections.Generic;

namespace Common
{
    public interface ICombineKey<T> where T : ICombineKey<T>
    {
        IList<string> IgnoreCombinePropertyNames { get; set; }
        string CombineKey { get; set; }
        void SetKeyByProperties();
        void SetPropertiesByKey();
    }

    public abstract class BaseCombineKey<T> : ICombineKey<T> where T : ICombineKey<T>
    {
        protected BaseCombineKey()
        {
            IgnoreCombinePropertyNames = new List<string>();
            IgnoreCombinePropertyNames.Add(nameof(CombineKey));
            IgnoreCombinePropertyNames.Add(nameof(IgnoreCombinePropertyNames));
        }

        public IList<string> IgnoreCombinePropertyNames { get; set; }
        public string CombineKey { get; set; }

        public virtual void SetKeyByProperties()
        {
            var iniString = this.AsIniString(IgnoreCombinePropertyNames);
            this.CombineKey = iniString;
        }

        public virtual void SetPropertiesByKey()
        {
            var key = this.CombineKey;
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            var items = key.AsIniDic();
            MyModelHelper.SetPropertiesWithDictionary(items, this);
        }
    }

    #region how to use

    //public class DemoCombineModel : BaseCombineKey<DemoCombineModel>
    //{
    //    public string FooId { get; set; }
    //    public string BarId { get; set; }
    //    public string BlahId { get; set; }
    //}

    //var demoCombineModel = new DemoCombineModel();
    //demoCombineModel.CombineKey = "FooId=Foo01;BarId=Bar01;BlahId=Blah01";
    //demoCombineModel.IgnorePropertyNames.Add("BlahId");
    //demoCombineModel.SetPropertiesByKey();
    //demoCombineModel.FooId.ShouldEqual("Foo01");
    //demoCombineModel.BarId.ShouldEqual("Bar01");
    //demoCombineModel.BlahId.ShouldEqual("Blah01");

    #endregion
}
