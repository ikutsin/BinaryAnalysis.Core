using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace BA.Examples.ScriptingHelper.Models
{
    public class DictionaryModel : DynamicObject, INotifyPropertyChanged
    {
        public Dictionary<string, object> Dictionary { get; protected set; }

        protected DictionaryModel()
            : this(new Dictionary<string, object>())
        {
        }

        protected DictionaryModel(Dictionary<string, object> dictionary)
        {
            Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            return Dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Dictionary[binder.Name] = value;

            RaisePropertyChanged(binder.Name);

            return true;
        }

        /// <summary>
        /// Tries to return the dictionary value for the specified name. If the name is not found defaultValue is returned.
        /// </summary>
        public T GetProperty<T>(string name, T defaultValue)
        {
            object result;

            if (Dictionary.TryGetValue(name, out result) && result is T)
            {
                return (T)result;
            }

            return defaultValue;
        }

        public void SetProperty(string name, object value)
        {
            if (Dictionary.ContainsKey(name) && Dictionary[name] == value)
                return;

            Dictionary[name] = value;

            RaisePropertyChanged(name);
        }

        public static explicit operator DictionaryModel(Dictionary<string, object> dictionary)
        {
            return new DictionaryModel(dictionary);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null) return;

            var args = new PropertyChangedEventArgs(propertyName);
            handler(this, args);
        }
    }
}
