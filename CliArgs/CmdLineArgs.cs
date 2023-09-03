using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Matthias77.CliArgs
{
    public class CmdLineArgs
    {
        public static List<string> Parse(object target, string[] args, bool debug)
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
                        Console.WriteLine($"dealing with {field.Name} <- {argsList[idx]}");
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
                                field.SetValue(target, listVal);
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