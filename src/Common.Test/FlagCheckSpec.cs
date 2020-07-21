using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common
{

    [TestClass]
    public class FlagCheckSpec
    {
        [TestMethod]
        public void ShouldThrow_None_Should_Throw()
        {
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.None, true).ShouldTrue();
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.None, false).ShouldTrue();
        }

        [TestMethod]
        public void ShouldThrow_All_Should_NotThrow()
        {
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.All, true).ShouldFalse();
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.All, false).ShouldFalse();
        }
        
        [TestMethod]
        public void ShouldThrow_Pro_Should_ThrowBy()
        {
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.Dev, true).ShouldFalse();
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.Dev, false).ShouldTrue();
            
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.Pro, true).ShouldTrue();
            OptionsCheck.ShouldJsonThrow(MyExceptionHandleJsonOptions.Pro, false).ShouldFalse();
        }
    }


    public class OptionsCheck
    {
        public static bool ShouldJsonThrow(MyExceptionHandleJsonOptions options, bool isDev)
        {
            if (!options.HasFlag(MyExceptionHandleJsonOptions.Dev) && isDev)
            {
                return true;
            }
            if (!options.HasFlag(MyExceptionHandleJsonOptions.Pro) && !isDev)
            {
                return true;
            }
            return false;
        }
        public static bool ShouldHtmlThrow(MyExceptionHandleHtmlOptions options, bool isDev)
        {
            if (!options.HasFlag(MyExceptionHandleHtmlOptions.Dev) && isDev)
            {
                return true;
            }
            if (!options.HasFlag(MyExceptionHandleHtmlOptions.Pro) && !isDev)
            {
                return true;
            }
            return false;
        }
    }

    [Flags]
    public enum MyExceptionHandleJsonOptions
    {
        None = 0,
        Dev = 1 << 0,
        Pro = 1 << 1,
        Default = Dev | Pro,
        All = Dev | Pro
    }
    [Flags]
    public enum MyExceptionHandleHtmlOptions
    {
        None = 0,
        Dev = 1 << 0,
        Pro = 1 << 1,
        Default = Pro,
        All = Dev | Pro
    }
}
