import traceback
import os
import functools
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
from threading import Thread

def callback(ch, method, properties, body, args):
    thrds = args
    delivery_tag = method.delivery_tag
    print(" [x] Received %r" % body)
    calculDada = missatges_pb2.CalculDada()
    calculDada.ParseFromString(body)
    print("Received petition for: %r" % calculDada.variableCalcular)        
    t = Thread(target=calcular, args=(ch, delivery_tag, calculDada))
    t.start()
    thrds.append(t)    


def calcular(ch, delivery_tag, calculDada):    
    global funcions
    global valorsVariables
    #Cerquem la funcio per calcular aquesta variable
    funcio = funcions.get(calculDada.variableCalcular)
    if funcio is None:
        print("Error, funcio per calcular %r no trobada",calculDada.variableCalcular)
    #Comprovem els parametres que falten
    parametres = {}
    for nomVariable in funcio.Parametres:
        valorVariable = valorsVariables.get(nomVariable)
        nomParametre = funcio.Parametres[nomVariable]
        if valorVariable is None or valorVariable.Timestamp < calculDada.timestampActual:
            #Demanar valor
            peticio = missatgesRPC_pb2.PeticioValor()
            peticio.nomVariable=nomVariable
            channelGRPC = grpc.insecure_channel(configGRPC['Host']+":"+str(configGRPC['Port']),options=(('grpc.enable_http_proxy', 0),))
            stubValorService = missatgesRPC_pb2_grpc.ValorServiceStub(channelGRPC)
            resultat = stubValorService.Valor(peticio);
            #Crear nova variable amb valor i timestamp per no afectar les que s'estan calculant
            valorVariable=ValorVariable(resultat.valor,resultat.timestampRebut)
            ActualitzaValor(nomVariable,resultat.valor,resultat.timestampRebut)
        elif valorVariable.Timestamp > calculDada.timestampRebut:
            #Cancelar l'operacio, ja s'ha calculat aquesta variable en el futur        
            return
        parametres[nomParametre]=valorVariable.Valor
    #Calcular
    resultatCalcul = getattr(funcionsCalculVariables,funcio.Name)(**parametres)
    #Actualitzar valor de la variable calculada
    ActualitzaValor(calculDada.variableCalcular,resultatCalcul,calculDada.timestampActual)
    #Informar del resultat del calcul
    payload = missatges_pb2.DadaCalculada()
    payload.nomVariable = calculDada.variableCalcular
    payload.valor = resultatCalcul
    payload.timestamp = calculDada.timestampRebut
    cb = functools.partial(ack_message_send_result, ch, delivery_tag,payload)
    ch.connection.add_callback_threadsafe(cb)
    

    
def ack_message_send_result(ch, delivery_tag, payload):    
    global queueName
    if ch.is_open:
        ch.basic_ack(delivery_tag)
        ch.basic_publish(exchange='',
            routing_key=queueName,
            body=payload)
    else:        
        print("Channel for calculated data closed")
        pass
    

def ActualitzaValor(nomVariable, valor, timestamp):
    global valorsVariables
    valorsVariables[nomVariable]=ValorVariable(valor,timestamp)


    
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
queueName = configRabbitMQ['Channel']
user = configRabbitMQ.get('User')
password=configRabbitMQ.get('Password')
if user is None:
    user = os.environ.get("RabbitMQCalculDades__User")
if password is None:
    password = os.environ.get("RabbitMQCalculDades__Password")
for configFunction in configFunctions['FunctionsDefinition']['Functions']:
    function = Funcio()    
    function.Name = configFunction['Function']
    function.Parametres = {}
    for configParameter in configFunction['Parameters']: 
        function.Parametres[configParameter['AssociatedValue']] = configParameter['Name']
    funcions[configFunction['ReturnValue']] = function   

while True:
    try:        
        channelGRPC = grpc.insecure_channel(configGRPC['Host']+":"+str(configGRPC['Port']),options=(('grpc.enable_http_proxy', 0),))
        stubValorService = missatgesRPC_pb2_grpc.ValorServiceStub(channelGRPC)
        peticio = missatgesRPC_pb2.PeticioValor()
        peticio.nomVariable='TEST'
        resultat = stubValorService.Valor(peticio);
        credentials = pika.PlainCredentials(user, password)
        connection = pika.BlockingConnection(pika.ConnectionParameters(host=configRabbitMQ['Host'],port=configRabbitMQ['Port'],credentials=credentials))
        channelRabbitMQ = connection.channel()
        print("Connected")
        threads = []
        on_message_callback = functools.partial(callback, args=(threads))
        channelRabbitMQ.queue_declare(queueName,durable=True)        
        channelRabbitMQ.basic_consume(queue=queueName, 
                      auto_ack=False,
                      on_message_callback=on_message_callback)        
        print("Waiting for messages...")
        try:
            channelRabbitMQ.start_consuming()
        except KeyboardInterrupt:
            channelRabbitMQ.stop_consuming()
            for thread in threads:
                thread.join()
        
            connection.close()
            exit
    except Exception as e:
        print(str(datetime.now()) + ": Connection failed "+str(e))
        print(e)
        time.sleep(30)
        continue
        
