using Matthias77.CliArgs;
namespace CliArgs
{
    internal class MainClass
    {
        [CmdLineArgOption("-dest", "destination for this example")]
        public string Destination = string.Empty;
        [CmdLineArgOption("-v", "increase verbosity", Increase = true)]
        [CmdLineArgOption("-vv","fixed verbosity")]
        public int Verbosity = 0;
        [CmdLineArgOption("-t", "telling the truth")]
        public bool TheTruth = false;
        [CmdLineArgOption("-p", "all the pees")]
        public List<string> Peas = new List<string>();

        public MainClass()
        {

        }

        internal void Run(object remainingArgs)
        {
            
        }
    }
}