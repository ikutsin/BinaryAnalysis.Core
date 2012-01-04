using BinaryAnalysis.Data.Settings;

namespace BA.Examples.ScriptingHelper.Models
{
    public class ServiceSettingsEntryBoxMapModel
    {
        private readonly SettingsEntryBoxMap _model;

        public ServiceSettingsEntryBoxMapModel(SettingsEntryBoxMap model)
        {
            _model = model;
        }

        public string SimpleValue
        {
            get { return Model.ToString(); }
        }

        public string Name
        {
            get { return Model.Name; }
        }

        public SettingsEntryBoxMap Model
        {
            get { return _model; }
        }
    }
}
