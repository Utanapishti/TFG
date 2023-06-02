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

WORKDIR /service/tractament
           
EXPOSE 50051

ENTRYPOINT [ "python", "tractamentDades.py" ]