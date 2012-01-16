using System;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryAnalysis.Browsing.Extensions;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.State;
using BinaryAnalysis.Extensions.Browsing;
using BinaryAnalysis.UI.BrowserContext;
using BinaryAnalysis.UI.Commons.Data;
using Newtonsoft.Json;

namespace BinaryAnalysis.Visuals.Annotations.BrowserContext
{
    [ComVisible(true)]
    public class StateContextExtension : IBrowserContextExtension
    {
        private readonly StateService _stateService;
        private readonly StateRepository _stateRepo;
        private readonly JqGridCustomCriteria<StateBoxMap, StateEntity> _jqgridCriteria;

        public StateContextExtension(StateService stateService,
            StateRepository stateRepo,
            JqGridCustomCriteria<StateBoxMap, StateEntity> jqgridCriteria)
        {
            _stateRepo = stateRepo;
            _jqgridCriteria = jqgridCriteria;
            _stateService = stateService;
        }
        //public bool deleteState()
        public string loadStateResponse(int id)
        {
            var state = _stateRepo.Load(id);
            if (state == null) return null;
            var stateBrowsingResponse = state.GetValue<StateBrowsingResponse>();
            if (stateBrowsingResponse == null) return "Wrong type: "+state.ContractType;
            return stateBrowsingResponse.ResponseContent;
        }

        public string loadFixedStateResponse(int id)
        {
            var state = _stateRepo.Load(id);
            if (state == null) return null;
            var stateBrowsingResponse = state.GetValue<StateBrowsingResponse>();
            if (stateBrowsingResponse == null) return "Wrong type: " + state.ContractType;
            return HtmlExtraction.RemoveScript(stateBrowsingResponse.ContentWithFixedToAbsoluteLinks());
        }
    }
}
