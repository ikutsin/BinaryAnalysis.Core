using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BA.Examples.ScriptingHelper.Logic;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class FiddlerHistoryOutputVm : AbstractVm
    {
        ObservableCollection<HistoryOutputStep> steps = new ObservableCollection<HistoryOutputStep>();
        public ObservableCollection<HistoryOutputStep> Steps { get { return steps; } }

        private HistoryOutputStep currStep;

        public void StartStep(string name)
        {
            if (currStep!=null && currStep.Count == 0)
            {
                currStep.Name = name;
                return;
            }
            currStep = new HistoryOutputStep(name);
            steps.Add(currStep);
        }
        public void AddSession(FiddlerSessionHolder session)
        {
            if (currStep == null) StartStep("unknown");
            currStep.Add(session);
        }
        public void Clear()
        {
            steps.Clear();
            currStep = null;
        }
    }


    public class HistoryOutputStep : ObservableCollection<FiddlerSessionHolder>
    {
        public HistoryOutputStep(string name)
        {
            this._name = name;
        }
        public HistoryOutputStep(IEnumerable<FiddlerSessionHolder> items, string name) : base(items)
        {
            this._name = name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { 
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs(_name));
            }
        }
    }
}
