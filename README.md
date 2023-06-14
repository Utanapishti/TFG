# TFG

Els directoris GestorDades, Generador, TractamentDades i WebAPI contenen els DockerFile dels microserveis.

Es pot executar build.bat per construir-los i registrar-los al docker.

El directori kube conté els yaml per desplegar a Kubernetes:
 - **component.yaml**: és simplement un servidor de mètriques per que funcionin els autoescaladors. Si ja es disposa de servidor de mètriques no cal instal·lar-lo
 - **rabbitMQ.yaml**: crea el cluster de RabbitMQ. Abans cal aplicar l'operador de kubernetes de RabbitMQ que es troba a https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml        
 - **gestordades.yaml**       
 - **tractament.yaml**    
 - **webapi.yaml**    
 - **estres.yaml**: crea pods *generador* que envien dades cada 5 mil·lisegons    
 - **normal.yaml**: crea pods *generador* que envien dades cada 5 segons i 1 minut
    
