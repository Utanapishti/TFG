using Microsoft.Extensions.Options;

namespace Scripting
{
    public class FunctionOptions
    {
        public string ReturnVariable { get; set; } = String.Empty;
        public string Function { get; set; } = String.Empty;
        public IEnumerable<ParameterOptions> Parameters { get; set; }=Array.Empty<ParameterOptions>();
    }

    public class ParameterOptions
    {
        public string Name { get; set; }= String.Empty;
        public string AssociatedVariable { get; set; } = String.Empty;
    }

    public class GestorFuncionsOptions
    {
        public IEnumerable<FunctionOptions> Functions { get; set; } = Array.Empty<FunctionOptions>();
    }
}