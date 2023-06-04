
from email import message
from sqlite3 import Timestamp
import pika
import time
from datetime import datetime
import json
import missatges_pb2
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
        variable = variables[nomVariable]
        nomParametre = funcio.Parametres[nomVariable]
        if variable.Timestamp < calculDada.timeStampActual:
            #Demanar valor
            #Crear nova variable amb valor i timestamp per no afectar les que s'estan calculant
        elif variable.Timestamp > calculDada.timeStampRebut:
            #Cancelar l'operacio, ja s'ha calculat aquesta variable en el futur        

        parametres[nomParametre]=variable.Valor
    #Calcular
    #Actualitzar valors variables de tots els parametres


    
class Funcio:
    def __init__(self):
        self.ReturnValue = ''
        self.Name = ''
        self.Parametres = {}

class ValorVariable:
    def __init__(self):
        self.Valor = 0.0        
        self.Timestamp = 0

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
variables = {}        
funcions = {}
for configFunction in configFunctions['FunctionsDefinition']['Functions']:
    function = Funcio()    
    function.Name = configFunction['Function']
    function.Parametres = {}
    for configParameter in configFunction['Parameters']: 
        function.Parametres[configParameter['AssociatedValue']] = configParameter['Name']
    funcions[function.ReturnValue] = function

funcio = 'TemperaturaPiezometre4500'
parametres= {'resistencia':0}
print(getattr(funcionsCalculVariables,funcio)(**parametres))
while True:
    try:        
        connection = pika.BlockingConnection(pika.ConnectionParameters(host=configRabbitMQ['Host'],port=configRabbitMQ['Port']))
        channel = connection.channel()
        print("Connected")
        channel.queue_declare(configRabbitMQ['Channel'],durable=True)        
        channel.basic_consume(queue=configRabbitMQ['Channel'],                                            
                      auto_ack=True,
                      on_message_callback=callback)        
        channel.start_consuming()
    except Exception as e:
        print(str(datetime.now()) + ": Connection failed "+str(e))
        time.sleep(30)
        continue
        