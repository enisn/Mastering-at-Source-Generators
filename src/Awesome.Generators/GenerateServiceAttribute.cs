namespace Awesome.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateServiceAttribute : Attribute
{
    public GenerateServiceAttribute(string template = null)
    {
    }
}
