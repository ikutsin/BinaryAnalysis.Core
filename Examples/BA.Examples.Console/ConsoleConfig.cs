using System.Configuration;

namespace BA.Examples.Console
{
    public class ConsoleConfigSection : ConfigurationSection
    {
        public static ConsoleConfigSection GetConfig()
        {
            return (ConsoleConfigSection)ConfigurationManager.
               GetSection("console") ??
               new ConsoleConfigSection();

        }
        [ConfigurationProperty("manage")]
        public NameValueConfigElementCollection Clean
        {
            get
            {
                return (NameValueConfigElementCollection)this["manage"] ??
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
