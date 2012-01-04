using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Data.Box;
using System.Reflection;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
{
    public class QueryCommands : ShellCommandSet
    {
        private IEnumerable<Tuple<string, string, string>> GetMappings()
        {
            return Context.Resolve<RepositoryFinder>().Mappings
                .Select(x => new Tuple<string, string, string>
                                 (x.Item2.Name, x.Item2.AssemblyQualifiedName, x.Item1.AssemblyQualifiedName));
        }

        private static IEnumerable<Tuple<string, string, string>> mappings;
        IEnumerable<Tuple<string, string, string>> Mappings
        {
            get { return mappings ?? (mappings = GetMappings()); }
        }

        [CommandDescription("Count box [entity-name]")]
        public string[] Count(string input)
        {
            if (input == null)
            {
                return Mappings.Select(x => x.Item1).ToArray();
            }
            Type reqType = Type.GetType(Mappings.First(x => x.Item1 == input).Item3);
            Type boxType = Type.GetType(Mappings.First(x => x.Item1 == input).Item2);

            Type boxedType = typeof(BoxQuery<>).MakeGenericType(boxType);

            var boxQuery = Context.Resolve(boxedType);
            MethodInfo takeMethod = typeof(Queryable)
                .GetMethods().First(m=>m.Name=="Count")
                .MakeGenericMethod(boxType);

            var result = (int) takeMethod.Invoke(null, new object[] {boxQuery});

            Writer.WriteLine(result);

            return null;
            
        }

        [CommandDescription("Query top 20 from box [entity-name]")]
        public string[] Top20(string input)
        {
            if (input == null)
            {
                return Mappings.Select(x => x.Item1).ToArray();
            }
            Type reqType = Type.GetType(Mappings.First(x => x.Item1 == input).Item3);
            Type boxType = Type.GetType(Mappings.First(x => x.Item1 == input).Item2);

            Type boxedType = typeof(BoxQuery<>).MakeGenericType(boxType);

            var boxQuery = Context.Resolve(boxedType);
            MethodInfo takeMethod = typeof(Queryable).GetMethod("Take").MakeGenericMethod(boxType);

            var result = ((IEnumerable<object>)takeMethod.Invoke
            (null, new object[] { boxQuery, 20 })).ToList();

            var presenter = new DumperBoxPresentation();

            var box = BoxExtensions.CreateFor(boxType);
            result.ToList().ForEach(x=>box.Add(x));

            Writer.WriteLine(presenter.AsString(box));

            return null;
        }
    }
}
