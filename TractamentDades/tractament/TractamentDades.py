
from email import message
import pika
import time
from datetime import datetime
import json
import missatges_pb2

def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)
    dadaRebuda = missatges_pb2.DadaGenerada()
    dadaRebuda.ParseFromString(body)
    print(" %r",dadaRebuda.valor)
    print(" %r",dadaRebuda.dataGeneracio)

try:
    configFile = open("appsettings.json")
    config = json.load(configFile)    
except:
    print("Could not read appsettings.json")    
    config=json.loads("""{
	"RabbitMQ": {
		"Host": "localhost",
		"Port": 5672,
		"Exchange": "SENSOR_NET",
		"Channel": "dadesGenerades"
	}
}""")
configRabbitMQ=config['RabbitMQ']
while True:
    try:
        connection = pika.BlockingConnection(pika.ConnectionParameters(host=configRabbitMQ['Host'],port=configRabbitMQ['Port']))
        channel = connection.channel()
        print("Connected")
        channel.queue_declare("dadesGenerades")
        channel.queue_bind(queue="dadesGenerades",
                      exchange=configRabbitMQ['Exchange'],
                      routing_key=configRabbitMQ['Channel'])        
        channel.basic_consume(queue="dadesGenerades",                                            
                      auto_ack=True,
                      on_message_callback=callback)        
        channel.start_consuming()
    except Exception as e:
        print(str(datetime.now()) + ": Connection failed "+str(e))
        time.sleep(30)
        continue
        