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
        LinkedList<Dada> lectures = new LinkedList<Dada>();

        public Variable(string nom)
        {
            Nom = nom;
        }

        internal Dada AfegirDada(uint ts, double valor)
        {
            var novaDada = new Dada(valor, ts);
            lectures.AddLast(novaDada);
            return novaDada;
        }

        internal uint GetLastTimestamp()
        {
            if (lectures.Count == 0) { return 0; }
            else return lectures.Last().Timestamp;
        }
    }

    internal record Dada(double Valor, uint Timestamp);    
}
