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
3. This will need to reference 2.1.0 builds of Mvc to pick up desirable features we're adding to ApiDescription (https://github.com/aspnet/Mvc/pull/6911)

#### NSwag.AspNetCore

1. If we're allowed to directly reference NSwag.SwaggerGeneration.AspNetCoreMvc from the middleware, we could pivot on a switch in `NSwag.AspNetCore.SwaggerSettings` to determine 
what source (WebApiToSwaggerGenerator \ AspNetCoreToSwaggerGenerator) to use. Direct referencing is problematic because it will cause TFM \ package reference changes (it currently reference Microsoft.AspNetCore.Mvc 1.0.3). 
Also requires building NSwag.SwaggerGeneration.AspNetCoreMvc with the other packages in this repo which will have a pre-release dependency until we ship 2.1.0.

2. We could pivot on an adapter referencing an interface to determine the source of truth. e.g. 
    1) Introduce `NSwag.SwaggerGeneraion.ISwaggerDocumentGenerator.Generate(SwaggerGenerationContext)` that's implemented by AspNetCoreToSwaggerGenerator.
    2) NSwag.SwaggerGeneration.AspNetCore would have an extension to register the service. The middleware would use the service if available or fallback to WebApiToSwaggerGenerator.
    2) Use registered value if available in DI. Code would look like:
    ```C#
        app.AddNSwagAspNetCoreAdapter(); // services.AddSingleton<ISwaggerDocumentGenerator, AspNetCoreToSwaggerGenerator>();
        ...
        app.UseSwaggerUI();
    ```
    
#### Tools
Getting to ApiDescription requires running the user's application. 
`nswag aspnetcore2swagger /assembly:{assembly.dll} /output:{output.json} [/depsFile:{assembly.deps.json}] /tfm:[desktop|coreclr] [/bitness:[32|64]] `

* nswag will run a bootstrapper in the context of the user'code. Essentially this - https://github.com/aspnet/MvcPrecompilation/blob/dev/src/Microsoft.AspNetCore.Mvc.Razor.ViewCompilation/build/netstandard2.0/Microsoft.AspNetCore.Mvc.Razor.ViewCompilation.targets#L52-L59
* Bootstrapper discovers and executes a contract type in NSwag.SwaggerGeneration.AspNetCore (maybe re-use ISwaggerDocumentGenerator)
* NPM module essentially launches the CLI tools, so we get this for free.
* Need to determine if this is feasible from NSwagStudio.


#### NSwag.MSBuild.AspNetCore
MSBuild tasks that allow build time generation of swagger documents. Could also be used to do client generation based on some property \ switch.

Could ship the same bootstrappers that ship in the CLI and launch those directly. Alternatively, execs `nswag aspnetcore2swagger` using variables that NSwag.MSBuild sets up.


## Client - Swagger in - Generated code out
TODO
