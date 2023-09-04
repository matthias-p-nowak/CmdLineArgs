using Matthias77.CliArgs;

namespace CliArgs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mc = new MainClass();
            var remainingArgs = CmdLineArgs.Parse(mc, args, false);
            mc.Run(remainingArgs);
            Console.WriteLine("Hello, World!");
        }
    }
}