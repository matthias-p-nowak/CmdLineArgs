using Matthias77.CliArgs;
namespace CliArgs
{
    [CmdLineArgDescription("okok")]
    internal class MainClass
    {
        [CmdLineArgOption("-dest", "destination for this example")]
        [CmdLineArgDescription("Destination for all")]
        public string Destination = string.Empty;
        [CmdLineArgDescription("Overall verbosity")]
        [CmdLineArgOption("-v", "increase verbosity", Increase = true)]
        [CmdLineArgOption("-vv","fixed verbosity")]
        public int Verbosity = 0;
        [CmdLineArgDescription("Truth telling")]
        [CmdLineArgOption("-t", "telling the truth")]
        public bool TheTruth = false;
        [CmdLineArgDescription("All the peers")]
        [CmdLineArgOption("-p", "all the pees")]
        public List<string> Peas = new List<string>();

        public MainClass()
        {

        }

        internal void Run(object remainingArgs)
        {
            Console.WriteLine();
            CmdLineArgs.PrintHelp(this);
        }
    }
}