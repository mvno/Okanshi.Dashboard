namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Okanshi.Dashboard")>]
[<assembly: AssemblyProductAttribute("Okanshi.Dashboard")>]
[<assembly: AssemblyDescriptionAttribute("Dashboard for visualizing Okanshi metrics")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
