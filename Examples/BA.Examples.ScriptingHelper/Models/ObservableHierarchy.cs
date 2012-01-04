using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BA.Examples.ScriptingHelper.ViewModels;

namespace BA.Examples.ScriptingHelper.Models
{
    public abstract class ObservableHierarchy<T> : AbstractVm
    {
        private T current;
        public T Current { get { return current; } set { current = value; NotifyPropertyChanged("Current"); } }

        private ObservableCollection<ObservableHierarchy<T>> children;
        public ObservableCollection<ObservableHierarchy<T>> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<ObservableHierarchy<T>>();
                    if (Current != null)
                    {
                        foreach (var item in Expand())
                        {
                            children.Add(Create(item));
                        }
                    }
                }
                return children;
            }
            set { children = value; }
        }

        public ObservableHierarchy(T current)
        {
            this.Current = current;
        }

        protected abstract IEnumerable<T> Expand();
        protected abstract ObservableHierarchy<T> Create(T item);

        public IEnumerable<ObservableHierarchy<T>> Descendants()
        {
            var result = Children.Select(x => x);
            foreach (var child in Children)
            {
                result = result.Concat(child.Descendants());
            }
            return result;

        }
    }

}
