﻿protoc --proto_path ../Protobufs --python_out=./tractament missatges.proto
python -m grpc_tools.protoc --proto_path ../Protobufs  --python_out=./tractament --grpc_python_out=./tractament missatgesRPC.proto
