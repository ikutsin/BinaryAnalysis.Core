using System.Collections.Specialized;
using BinaryAnalysis.Browsing.Local;

namespace BinaryAnalysis.Tests.Helpers
{
    class TestResponse : LocalFileBrowsingResponse
    {
        public TestResponse()
            : base("_data/usanov-net.txt")
        {
        }
        public override NameValueCollection Headers
        {
            get
            {
                return new NameValueCollection()
                           {
                               {"Server", "IiS6"}
                           };
            }
        }
    }
}
