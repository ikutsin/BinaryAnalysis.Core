using System;
using System.Collections.Generic;
using System.Linq;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.Scheduler.Scheduler.Data;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ConsoleClient.ShellCommands
{
    public class QueryCommands : ShellCommandSet
    {
        public static List<Tuple<string, Type>> KnownTypes = new List<Tuple<string, Type>>  {
            new Tuple<string, Type>("HttpProxy", typeof(HttpProxyBoxMap)),
            new Tuple<string, Type>("ScheduleBoxMap", typeof(ScheduleBoxMap))
        };

        [CommandDescription("Query top 20 from box [entity-name]")]
        public string[] Top20(string input)
        {
            if (input == null)
            {
                return KnownTypes.Select(x=>x.Item1).ToArray();
            }

            throw new NotImplementedException();

            //Type boxedType = typeof(BoxQuery<>)
            //    .MakeGenericType(KnownTypes.First(x => x.Item1 == input).Item2);

            //var box = Context.Resolve(boxedType);
            //MethodInfo takeMethod = typeof(Queryable).GetMethod("Take")
            //    .MakeGenericMethod(KnownTypes.First(x => x.Item1 == input).Item2);

            //MethodInfo toBoxedMethod = typeof(BoxedExtensions).GetMethod("ToBoxed")
            //    .MakeGenericMethod(KnownTypes.First(x => x.Item1 == input).Item2);

            //var result = ((IEnumerable<object>)takeMethod.Invoke
            //(null, new object[] { box, 20 })).ToList();

            //var boxed = (IBoxed)toBoxedMethod.Invoke(null, new object[] { result });

            //var presenter = new DumperBoxPresentation();
            //presenter.MappedList = boxed.MappedList;
            //Writer.WriteLine(presenter.AsString());

            return null;
        }
    }
}
