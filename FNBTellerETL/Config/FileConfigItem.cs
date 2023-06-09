using FNBCoreETL.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Config
{
    public static class FileConfigItem
    {
        public class AsString
        {
            public string Key { get; private set; }
            public bool IsRequired { get; private set; }

            private string defaultValue;

            public string Value
            {
                get
                {
                    if (IsRequired)
                    {
                        return GetRequiredConfig(Key);
                    }
                    else
                    {
                        return GetOptionalConfig(Key, defaultValue);
                    }
                }
            }

            public AsString(string key, bool isRequired = true, string defaultValueIfNotRequired = null)
            {
                Key = key;
                IsRequired = isRequired;
                defaultValue = defaultValueIfNotRequired;
            }

            public override string ToString()
            {
                return Value;
            }
        }

        public class AsBool
        {
            public string Key { get; private set; }
            public bool IsRequired { get; private set; }

            private bool defaultValue;

            public bool Value
            {
                get
                {
                    if (IsRequired)
                    {
                        return GetRequiredConfigBool(Key);
                    }
                    else
                    {
                        return GetOptionalConfigBool(Key, defaultValue);
                    }
                }
            }

            public AsBool(string key, bool isRequired = true, bool defaultValueIfNotRequired = false)
            {
                Key = key;
                IsRequired = isRequired;
                defaultValue = defaultValueIfNotRequired;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public class AsInt
        {
            public string Key { get; private set; }
            public bool IsRequired { get; private set; }

            private int defaultValue;

            public int Value
            {
                get
                {
                    if (IsRequired)
                    {
                        return GetRequiredConfigInt(Key);
                    }
                    else
                    {
                        return GetOptionalConfigInt(Key, defaultValue);
                    }
                }
            }

            public AsInt(string key, bool isRequired = true, int defaultValueIfNotRequired = 0)
            {
                Key = key;
                IsRequired = isRequired;
                defaultValue = defaultValueIfNotRequired;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private static string GetRequiredConfig(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings.Get(key);
            }
            catch (Exception ex)
            {
                var myEx = new FileConfigurationException("Missing app config key: " + key ?? "null", ex);
                throw myEx;
            }
        }

        private static bool GetRequiredConfigBool(string key)
        {
            string holder = GetRequiredConfig(key);
            bool boolHolder;
            if (Boolean.TryParse(holder, out boolHolder))
                return boolHolder;
            else
            {
                var myEx = new FileConfigurationException("Invalid Type, Not Boolean for app config key: " + key + " (value = " + holder + ")");
                throw myEx;
            }
        }

        private static int GetRequiredConfigInt(string key)
        {
            string holder = GetRequiredConfig(key);
            int intHolder;
            if (Int32.TryParse(holder, out intHolder))
                return intHolder;
            else
            {
                var myEx = new FileConfigurationException("Invalid Type, Not Integer for app config key: " + key + " (value = " + holder + ")");
                throw myEx;
            }
        }

        private static string GetOptionalConfig(string key, string defaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                return ConfigurationManager.AppSettings.Get(key);
            else
                return defaultValue;
        }

        private static bool GetOptionalConfigBool(string key, bool defaultValue)
        {
            string holder = GetOptionalConfig(key, null);
            if (!String.IsNullOrEmpty(holder))
            {
                bool boolHolder;
                if (Boolean.TryParse(holder, out boolHolder))
                    return boolHolder;
                else
                {
                    var myEx = new FileConfigurationException("Invalid Type, Not Boolean for app config key: " + key + " (value = " + holder + ")");
                    throw myEx;
                }
            }
            else
                return defaultValue;
        }

        private static int GetOptionalConfigInt(string key, int defaultValue)
        {
            string holder = GetOptionalConfig(key, null);
            if (!String.IsNullOrEmpty(holder))
            {
                int intHolder;
                if (Int32.TryParse(holder, out intHolder))
                    return intHolder;
                else
                {
                    var myEx = new FileConfigurationException("Invalid Type, Not Integer for app config key: " + key + " (value = " + holder + ")");
                    throw myEx;
                }
            }
            else
                return defaultValue;
        }
    }
}
