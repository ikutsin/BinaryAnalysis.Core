using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;

namespace BinaryAnalysis.Terminal.Commanding
{
    public abstract class ShellCommandSet
    {
        public TextWriter Writer { get; set; }
        public IComponentContext Context { get; set; }
    }
}
