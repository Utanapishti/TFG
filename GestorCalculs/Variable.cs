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
        Dada ultimaDada;
        private object lockDada=new object();

        public Variable(string nom)
        {
            Nom = nom;
        }

        internal Dada AfegirDada(uint ts, double valor)
        {
            var novaDada = new Dada(valor, ts);
            lock(lockDada)
            {                
                if (ts>ultimaDada.Timestamp)
                {
                    ultimaDada=novaDada;
                    return ultimaDada;
                }
            }
            return null;
        }

        internal uint GetUltimTimestamp()
        {
            if (ultimaDada == null)
                return 0;
            return ultimaDada.Timestamp;
        }

        internal Dada GetUltimaDada()
        {
            return ultimaDada;
        }
    }

    public record Dada(double Valor, uint Timestamp);    
}
