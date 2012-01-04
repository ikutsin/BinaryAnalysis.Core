namespace BA.Examples.ScriptingHelper.Logic
{
    public class FiddlerSessionHolder
    {
        private Fiddler.Session session;
        public bool isAjaxCall { get; private set; }

        public Fiddler.Session FiddlerSession { get { return session; } }

        public FiddlerSessionHolder(Fiddler.Session session, bool isAjaxCall)
        {
            this.session = session;
            this.isAjaxCall = isAjaxCall;
        }


        public FiddlerSessionBrowsingResponse BrowsingResponse
        {
            get
            {
                return new FiddlerSessionBrowsingResponse(session);
            }
        }
    }
}
