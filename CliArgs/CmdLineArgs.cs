using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Matthias77.CliArgs
{
    /// <summary>
    /// CmdLineArgs contains the Parser and Help print.
    /// </summary>
    public class CmdLineArgs
    {
        private static Regex intListRegEx = new Regex(@"((?<le>\d+)-(?<ue>\d+))|(?<sn>\d+)");
        /// <summary>
        /// Parses the commandline and fills in field and property values
        /// </summary>
        /// <param name="target">The annotated object to fill with values</param>
        /// <param name="args">Command line arguments</param>
        /// <param name="debug">If true, all assignments are logged.</param>
        /// <returns></returns>
        public static List<string> Parse(object target, string[] args, bool debug = false)
        {
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "-h":
                    case "-?":
                    case "/h":
                    case "/?":
                        PrintHelp(target);
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
            var argsList = args.ToList();
            var targetType = target.GetType();
            foreach (System.Reflection.FieldInfo field in targetType.GetFields())
            {
                if (debug)
                {
                    Console.WriteLine($"inpecting {field.Name}");
                }
                foreach (var attr in field.GetCustomAttributes<CmdLineArgOptionAttribute>(true))
                {
                    if (debug)
                    {
                        Console.WriteLine($"specified option {attr.Option} as '{attr.HelpText}'");
                    }
                    int idx;

                    while ((idx = argsList.IndexOf(attr.Option)) >= 0)
                    {
                        argsList.RemoveAt(idx);
                        if (field.FieldType == typeof(bool))
                        {
                            field.SetValue(target, true);
                            if (debug)
                            {
                                Console.WriteLine($"setting flag {field.Name} due to '{attr.Option}'");
                            }
                            continue;
                        }
                        if (attr.Increase)
                        {
                            object obj = field.GetValue(target);
                            if (obj is int intVal)
                            {
                                intVal += 1;
                                field.SetValue(target, intVal);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' increased {field.Name} to {intVal}");
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"Increase can only be specified on integer values: {field.Name}[{attr.Option}]");
                                Environment.Exit(-1);
                            }
                            continue;
                        }
                        if (idx >= argsList.Count)
                        {
                            Console.Error.WriteLine($"missing argument for {attr.Option}");
                            Environment.Exit(-1);
                        }
                        if (debug)
                        {
                            Console.WriteLine($"dealing with {field.Name} <- {argsList[idx]}");
                        }
                        string value = argsList[idx];
                        argsList.RemoveAt(idx);
                        if (field.FieldType == typeof(string))
                        {
                            field.SetValue(target, value);
                            if (debug)
                            {
                                Console.WriteLine($"'{attr.Option}' -> {field.Name}:={value}");
                            }
                            continue;
                        }
                        else if (field.FieldType == typeof(int))
                        {
                            try
                            {
                                var intVal = int.Parse(value);
                                field.SetValue(target, intVal);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' -> {field.Name}:={intVal}");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e.ToString());
                                Environment.Exit(-1);
                            }
                        }
                        else if (field.FieldType == typeof(List<string>))
                        {
                            var obj = field.GetValue(target);
                            if (obj is List<string> listVal)
                            {
                                listVal.Add(value);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' -> {field.Name}+='{value}'");
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"Runtime error: is {field.Name} initialized?");
                                Environment.Exit(-1);
                            }
                            continue;
                        }
                        else if (field.FieldType == typeof(List<int>))
                        {
                            var obj=field.GetValue(target);
                            if(obj is List<int> listVal)
                            {
                                var matches= intListRegEx.Matches(value);
                                if(matches.Count > 0)
                                {
                                    foreach(Match match in matches)
                                    {
                                        Group sng =match.Groups["sn"];
                                        if (sng.Captures.Count > 0)
                                        {
                                            int val = int.Parse(sng.Value);
                                            listVal.Add(val);
                                            if (debug)
                                            {
                                                Console.WriteLine($"'{attr.Option}' -> {field.Name}+='{sng.Value}'");
                                            }
                                            continue;
                                        }
                                        Group le = match.Groups["le"];
                                        if(le.Captures.Count > 0)
                                        {
                                            int leVal=int.Parse(le.Value);
                                            Group ue = match.Groups["ue"];
                                            if(ue.Captures.Count > 0)
                                            {
                                                int ueVal=int.Parse(ue.Value);
                                                if(leVal< ueVal)
                                                {
                                                    for(int i=leVal;i<=ueVal;++i)
                                                        listVal.Add(i);
                                                    if (debug)
                                                    {
                                                        Console.WriteLine($"'{attr.Option}' -> {field.Name}+='{le.Value}..{ue.Value}'");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.Error.WriteLine($"order required {le.Value}<{ue.Value}");
                                                }
                                            }
                                            else
                                            {
                                                Console.Error.WriteLine("missing upper end");
                                            }
                                        }
                                        else
                                        {
                                            Console.Error.WriteLine("missing lower end");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine($"{value} did not match <number> nor <number>-<number>");
                                }
                            }else
                            {
                                Console.Error.WriteLine($"Runtime error: is {field.Name} initialized?");
                                Environment.Exit(-1);
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine($"{field.Name} has {field.FieldType.Name} which is not yet implemented");
                            Environment.Exit(-1);
                        }
                    }
                }
            }
            foreach (System.Reflection.PropertyInfo prop in targetType.GetProperties())
            {
                if (debug)
                {
                    Console.WriteLine($"inpecting {prop.Name}");
                }
                foreach (var attr in prop.GetCustomAttributes<CmdLineArgOptionAttribute>(true))
                {
                    if (debug)
                    {
                        Console.WriteLine($"specified option {attr.Option} as '{attr.HelpText}'");
                    }
                    int idx;

                    while ((idx = argsList.IndexOf(attr.Option)) >= 0)
                    {
                        argsList.RemoveAt(idx);
                        if (prop.PropertyType == typeof(bool))
                        {
                            prop.SetValue(target, true);
                            if (debug)
                            {
                                Console.WriteLine($"setting flag {prop.Name} due to '{attr.Option}'");
                            }
                            continue;
                        }
                        if (attr.Increase)
                        {
                            object obj = prop.GetValue(target);
                            if (obj is int intVal)
                            {
                                intVal += 1;
                                prop.SetValue(target, intVal);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' increased {prop.Name} to {intVal}");
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"Increase can only be specified on integer values: {prop.Name}[{attr.Option}]");
                                Environment.Exit(-1);
                            }
                            continue;
                        }
                        if (idx >= argsList.Count)
                        {
                            Console.Error.WriteLine($"missing argument for {attr.Option}");
                            Environment.Exit(-1);
                        }
                        Console.WriteLine($"dealing with {prop.Name} <- {argsList[idx]}");
                        string value = argsList[idx];
                        argsList.RemoveAt(idx);
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(target, value);
                            if (debug)
                            {
                                Console.WriteLine($"'{attr.Option}' -> {prop.Name}:={value}");
                            }
                            continue;
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            try
                            {
                                var intVal = int.Parse(value);
                                prop.SetValue(target, intVal);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' -> {prop.Name}:={intVal}");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e.ToString());
                                Environment.Exit(-1);
                            }
                        }
                        else if (prop.PropertyType == typeof(List<string>))
                        {
                            var obj = prop.GetValue(target);
                            if (obj is List<string> listVal)
                            {
                                listVal.Add(value);
                                prop.SetValue(target, listVal);
                                if (debug)
                                {
                                    Console.WriteLine($"'{attr.Option}' -> {prop.Name}+='{value}'");
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"Runtime error: is {prop.Name} initialized?");
                                Environment.Exit(-1);
                            }
                            continue;
                        }
                        else
                        {
                            Console.Error.WriteLine($"{prop.Name} has {prop.PropertyType.Name} which is not yet implemented");
                            Environment.Exit(-1);
                        }
                    }
                }

            }
            return argsList;
        }

        /// <summary>
        /// Prints the available options
        /// </summary>
        /// <param name="target">the object with the annotated fields/properties</param>
        public static void PrintHelp(object target)
        {
            var targetType = target.GetType();
            foreach (CmdLineArgDescriptionAttribute desc in targetType.GetCustomAttributes<CmdLineArgDescriptionAttribute>())
            {
                Console.WriteLine($"Possible commandline arguments for {desc.Description}");
            }
            foreach (var field in targetType.GetFields())
            {
                foreach (var fdesc in field.GetCustomAttributes<CmdLineArgDescriptionAttribute>())
                {
                    Console.WriteLine($" {fdesc.Description}:");
                }
                foreach (var fo in field.GetCustomAttributes<CmdLineArgOptionAttribute>(true))
                {
                    ShowOption(field.FieldType, fo);
                }
            }
            foreach (var prop in targetType.GetProperties())
            {
                foreach (var pdesc in prop.GetCustomAttributes<CmdLineArgDescriptionAttribute>())
                {
                    Console.WriteLine($" {pdesc.Description}:");
                }
                foreach (var po in prop.GetCustomAttributes<CmdLineArgOptionAttribute>(true))
                {
                    ShowOption(prop.PropertyType, po);
                }
            }
        }


        private static void ShowOption(Type memberType, CmdLineArgOptionAttribute option)
        {
            string extra = string.Empty;
            if (memberType == typeof(bool))
            {
                extra = "(setting to true)";
            }
            else if (memberType == typeof(int))
            {
                if (option.Increase)
                    extra = "(incrementing value)";
                else
                    extra = "(assigning next argument)";
            }
            else if (memberType == typeof(string))
            {
                extra = "(assigning next argument)";
            }
            else if (memberType == typeof(List<string>))
            {
                extra = "(appending next argument)";
            }
            Console.WriteLine($"  {option.Option} - {option.HelpText} {extra}");
        }
    }
}