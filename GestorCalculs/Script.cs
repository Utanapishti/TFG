using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorCalculs
{    
    internal record FuncioVariable(string NomVariable,string NomScript,IEnumerable<ParametreScript> Parametres);
    internal record ParametreScript(string Nom,string VariableAssociada);    
}
