using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.Browsing.Windowless;

namespace BinaryAnalysis.Browsing.Local
{
    public class LocalFilesBrowser
    {
        public LocalFilesBrowser()
        {
        }
        public IBrowsingResponse LoadFile(string filePath)
        {
            return new LocalFileBrowsingResponse(filePath);
        }
    }
}
