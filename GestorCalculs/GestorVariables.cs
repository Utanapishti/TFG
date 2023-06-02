using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorCalculs
{
    internal class GestorVariables
    {
        Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        public uint GetLastTimestamp(string nomVariable)
        {
            if (!variables.TryGetValue(nomVariable, out var variable)) 
            {
                return 0;
            }
            else
            {
                return variable.GetLastTimestamp();
            }
        }

        internal Variable GetVariable(string nomVariable)
        {
            if (!variables.TryGetValue(nomVariable, out var variable))
            {
                variable = new Variable(nomVariable);
                variables.Add(nomVariable, variable);
            }

            return variable;
        }
    }
}
