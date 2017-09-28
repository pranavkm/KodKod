using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace KodKod.Analyzer
{
    public abstract class KodKodAnalyzerAnalyzerBase : DiagnosticAnalyzer
    {
        public KodKodAnalyzerAnalyzerBase(DiagnosticDescriptor diagnostic)
        {
            SupportedDiagnostics = ImmutableArray.Create(diagnostic);
        }

        protected DiagnosticDescriptor SupportedDiagnostic => SupportedDiagnostics[0];

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var kodKodContext = new KodKodContext(compilationContext);

                // Only do work if ApiControllerAttribute is defined.
                if (kodKodContext.ApiControllerAttribute == null)
                {
                    return;
                }

                InitializeWorker(kodKodContext);
            });
        }

        protected abstract void InitializeWorker(KodKodContext kodKodContext);
    }

    public class KodKodContext
    {
        public KodKodContext(CompilationStartAnalysisContext context)
        {
            Context = context;
            ApiControllerAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ApiControllerAttribute");
        }

        public CompilationStartAnalysisContext Context { get; }

        public INamedTypeSymbol ApiControllerAttribute { get; }

        private INamedTypeSymbol _routeAttribute;
        public INamedTypeSymbol RouteAttribute => GetType("Microsoft.AspNetCore.Mvc.Routing.IRouteTemplateProvider", ref _routeAttribute);

        private INamedTypeSymbol _actionResultOfT;
        public INamedTypeSymbol ActionResultOfT => GetType(TypeNames.ActionResultOfT, ref _actionResultOfT);

        private INamedTypeSymbol _systemThreadingTask;
        public INamedTypeSymbol SystemThreadingTask => GetType("System.Threading.Tasks.Task", ref _systemThreadingTask);

        private INamedTypeSymbol _objectResult;
        public INamedTypeSymbol ObjectResult => GetType("Microsoft.AspNetCore.Mvc.ObjectResult", ref _objectResult);

        private INamedTypeSymbol _iActionResult;
        public INamedTypeSymbol IActionResult => GetType("Microsoft.AspNetCore.Mvc.IActionResult", ref _iActionResult);

        public INamedTypeSymbol _modelState;
        public INamedTypeSymbol ModelStateDictionary => GetType("Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary", ref _modelState);

        public bool IsKodKod(INamedTypeSymbol type) => type.GetAttributes().Any(a => a.AttributeClass == ApiControllerAttribute);

        public bool IsKodKod(IMethodSymbol method)
        {
            return 
                IsKodKod(method.ContainingType) &&
                method.DeclaredAccessibility == Accessibility.Public &&
                !method.IsAbstract &&
                !method.IsStatic;
        }

        private INamedTypeSymbol GetType(string name, ref INamedTypeSymbol cache) =>
            cache = cache ?? Context.Compilation.GetTypeByMetadataName(name);
    }
}
