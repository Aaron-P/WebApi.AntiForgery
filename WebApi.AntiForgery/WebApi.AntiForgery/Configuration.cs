using System;
using System.Configuration;

namespace WebApi.AntiForgery
{
    internal static class Configuration
    {
        internal static int HstsMaxAge
        {
            get
            {
                return GetConfig().HstsMaxAge;
            }
        }

        internal static bool HttpsFiller
        {
            get
            {
                return GetConfig().HttpsFiller;
            }
        }

        internal static string Id
        {
            get
            {
                return GetConfig().Id;
            }
        }

        internal static bool SecureCookie
        {
            get
            {
                return GetConfig().SecureCookie;
            }
        }

        internal static string Title
        {
            get
            {
                return GetConfig().Title;
            }
        }

        private static ConfigSection GetConfig()
        {
            return (ConfigSection)ConfigurationManager.GetSection(ConfigSection.SectionName) ?? new ConfigSection();
        }
    }

    internal class ConfigSection : ConfigurationSection
    {
        internal const string SectionName = "webApi.antiForgery";

        private const string _HstsMaxAge = "hstsMaxAge";
        private const string _HttpsFiller = "httpsFiller";
        private const string _Id = "id";
        private const string _IdDefault = "VerificationToken";
        private const string _SecureCookie = "secureCookie";
        private const string _Title = "title";
        private const string _TitleDefault = "&#128273;";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ConfigurationProperty(_HstsMaxAge, IsKey = false, IsRequired = false, DefaultValue = 0)]
        [IntegerValidator]
        internal int HstsMaxAge
        {
            get
            {
                return (int)base[_HstsMaxAge];
            }
            set
            {
                base[_HstsMaxAge] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ConfigurationProperty(_HttpsFiller, IsKey = false, IsRequired = false, DefaultValue = false)]
        internal bool HttpsFiller
        {
            get
            {
                return (bool)base[_HttpsFiller];
            }
            set
            {
                base[_HttpsFiller] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ConfigurationProperty(_Id, IsKey = false, IsRequired = false, DefaultValue = _IdDefault)]
        [RegexStringValidator(@"^[a-zA-Z][\w:.-]*$")]
        internal string Id
        {
            get
            {
                var value = (string)base[_Id];
                return String.IsNullOrWhiteSpace(value) ? _IdDefault : value;
            }
            set
            {
                base[_Id] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ConfigurationProperty(_SecureCookie, IsKey = false, IsRequired = false, DefaultValue = false)]
        internal bool SecureCookie
        {
            get
            {
                return (bool)base[_SecureCookie];
            }
            set
            {
                base[_SecureCookie] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ConfigurationProperty(_Title, IsKey = false, IsRequired = false, DefaultValue = _TitleDefault)]
        [RegexStringValidator(@"^[^<>]+$")]
        internal string Title
        {
            get
            {
                var value = (string)base[_Title];
                return String.IsNullOrWhiteSpace(value) ? _TitleDefault : value;
            }
            set
            {
                base[_Title] = value;
            }
        }
    }
}