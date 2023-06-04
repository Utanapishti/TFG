using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorCalculs
{
    internal class Variable
    {
        public string Nom { get; }
        LinkedList<Dada> dades = new LinkedList<Dada>();

        public Variable(string nom)
        {
            Nom = nom;
        }

        internal Dada AfegirDada(uint ts, double valor)
        {
            var novaDada = new Dada(valor, ts);
            dades.AddLast(novaDada);
            return novaDada;
        }

        internal uint GetUltimTimestamp()
        {
            if (dades.Count == 0) { return 0; }
            else return dades.Last().Timestamp;
        }

        internal Dada GetUltimaDada()
        {
            return dades.Last();
        }
    }

    public record Dada(double Valor, uint Timestamp);    
}
