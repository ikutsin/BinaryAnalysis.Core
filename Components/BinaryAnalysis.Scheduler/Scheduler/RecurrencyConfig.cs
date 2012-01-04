using System;
using System.Configuration;

namespace BinaryAnalysis.Scheduler.Scheduler
{
    public class RecurrencyConfigSection : ConfigurationSection
    {
        public static RecurrencyConfigSection GetConfig()
        {
            return (RecurrencyConfigSection)ConfigurationManager.
               GetSection("recurrencyConfig") ??
               new RecurrencyConfigSection();

        }
        [ConfigurationProperty("crontab")]
        public RecurrencyConfigElementCollection CronTab
        {
            get
            {
                return (RecurrencyConfigElementCollection)this["crontab"] ??
                   new RecurrencyConfigElementCollection();
            }
        }
    }
    public class RecurrencyConfigElementCollection : ConfigurationElementCollection
    {
        public RecurrencyConfigElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as RecurrencyConfigElement;
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
            return new RecurrencyConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecurrencyConfigElement)element).TaskName;
        }
    }

    public class RecurrencyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("cron", IsRequired = true)]
        public string Cron { get { return this["cron"] as string; } }

        [ConfigurationProperty("taskname", IsRequired = true)]
        public string TaskName { get { return this["taskname"] as string; } }

        [ConfigurationProperty("recreate", IsRequired = false)]
        public bool Recreate { get { return (this["recreate"] != null && Boolean.Parse(this["recreate"].ToString())); } }
    }
}
