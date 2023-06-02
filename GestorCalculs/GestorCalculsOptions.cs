using Microsoft.Extensions.Options;

namespace Scripting
{
    public class ScriptOptions
    {    
        public string Name { get; set; }
        public string Code { get; set; }        
    }

    public class CalculatedValuesOptions
    {
        public string ScriptName { get; set; }
        public string ReturnValue { get; set; }
        public IEnumerable<ParameterOptions> AssociatedParameters { get; set; }
    }

    public class ParameterOptions
    {
        public string Name { get; set; }
        public string AssociatedValue { get; set; }
    }

    public class GestorCalculsOptions
    {
        public IEnumerable<ScriptOptions> Scripts { get; set; }

        public IEnumerable<CalculatedValuesOptions> CalculatedValues { get; set; }
    }
}