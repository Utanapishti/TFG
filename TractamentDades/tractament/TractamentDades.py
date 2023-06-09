import time
from email import message
import queue
from sqlite3 import Timestamp
import pika
import time
from datetime import datetime
import json
import grpc
import missatges_pb2
import missatgesRPC_pb2
import missatgesRPC_pb2_grpc
import collections
import funcionsCalculVariables

def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)
    calculDada = missatges_pb2.CalculDada()
    calculDada.ParseFromString(body)
    print("Received petition for: %r",calculDada.variableCalcular)        
    
    #Cerquem la funcio per calcular aquesta variable
    funcio = funcions[calculDada.variableCalcular]
    if funcio is None:
        print("Error, funcio per calcular %r no trobada",calculDada.variableCalcular)
    #Comprovem els parametres que falten
    parametres = {}
    for nomVariable in funcio.Parametres:
        valorVariable = valorsVariables[nomVariable]
        nomParametre = funcio.Parametres[nomVariable]
        if valorVariable is None or valorVariable.Timestamp < calculDada.timeStampActual:
            #Demanar valor
            peticio = missatgesRPC_pb2.PeticioValor()
            peticio.nomVariable=nomVariable
            channelGRPC = grpc.insecure_channel(configGRPC['Host']+":"+str(configGRPC['Port']),options=(('grpc.enable_http_proxy', 0),))
            stubValorService = missatgesRPC_pb2_grpc.ValorServiceStub(channelGRPC)
            resultat = stubValorService.Valor(peticio);
            #Crear nova variable amb valor i timestamp per no afectar les que s'estan calculant
            valorVariable=ValorVariable(resultat.valor,resultat.timestampRebut)
            ActualitzaValor(nomVariable,resultat.valor,resultat.timestampRebut)
        elif valorVariable.Timestamp > calculDada.timeStampRebut:
            #Cancelar l'operacio, ja s'ha calculat aquesta variable en el futur        
            return
        parametres[nomParametre]=valorVariable.Valor
    #Calcular
    resultatCalcul = getattr(funcionsCalculVariables,funcio)(**parametres)
    #Actualitzar valor de la variable calculada
    ActualitzaValor(calculDada.variableCalcular,resultatCalcul,calculDada.timeStampActual)
    #Informar del resultat del calcul
    payload = missatges_pb2.DadaCalculada()
    channelRabbitMQ.basic_publish(queue=configRabbitMQ['Channel'],
                                  body=payload.SerializeToString())

    

def ActualitzaValor(nomVariable, valor, timestamp):
    valorsVariables[nomVariable]=ValorVariable(nomVariable,valor,timestamp)


    
class Funcio:
    def __init__(self):
        self.ReturnValue = ''
        self.Name = ''
        self.Parametres = {}

class ValorVariable:
    def __init__(self,valor,timestamp):
        self.Valor = valor
        self.Timestamp = timestamp

    def ActualitzaValor(valor, timestamp):
        if (timestamp<Timestamp):
            Valor=valor
            Timestamp=timestamp
        

try:        
    configFile = open("appsettings.json")
    config = json.load(configFile)    
    configFunctions = json.load(open("functionsDefinition.json"))
except:
    print("Could not read appsettings.json")    
    config=json.loads("""{
	"RabbitMQCalculDades": {
		"Host": "localhost",
		"Port": 5672,
		"Channel": "calculDades"
	},
    "GRPCValors": {
		"Host": "localhost",
		"Port":  5098
	}
}""")
    configFunctions = json.loads("""
    {
  "FunctionsDefinition": {
    "Functions": [
      {
        "ReturnValue": "PIE1_TEMP",
        "Function": "TemperaturaPiezometre4500",
        "Parameters": [
          {
            "Name": "resistencia",
            "AssociatedValue": "PIE1_RES"
          }
        ]
      },
      {
        "ReturnValue": "PIE1_PRE",
            "Function": "PressioPiezometre4500",
        "Parameters": [
          {
            "Name": "periode",
            "AssociatedValue": "PIE1_PER"
          },
          {
            "Name": "temperatura",
            "AssociatedValue": "PIE1_TEMP"
          },
          {
            "Name": "pressioAtmosferica",
            "AssociatedValue": "METEO_PRE"
          }
        ]
      }
    ]
  }
}
    """)
configRabbitMQ=config['RabbitMQCalculDades']
configGRPC=config['GRPCValors']
valorsVariables = {}        
funcions = {}
for configFunction in configFunctions['FunctionsDefinition']['Functions']:
    function = Funcio()    
    function.Name = configFunction['Function']
    function.Parametres = {}
    for configParameter in configFunction['Parameters']: 
        function.Parametres[configParameter['AssociatedValue']] = configParameter['Name']
    funcions[function.ReturnValue] = function
    

while True:
    try:        
        channelGRPC = grpc.insecure_channel(configGRPC['Host']+":"+str(configGRPC['Port']),options=(('grpc.enable_http_proxy', 0),))
        stubValorService = missatgesRPC_pb2_grpc.ValorServiceStub(channelGRPC)
        peticio = missatgesRPC_pb2.PeticioValor()
        peticio.nomVariable='TEST'
        resultat = stubValorService.Valor(peticio);

        peticio = missatgesRPC_pb2.PeticioValor()
        peticio.nomVariable='TEST1'
        resultat = stubValorService.Valor(peticio);

        connection = pika.BlockingConnection(pika.ConnectionParameters(host=configRabbitMQ['Host'],port=configRabbitMQ['Port']))
        channelRabbitMQ = connection.channel()
        print("Connected")
        channelRabbitMQ.queue_declare(configRabbitMQ['Channel'],durable=True)        
        channelRabbitMQ.basic_consume(queue=configRabbitMQ['Channel'],                                            
                      auto_ack=True,
                      on_message_callback=callback)
        
        peticio = missatgesRPC_pb2.PeticioValor()
        peticio.nomVariable='TEST2'
        resultat = stubValorService.Valor(peticio);

        time.sleep(10)
        peticio = missatgesRPC_pb2.PeticioValor()
        peticio.nomVariable='TEST3'
        resultat = stubValorService.Valor(peticio);


        print("Waiting for messages...")
        channelRabbitMQ.start_consuming()
    except Exception as e:
        print(str(datetime.now()) + ": Connection failed "+str(e))
        time.sleep(30)
        continue
        