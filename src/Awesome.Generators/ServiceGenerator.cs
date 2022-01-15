using Awesome.Generators.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace Awesome.Generators;

[Generator]
public class ServiceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateServiceAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateServiceAttribute> syntaxReceiver)
        {
            return;
        }

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax);

            // Finding my GenerateServiceAttribute over it. I'm sure this attribute is placed, because my syntax receiver already checked before.
            // So, I can surely execute following query.
            var attribute = classSyntax.AttributeLists.SelectMany(sm => sm.Attributes).First(x => x.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(GenerateServiceAttribute).Name));

            // Getting constructor parameter of the attribute. It might be not presented.
            var templateParameter = attribute.ArgumentList?.Arguments.FirstOrDefault()?.GetLastToken().ValueText; // Temprorary... Attribute has only one argument for now.

            // Can't access embeded resource of main project.
            // So overridden template must be marked as Analyzer Additional File to be able to be accessed by an analyzer.
            var overridenTemplate = templateParameter != null ?
                context.AdditionalFiles.FirstOrDefault(x => x.Path.EndsWith(templateParameter))?.GetText().ToString() :
                null;

            // Generate the real source code. Pass the template parameter if there is a overriden template.
            var sourceCode = GetSourceCodeFor(symbol, overridenTemplate);

            context.AddSource(
                $"{symbol.Name}{templateParameter ?? "Controller"}.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
            Console.WriteLine(classSyntax);
        }
    }

    private string GetSourceCodeFor(INamedTypeSymbol symbol, string template = null)
    {
        // If template isn't provieded, use default one from embeded resources.
        template ??= GetEmbededResource("Awesome.Generators.Templates.Default.txt");

        // Can't use scriban at the moment, make it manually for now.
        return template
            .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", symbol.Name)
            .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
            .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name)
            ;
    }

    private string GetEmbededResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    private string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}
