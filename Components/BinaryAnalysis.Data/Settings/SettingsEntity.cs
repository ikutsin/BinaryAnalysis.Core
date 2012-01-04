using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Data.Core.Impl;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Box;
using NHibernate.Validator.Constraints;

namespace BinaryAnalysis.Data.Settings
{
    [BoxTo(typeof(SettingsBoxMap))]
    public class SettingsEntity : Entity, IClassifiable
    {
        private object assignmentLocker = new object();

        [NotNullNotEmpty, Length(250)]
        public virtual string Name { get; set; }

        public SettingsEntity()
        {
            Entries = new List<SettingsEntryEntity>();
        }
        public SettingsEntity(string name):this()
        {
            Name = name;
        }
        public virtual SettingsEntryEntity this[string name] { get { return this.Entries.First(x => x.Name == name); } }

        public virtual void AddEntry(string key, object value)
        {
            lock (assignmentLocker)
            {
                if (Entries.Any(x => x.Name == key)) throw new InvalidOperationException(key + " is already exist");
                Entries.Add(new SettingsEntryEntity(key, value) {Settings = this});
            }
        }

        public virtual void SetEntry(string key, object value)
        {
            lock (assignmentLocker)
            {
                if (Entries.Any(x => x.Name == key)) this[key].SetValue(value);
                else AddEntry(key, value);
            }
        }
        public virtual SettingsEntryEntity GetEntry(string key)
        {
            return this.Entries.FirstOrDefault(x => x.Name == key);
        }

        /// <summary>
        /// Only readonly!!
        /// </summary>
        public virtual IList<SettingsEntryEntity> Entries { get; set; }
        public virtual string ObjectName
        {
            get { return "Settings"; }
        }

        public override string ToString()
        {
            return ObjectName + " " + Id;
        }
    }
}
