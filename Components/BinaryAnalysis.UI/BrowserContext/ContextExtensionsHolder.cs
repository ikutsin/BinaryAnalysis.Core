using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryAnalysis.UI.Commons;

namespace BinaryAnalysis.UI.BrowserContext
{
    [ComVisible(true)]
    public class ContextExtensionsHolder
    {
        IEnumerable<Lazy<IBrowserContextExtension, IBrowserContextExtensionMetadata>> _exts { get; set; }

        public ContextExtensionsHolder(IEnumerable<Lazy<IBrowserContextExtension, IBrowserContextExtensionMetadata>> exts)
        {
            _exts = exts;
        }

        /// <summary>
        /// window.external('name')
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IBrowserContextExtension this[string name]
        {
            get
            {
                var extension = _exts.First(x => x.Metadata.Name == name).Value;
                return extension;
            }
        }
    }
}
