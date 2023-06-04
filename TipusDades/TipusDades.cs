using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipusDades
{
    public record DadaGenerada(string Name,double Valor);    
    public record Dada(double valor);
    public record DatesConsulta(DateTime DataInici, DateTime DataFi);
    public record RespostaDades(IEnumerable<Dada> Dades);
}
