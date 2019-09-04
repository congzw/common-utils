using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace Common
{
    public class IncludeProperties
    {
        public IncludeProperties()
        {
            SplitChar = ',';
            Properties = string.Empty;
            IgnoreCase = true;
        }

        public bool IgnoreCase { get; set; }
        public char SplitChar { get; set; }
        public string Properties { get; set; }

        public IncludeProperties SetProperty(string name, bool include)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return this;
            }

            if (Properties == null)
            {
                Properties = string.Empty;
            }

            var names = Properties.Split(SplitChar).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return include ? AddIfNotExit(names, name) : RemoveIfExit(names, name);
        }

        public IncludeProperties SetProperties(bool include, params string[] names)
        {
            foreach (var name in names)
            {
                SetProperty(name, include);
            }
            return this;
        }

        public bool HasProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(Properties))
            {
                return false;
            }
            var names = Properties.Split(SplitChar).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var tryFindIt = TryFindIt(names, name);
            return !string.IsNullOrWhiteSpace(tryFindIt);
        }

        private string TryFindIt(IList<string> names, string name)
        {
            return IgnoreCase ? names.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)) : names.FirstOrDefault(x => x == name);
        }
        private IncludeProperties AddIfNotExit(IList<string> names, string name)
        {
            var theOne = TryFindIt(names, name);
            if (string.IsNullOrWhiteSpace(theOne))
            {
                names.Add(name);
                Properties = string.Join(SplitChar.ToString(), names);
                return this;
            }
            return this;
        }
        private IncludeProperties RemoveIfExit(IList<string> names, string name)
        {
            var theOne = TryFindIt(names, name);
            if (string.IsNullOrWhiteSpace(theOne))
            {
                return this;
            }
            names.Remove(theOne);
            Properties = string.Join(SplitChar.ToString(), names);
            return this;
        }

        public static IncludeProperties Create(bool ignoreCase = true, char splitChar = ',')
        {
            var includeProperties = new IncludeProperties();
            includeProperties.IgnoreCase = ignoreCase;
            includeProperties.SplitChar = splitChar;
            return includeProperties;
        }
    }
}
