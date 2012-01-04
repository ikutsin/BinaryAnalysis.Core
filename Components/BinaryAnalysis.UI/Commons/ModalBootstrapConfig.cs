using System;
using System.Configuration;

namespace BinaryAnalysis.UI.Commons
{
    public class ModalBootstrapConfigSection : ConfigurationSection
    {
        public static ModalBootstrapConfigSection GetConfig()
        {
            return (ModalBootstrapConfigSection)ConfigurationManager.
               GetSection("modalBootstrap") ??
               new ModalBootstrapConfigSection();

        }
        [ConfigurationProperty("jsComponents")]
        public NameValueConfigElementCollection JsComponents
        {
            get
            {
                return (NameValueConfigElementCollection)this["jsComponents"] ??
                   new NameValueConfigElementCollection();
            }
        }
        [ConfigurationProperty("restore")]
        public NameValueConfigElementCollection Restore
        {
            get
            {
                return (NameValueConfigElementCollection)this["restore"] ??
                   new NameValueConfigElementCollection();
            }
        }
        [ConfigurationProperty("registry")]
        public NameValueConfigElementCollection Registry
        {
            get
            {
                return (NameValueConfigElementCollection)this["registry"] ??
                   new NameValueConfigElementCollection();
            }
        }
    }

    #region NameValueConfigElement
    public class NameValueConfigElementCollection : ConfigurationElementCollection
    {
        public NameValueConfigElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as NameValueConfigElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new NameValueConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NameValueConfigElement)element).Name;
        }

        [ConfigurationProperty("disabled", IsKey = true)]
        public bool Disabled { get { return (bool)this["disabled"]; } }
    }
    public class NameValueConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name { get { return this["name"] as string; } }

        [ConfigurationProperty("value", IsRequired = false)]
        public string Value { get { return this["value"] as string; } }
    } 
    #endregion

}
