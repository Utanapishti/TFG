using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipusDades;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace Protocol
{
    public static class ProtocolDadaGenerada
    {
        public static byte[] GeneratePayload(DadaGenerada dada) 
        {
            TFG.Protobuf.DadaGenerada message = new TFG.Protobuf.DadaGenerada();
            message.Valor=dada.Valor;
            message.DataGeneracio=Timestamp.FromDateTime(dada.DataGeneracio);
            return MessageExtensions.ToByteArray(message);
        }
        
        public static DadaGenerada Parse(byte[] payload)
        {
            var message = TFG.Protobuf.DadaGenerada.Parser.ParseFrom(payload);

            return new DadaGenerada(message.Valor, message.DataGeneracio.ToDateTime());            
        }
    }

    public static class ProtocolDadaTractada
    {
        public static byte[] GeneratePayload(DadaTractada dada)
        {
            TFG.Protobuf.DadaTractada message = new TFG.Protobuf.DadaTractada();
            message.Valor = dada.Valor;
            message.DataGeneracio = Timestamp.FromDateTime(dada.DataGeneracio);
            return MessageExtensions.ToByteArray(message);
        }

        public static DadaTractada Parse(byte[] payload)
        {
            var message = TFG.Protobuf.DadaTractada.Parser.ParseFrom(payload);

            return new DadaTractada(message.Valor, message.DataGeneracio.ToDateTime());
        }
    }

    public static class ProtocolDatesConsulta
    {
        public static byte[] GeneratePayload(DatesConsulta consulta)
        {
            TFG.Protobuf.DatesConsulta message = new TFG.Protobuf.DatesConsulta();
            message.DataInici = Timestamp.FromDateTime(consulta.DataInici);
            message.DataFi = Timestamp.FromDateTime(consulta.DataFi);
            return MessageExtensions.ToByteArray(message);
        }

        public static DatesConsulta Parse(byte[] payload)
        {
            var message=TFG.Protobuf.DatesConsulta.Parser.ParseFrom(payload);

            return new DatesConsulta(message.DataInici.ToDateTime(),message.DataFi.ToDateTime());
        }
    }

    public static class ProtocolRespostaDades
    {
        public static byte[] GeneratePayload(RespostaDades resposta)
        {
            TFG.Protobuf.RespostaDades message = new TFG.Protobuf.RespostaDades();
            message.Dades.AddRange(resposta.Dades.Select(dada => new TFG.Protobuf.DadaTractada()
            {
                DataGeneracio = Timestamp.FromDateTime(dada.DataGeneracio),
                Valor = dada.Valor
            }));
            return MessageExtensions.ToByteArray(message);
        }
    }
}
