[<RequireQualifiedAccess>]
module Tools

open Nuke.Common.Tooling

let dotnet = ToolPathResolver.GetPathExecutable("dotnet")
let npm = ToolPathResolver.GetPathExecutable("npm")
let node = ToolPathResolver.GetPathExecutable("node")

/// Azure CLI
/// Troubleshooting: Run `where az` from cmd line to find path to az.cmd
let az = @"C:\Program Files (x86)\Microsoft SDKs\Azure\CLI2\wbin\az.cmd"
