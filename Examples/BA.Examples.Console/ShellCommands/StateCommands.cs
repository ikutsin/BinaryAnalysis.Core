using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Browsing.Extensions;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.State;
using BinaryAnalysis.Extensions.Browsing;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
{
    public class StateCommands : ShellCommandSet
    {
        public static List<string> resultCache = new List<string>();

        [CommandDescription("Query states which key contains input")]
        public void Contains(string input)
        {
            var stateRepo = Context.Resolve<StateRepository>();
            var result = stateRepo.FindKeys("get_", input).Select(TrimResponseClass).ToList();

            foreach (var str in result)
            {
                Writer.WriteLine(str);
                resultCache.Add(str);
            }
        }

        [CommandDescription("Query states which key starts with input")]
        public void StartsWith(string input)
        {
            var stateRepo = Context.Resolve<StateRepository>();
            var result =
                stateRepo.FindKeys("get_" + input)
                    .Concat(stateRepo.FindKeys("get_http://" + input))
                    .Select(TrimResponseClass).ToList();
            foreach (var str in result)
            {
                Writer.WriteLine(str);
                resultCache.Add(str);
            }
        }

        [CommandDescription("Show the queries to load by number")]
        public void Cache()
        {
            int cnt = 0;
            foreach (var str in resultCache)
            {
                Writer.WriteLine(cnt + ": " + str);
                cnt++;
            }
            Dumper.Dump(resultCache, Writer);
        }
        public void CacheClean()
        {
            resultCache = new List<string>();
        }

        [CommandDescription("Show state content")]
        public void Load(string input)
        {
            int num = -1;
            if (Int32.TryParse(input, out num) && num >= 0)
            {
                input = resultCache[num];
            }
            var stateService = Context.Resolve<StateService>();
            var state = stateService.Get<StateBrowsingResponse>("get_" + input);
            if (state == null)
            {
                Writer.WriteLine("State not found");
                return;
            }
            Writer.WriteLine("--===== Content of " + state + ":");
            Writer.WriteLine(state.ResponseContent);
            Writer.WriteLine("--===== End of " + state + "===>");
        }

        [CommandDescription("Show state content with fixed links")]
        public void LoadFixed(string input)
        {
            int num = -1;
            if (Int32.TryParse(input, out num) && num > 0)
            {
                input = resultCache[num];
            }
            var stateService = Context.Resolve<StateService>();
            var state = stateService.Get<StateBrowsingResponse>("get_" + input);
            if (state == null)
            {
                Writer.WriteLine("State not found");
                return;
            }
            Writer.WriteLine("--===== Fixed Content of " + state + ":");
            Writer.WriteLine(state.ContentWithFixedToAbsoluteLinks());
            Writer.WriteLine("--===== End of " + state + "===>");
        }

        private string TrimResponseClass(string str)
        {
            var result = str;

            if (result.StartsWith("post_")) result = result.Substring("post_".Length);
            else if (result.StartsWith("get_")) result = result.Substring("get_".Length);

            return result;
        }
    }
}
