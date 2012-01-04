using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryAnalysis.UI.Commons
{
    public class VisualMenuItem
    {
        public string Path { get; set;  }
        public dynamic Action { get; set;  }
        public int Weight { get; set;  }
    }
    public interface IVisualDependencies
    {
        Dictionary<string, List<string>> Dependencies { get; }
        List<VisualMenuItem> MenuItems { get; }
    }
}
