using CliArgs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Matthias77.CliArgs
{
    public class CmdLineArgs
    {
        public static List<string> Parse(object target, string[] args, bool debug)
        {
            var argsList = args.ToList();
            var targetType = target.GetType();
            foreach (System.Reflection.FieldInfo field in targetType.GetFields())
            {
                if (debug)
                {
                    Console.WriteLine($"inpecting {field.Name}");
                }
                var attributes = field.GetCustomAttributes(typeof(CmdLineArgOptionAttribute), true);
                foreach (CmdLineArgOptionAttribute attr in attributes)
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

            }
            return argsList;
        }
    }
}