namespace BinaryAnalysis.ScriptingHelper.Development
{
    public class InteractiveBase
    {
        public static object Result { get; set; }

        static InteractiveBase()
        {
            Aaa = "Hello!";
        }

        public static string Aaa { get; set; }
    }
}