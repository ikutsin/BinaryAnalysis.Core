using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.ContractContainer;
using BinaryAnalysis.Data.Core.Impl;
using FluentNHibernate.Mapping;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using BinaryAnalysis.Data.Box;
using System.Reflection;

namespace BinaryAnalysis.Data.Settings
{
    public class SettingsEntityMap : EntityClassMap<SettingsEntity>
    {
        public SettingsEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            HasMany(x => x.Entries)
                .KeyColumn("Setting_Id").Inverse().Cascade.All();
        }
    }
    public class SettingsEntryEntityMap : ContractContainerClassMap<SettingsEntryEntity>
    {

        public SettingsEntryEntityMap()
        {
            Map(x => x.Name).Not.Nullable();
            References(x => x.Settings, "Setting_Id").Not.Nullable();
        }
    }
}
