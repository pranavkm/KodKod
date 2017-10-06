## Server: ApiDescription in - Swagger out 

### What exists today:

#### NSwag.SwaggerGeneration.WebApi - https://www.nuget.org/packages/NSwag.SwaggerGeneration.WebApi/
Uses reflection to produce a SwaggerDocument from an assembly referencing Asp.Net Core Mvc. Understands some Mvc attributes (such as Http####, From#### attributes), but does this
without referencing Mvc \ interacting with ApiDescription.

#### NSwag.Annotations - https://www.nuget.org/packages/NSwag.Annotations/
Attributes to provide swagger gen information. Has some overlap with ApiDescription attributes.

#### NSwag.AspNetCore - https://www.nuget.org/packages/NSwag.AspNetCore/
Adds middleware to your application that does swagger gen on the fly and adds an end point to server the Swagger document.

#### Tools
1. NSwagStudio- http://rsuter.com/Projects/NSwagStudio/installer.php
2. NSwag NPM module - https://www.npmjs.com/package/nswag0=}KILO;
3. NSwag CLI (Desktop \ CoreCLR binaries)
  
Tools to produce swagger document (json). Invokes NSwag.SwaggerGeneration.WebApi.

--------------------------------------------------------------------------------------------------------------------------

### What we need:

#### NSwag.SwaggerGeneration.AspNetCoreMvc - Adapter library for ApiDescription -> SwaggerDocument.

1. Something equivalent to this - https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerGen/Generator/SwaggerGenerator.cs
2. Prototype code - https://github.com/pranavkm/KodKod/blob/617d99f49ffc66f02adb6247f9042118a9134fa3/tools/KodKod.Tool/NSwaggerizer.cs
3. We'll reference Microsoft.AspNetCore.Mvc 1.0.3 (the version NSwag.AspNetCore references) and use reflection to look up any new properties to ApiDescription that were introduced in later versions of Mvc.

#### NSwag.AspNetCore

1. NSwag.AspNetCore will have a project dependency on NSwag.SwaggerGeneration.AspNetCoreMvc. We'll use a switch on `NSwag.AspNetCore.SwaggerSettings` to determine what source (WebApiToSwaggerGenerator \ AspNetCoreToSwaggerGenerator) to use:
Example:
```C#
app.UseSwaggerUI(new SwaggerUISettings { UseApiExplorerForSwaggerGeneration = true });
```
    
#### Tools
Getting to ApiDescription requires running the user's application. 
`nswag aspnetcore2swagger /assembly:{assembly.dll} /output:{output.json} [/depsFile:{assembly.deps.json}] /tfm:[desktop|coreclr] [/bitness:[32|64]] `

We'll follow a similar pattern to what we do in MvcPrecompilation. `aspnetcore2swagger` would launch a process in the user's context. 
* For dotnet core, this  means `dotnet exec ExecBinary --depsfile &quot;$(assembly.deps.json)&quot; /output:{output.json} /input:{assembly.dll}`. 
* For desktop, this would involve copying ExecBinary.{bitness}.exe to the bin directory and running it.

ExecBinary will 
1. run some portion of the startup code (https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/IDesignTimeMvcBuilderConfiguration.cs), 
2. use the `IApiDescriptionGroupCollectionProvider` from DI to run NSWag.SwaggerGeneration.AspNetCore. 
3. Write the generated output to {output.json}

* NPM module essentially launches the CLI tools, so we get this for free.
* Need to determine if this is feasible from NSwagStudio.


#### NSwag.MSBuild.AspNetCore
MSBuild tasks that allow build time generation of swagger documents. Could also be used to do client generation based on some property \ switch.
The package will add a target to the project that gathers project metadata (bitness, output path, deps file location, tfm etc) and launch `nswag aspnetcore2swagger` \ `dotnet exec ExecBinary ..`


## Client - Swagger in - Generated code out
TODO
