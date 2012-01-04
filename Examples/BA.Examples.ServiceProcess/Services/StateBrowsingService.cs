using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Browsing.Extensions;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.State;
using BinaryAnalysis.Extensions.Browsing;

namespace BA.Examples.ServiceProcess.Services
{
    class StateBrowsingService : IStateBrowsingService
    {
        private readonly StateService _stateService;
        private readonly StateRepository _stateRepo;

        public StateBrowsingService(StateService stateService, StateRepository stateRepo)
        {
            _stateService = stateService;
            _stateRepo = stateRepo;
        }

        public List<string> RequestContains(string str)
        {
            return _stateRepo.FindKeys("get_", str).Select(TrimResponseClass).ToList();
        }
        public List<string> RequestStartsWith(string str)
        {
            var result =
                _stateRepo.FindKeys("get_" + str)
                    //.Concat(_stateRepo.FindKeys("IBrowsingResponse_post_" + str))
                    .Concat(_stateRepo.FindKeys("get_http://" + str));

            return result.Select(TrimResponseClass).ToList();
        }
        private string TrimResponseClass(string str)
        {
            var result = str;
            if (result.StartsWith("post_")) result = result.Substring("post_".Length);
            else if (result.StartsWith("get_")) result = result.Substring("get_".Length);

            return result;
        }

        public string LoadStateResponse(string href)
        {
            var state = _stateService.Get<StateBrowsingResponse>("get_" + href);
            if (state == null) return null;
            return state.ResponseContent;
        }
        
        public string LoadFixedStateResponse(string href)
        {
            var state = _stateService.Get<StateBrowsingResponse>("get_" + href);
            if (state == null) return null;
            return state.ContentWithFixedToAbsoluteLinks();
        }
    }
}
