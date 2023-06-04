
from email import message
import pika
import time
from datetime import datetime
import json
import missatges_pb2

def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)
    calculDada = missatges_pb2.CalculDada()
    calculDada.ParseFromString(body)
    print(" %r",calculDada.variableCalcular)
    print(" %r",calculDada.variableRebuda)

try:
    configFile = open("appsettings.json")
    config = json.load(configFile)    
except:
    print("Could not read appsettings.json")    
    config=json.loads("""{
	"RabbitMQCalculDades": {
		"Host": "localhost",
		"Port": 5672,
        "Exchange":"SENSOR_NET",
		"Channel": "calculDades"
	}
}""")
configRabbitMQ=config['RabbitMQCalculDades']
while True:
    try:
        connection = pika.BlockingConnection(pika.ConnectionParameters(host=configRabbitMQ['Host'],port=configRabbitMQ['Port']))
        channel = connection.channel()
        print("Connected")
        channel.queue_declare(configRabbitMQ['Channel'])
        channel.queue_bind(queue=configRabbitMQ['Channel'],
                      exchange=configRabbitMQ['Exchange'])        
        channel.basic_consume(queue=configRabbitMQ['Channel'],                                            
                      auto_ack=True,
                      on_message_callback=callback)        
        channel.start_consuming()
    except Exception as e:
        print(str(datetime.now()) + ": Connection failed "+str(e))
        time.sleep(30)
        continue
        