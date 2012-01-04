using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using System.Runtime.Serialization;
using System.IO;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.Data.Settings
{
    [BoxTo(typeof(SettingsEntryBoxMap))]
    public class SettingsEntryEntity : ContractContainerEntity
    {
        public virtual SettingsEntity Settings { get; set; }
        public virtual string Name { get; set; }

        public SettingsEntryEntity()
        {
        }

        public SettingsEntryEntity(string key, string value):this(key, value as object)
        {
            
        }
        public SettingsEntryEntity(string key, object value)
        {
            this.Name = key;
            SetValue(value);
        }

        public override string ToString()
        {
            return Settings.Id + ":" + Name+"="+GetValue();
        }
    }
}
