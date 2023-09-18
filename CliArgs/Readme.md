# CliArgs - a simplified command line argument parser

## Intention

It is a minimalistic command line parser compared with `GetOpt`. 
All command line options are optional and cannot be combined. 
No checks on the configuration are performed, the developer has to avoid duplicates and other complications.

The possible options are specified as annotations on the fields of a class. 

## Limitations

It is `GetOpt` little cousin, it has no advanced features. CommandLine arguments are processed in the order they appear on fields at runtime and are subsequently removed from the returned residual command line. The order is not guaranteed. And the values are filled in th supplied class with no checks.

## How does it work?

The Parser in `Matthias77.CliArgs.CmdLineArgs.Parse()` inspects the fields in the supplied class and checks the attached attributes. It then finds occurances on the command line and dependent on the field type, it might retrieve arguments and store them.

## Actions

Field type | Condition | Action
--- | --- | ---
`Boolean` | | option sets this field to true
`Integer` | Increase=false | takes next argument and assigns the int value
`Integer` | Increase=true | increments the value
`String` | | assigns the next argument
`List<String>` | | appends the next argument to the list
`List<int>` | | arguments might be comma-separated list of single numbers or ranges	`<lower end>-<upper end>`, all numbers are added to the list
