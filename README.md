<!--
SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>

SPDX-License-Identifier: CC0-1.0
-->

# Open Data Hub DataImport Content Api V2

## Main Components

### DataImport Api

Exposed Api where it is possible to start each import.
Data is imported from an interface by using the Helper Project
'LTS Api' ||
'HGV Api'
and sent to RabbitMQ

### DataImport Console

Console Project with Scheduler to Trigger Data Imports.

### Accommodation Transformer

Transforms the Raw Accommodation Data to the Open Data Hub Accommodation Datamodel and sends it to the Open Data Hub Core Api

## Helper Components

### DataModel

DataModel used also on od-api-core

### Helper

Generic Helper Methods

### HGV Api

Getting Data from HGV Api

### LTS Api

Getting Data from LTS Api

### RabbitPusher

Pushes the data to RabbitMQ queue ingress-q. A route key and a data object has to be passed. 

### DataImport Helper

DataImportHelper offers the various DataImport Interfaces like LTS Api &HGV Api and pushes the received data to RabbitMQ. Uses RabbitPusher, LTS Api, HGV Api.
Used in DataImport Api, DataImport Console and also in the Transformer because it could be that a raw data Import triggers another data Import.

### RabbitListener

Test Project obsolete

### MongoDBConnector

Getting Data from MongoDB

### Transformer Helper

Listens to Rabbit queue with a certain route key and starts the Transformer. Used into each Transformer Project. Uses MongoDBConnector to get the Data. Uses also the Data Import Helper if a Transformer needs to trigger another Data import. 
Calls odh-api-core POST Methods

## Testing the Project

### Port Forwarding 

In order to Use the RabbitMQ, MongoDB Instances on Kubernetes it is possible to use port forwarding:

Forward RabbitMQ  
`kubectl port-forward service/rabbitmq 15672:15672 5672:5672 --namespace core`

Forward MongoDB  
`kubectl port-forward service/mongodb-headless 27018:27017 --namespace core`