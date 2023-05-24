using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipusDades
{
    public record DadaGenerada(double Valor, DateTime DataGeneracio);
    public record DadaTractada(string Valor, DateTime DataGeneracio);
    public record DatesConsulta(DateTime DataInici, DateTime DataFi);
    public record RespostaDades(IEnumerable<DadaTractada> Dades);
}
