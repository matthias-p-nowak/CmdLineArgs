using System;

namespace Matthias77.CliArgs
{
    /// <summary>
    /// Denotes a field/property that can be filled with a value from the command line
    /// </summary>
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple =true)]
    public class CmdLineArgOptionAttribute: Attribute
    {
        /// <summary>
        /// short option value like "-p"
        /// </summary>
        public string Option;
        /// <summary>
        /// Help text displayed if something goes wrong or if requested
        /// </summary>
        public string HelpText;
        /// <summary>
        /// when an int-value option is increased each time it is mentioned
        /// </summary>
        public bool Increase;

        /// <summary>
        /// Basic 
        /// </summary>
        /// <param name="option">the exact phrase on the command line</param>
        /// <param name="helpText">Help text to be displayed for this option</param>
        public CmdLineArgOptionAttribute(string option, string helpText)
        {
            Option = option;
            HelpText = helpText;
        }
    }
}
