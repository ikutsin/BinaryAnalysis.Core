using System.Linq;
using System.Runtime.InteropServices;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Settings;
using BinaryAnalysis.UI.BrowserContext;
using Newtonsoft.Json;

namespace BinaryAnalysis.Visuals.Annotations.BrowserContext
{
    [ComVisible(true)]
    public class SettingsContextExtension : IBrowserContextExtension
    {
        private readonly SettingsService _settingsService;
        private readonly NHibernateBoxTransformation<SettingsBoxMap, SettingsEntity> _settingsTransformer;

        public SettingsContextExtension(SettingsService settingsService,
            NHibernateBoxTransformation<SettingsBoxMap, SettingsEntity> settingsTransformer)
        {
            _settingsService = settingsService;
            _settingsTransformer = settingsTransformer;
        }

        public string getListColNames()
        {
            return JsonConvert.SerializeObject(new[] { "Name", "Value", "Type" });
        }

        public string getListColModel()
        {
            return JsonConvert.SerializeObject(
                new[]
                    {
                        QjGridColumnFormat.Create("Name", typeof (string)),
                        QjGridColumnFormat.Create("Value", typeof (string)),
                        QjGridColumnFormat.Create("Type", typeof (string))
                    });
        }

        public string getListEntries(string typename, int id)
        {
            var settings = _settingsService.GetFor(new ClassifiableElementProxy(typename, id));

            _settingsTransformer.Entries = new[] { settings };
            var box = _settingsTransformer.ToBox().First();

            var viewmodel = box.Entries.Select(
                d => new
                         {
                             Name = d.Name,
                             Value = d.GetValue(),
                             //d.IsContract?ToJsObject(d.GetValue()):d.GetValue().ToString(),
                             Type = d.ContractType
                         }).ToList();

            var obj = new
                          {
                              total = viewmodel.Count(),
                              page = 1,
                              records = viewmodel.Count(),
                              rows = viewmodel
                          };
            return JsonConvert.SerializeObject(obj);
        }

    }
}
