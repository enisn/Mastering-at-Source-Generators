using Microsoft.CodeAnalysis;

namespace Awesome.Generators;

[Generator]
public class ServiceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateServiceAttribute>());
    }
}
