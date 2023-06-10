using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipusDades;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using TFG.Protobuf;

namespace Protocol
{
    public static class ProtocolDadaGenerada
    {
        public static byte[] GeneratePayload(TipusDades.DadaGenerada dada) 
        {
            TFG.Protobuf.DadaGenerada message = new TFG.Protobuf.DadaGenerada();
            message.Valor=dada.Valor;
            message.NomSensor = dada.Name;
            return MessageExtensions.ToByteArray(message);
        }
        
        public static TipusDades.DadaGenerada Parse(byte[] payload)
        {
            var message = TFG.Protobuf.DadaGenerada.Parser.ParseFrom(payload);

            return new TipusDades.DadaGenerada(message.NomSensor, message.Valor);
        }
    }

    public static class ProtocolDadaCalculada
    {
        public static byte[] GeneratePayload(TipusDades.DadaCalculada dada)
        {
            TFG.Protobuf.DadaCalculada message = new();
            message.Valor= dada.Valor;
            message.NomVariable=dada.NomVariable;
            message.Timestamp = dada.Timestamp; 
            return MessageExtensions.ToByteArray(message);
        }

        public static TipusDades.DadaCalculada Parse(byte[] payload)
        {
            var message=TFG.Protobuf.DadaCalculada.Parser.ParseFrom(payload);
            return new TipusDades.DadaCalculada(message.NomVariable,message.Valor,message.Timestamp);
        }
    }

    /*public static class ProtocolDadaTractada
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
    }*/

    public static class ProtocolConsultaDades
    {
        public static byte[] GeneratePayload(string[] nomsVariables)
        {
            TFG.Protobuf.ConsultaDades message = new TFG.Protobuf.ConsultaDades();
            message.NomVariable.AddRange(nomsVariables);            
            return MessageExtensions.ToByteArray(message);
        }

        public static string[] Parse(byte[] payload)
        {
            var message=TFG.Protobuf.ConsultaDades.Parser.ParseFrom(payload);

            return message.NomVariable.ToArray();
        }
    }

    public static class ProtocolRespostaDades
    {
        public static byte[] GeneratePayload(TipusDades.RespostaDades resposta)
        {
            TFG.Protobuf.RespostaDades message = new TFG.Protobuf.RespostaDades();
            message.Dades.AddRange(resposta.Dades.Select(dada => new TFG.Protobuf.Dada()
            {                
                Valor = dada.valor
            }));
            return MessageExtensions.ToByteArray(message);
        }
    }
}
