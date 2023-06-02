using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorCalculs
{
    internal class Script
    {
        public string Nom { get; set; }
        public string CodiPython { get; set; }
        public IEnumerable<ParametreScript> Parametres { get; set; }                   
    }

    internal class ParametreScript
    {
        public string Nom { get; set; }
        public string VariableAssociada { get; set; }        
    }
}
