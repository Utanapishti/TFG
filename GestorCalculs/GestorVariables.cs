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

        public uint GetUltimTimestamp(string nomVariable)
        {
            if (!variables.TryGetValue(nomVariable, out var variable)) 
            {
                return 0;
            }
            else
            {
                return variable.GetUltimTimestamp();
            }
        }

        internal IEnumerable<string> GetNomsVariables()
        {
            return variables.Keys;
        }

        internal Dada AfegirDada(string nomVariable, uint ts, double valor)
        {
            var variable=GetVariable(nomVariable);
            return variable.AfegirDada(ts,valor);            
        }

        internal Dada? GetUltimaDada(string nomVariable)
        {
            if (!variables.TryGetValue(nomVariable, out var variable))
            {
                return null;
            }
            else
            {
                return variable.GetUltimaDada();
            }
        }

        private Variable GetVariable(string nomVariable)
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
