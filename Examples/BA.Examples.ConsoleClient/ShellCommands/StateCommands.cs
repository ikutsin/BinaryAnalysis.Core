using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ConsoleClient.ShellCommands
{
    public class StateCommands : ShellCommandSet
    {
        public static List<string> resultCache = new List<string>();

        [CommandDescription("Query states which key contains input")]
        public void Contains(string input)
        {
            var client = Context.Resolve<IStateBrowsingService>();
            var result = client.RequestContains(input);
            foreach (var str in result)
            {
                Writer.WriteLine(str);
                resultCache.Add(str);
            }
            //Dumper.Dump(result, Writer);
        }

        [CommandDescription("Query states which key starts with input")]
        public void StartsWith(string input)
        {
            var client = Context.Resolve<IStateBrowsingService>();
            var result = client.RequestStartsWith(input);
            foreach (var str in result)
            {
                Writer.WriteLine(str);
                resultCache.Add(str);
            }
            //Dumper.Dump(result, Writer);
        }
        [CommandDescription("Show the queries to load by number")]
        public void Cache()
        {
            int cnt = 0;
            foreach (var str in resultCache)
            {
                Writer.WriteLine(cnt +": "+str);
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
            if (Int32.TryParse(input, out num) && num > 0)
            {
                input = resultCache[num];
            }

            var client = Context.Resolve<IStateBrowsingService>();
            var result = client.RequestStartsWith(input).FirstOrDefault();

            if (result == null)
            {
                Writer.WriteLine("State not found");
                return;
            }
            var response = client.LoadStateResponse(result);

            Writer.WriteLine("--===== Content of "+result+":");
            Writer.WriteLine(response);
            Writer.WriteLine("--===== End of " + result + "===>");
        }
    }
}
