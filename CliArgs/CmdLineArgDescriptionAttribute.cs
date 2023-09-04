using System;
using System.Collections.Generic;
using System.Text;

namespace Matthias77.CliArgs
{
    /// <summary>
    /// Adding a description to the field shown in the help.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class CmdLineArgDescriptionAttribute : Attribute
    {
        public string Description;

        public CmdLineArgDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
