
import pika
import time
from datetime import datetime
import json

def callback(ch, method, properties, body):
    print(" [x] Received %r" % body)

try:
    configFile = open("appsettings.json")
    config = json.load(configFile)
    configRabbitMQ=config['RabbitMQ']
except:
    print("Could not read appsettings.json")
    while True:
        time.sleep(30)
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
        