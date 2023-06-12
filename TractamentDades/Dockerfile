FROM python:3-alpine

RUN mkdir /service

COPY Protobufs/ /service/protobufs/

COPY TractamentDades/requirements.txt /service

WORKDIR /service

RUN python -m pip install --upgrade pip

RUN python -m pip install -r requirements.txt

RUN pip install protobuf

RUN apk add --no-cache protoc

COPY TractamentDades/tractament/ /service/tractament/

RUN protoc -I=./protobufs --python_out=./tractament ./protobufs/missatges.proto
RUN python -m grpc_tools.protoc --proto_path ./protobufs  --python_out=./tractament --grpc_python_out=./tractament missatgesRPC.proto


WORKDIR /service/tractament
           
EXPOSE 50051

ENTRYPOINT [ "python", "tractamentDades.py" ]