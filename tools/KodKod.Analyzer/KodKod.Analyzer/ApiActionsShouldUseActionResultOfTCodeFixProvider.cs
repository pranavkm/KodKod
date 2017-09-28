using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace KodKod.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiActionsShouldUseActionResultOfTCodeFixProvider)), Shared]
    public class ApiActionsShouldUseActionResultOfTCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(DiagnosticDescriptors.KK1002_ApiActionsShouldReturnActionResultOf.Id);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (context.Diagnostics.Length == 1 && context.Diagnostics[0].Properties.TryGetValue("ReturnType", out var returnTypeName))
            {
                var rootNode = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var title = $"Make return type {returnTypeName}";
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title,
                        createChangedDocument: cancellationToken => CreateChangedDocumentAsync(rootNode, cancellationToken),
                        equivalenceKey: title),
                    context.Diagnostics);
            }

            async Task<Document> CreateChangedDocumentAsync(SyntaxNode rootNode, CancellationToken cancellationToken)
            {
                var returnType = (IdentifierNameSyntax)rootNode.FindNode(context.Span);
                var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken).ConfigureAwait(false);
                editor.ReplaceNode(returnType, SyntaxFactory.IdentifierName(returnTypeName));

                return editor.GetChangedDocument();
            }
        }
    }
}